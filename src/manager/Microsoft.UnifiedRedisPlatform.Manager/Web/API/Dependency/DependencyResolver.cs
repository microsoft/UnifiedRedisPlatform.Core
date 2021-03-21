using Autofac;
using System.Linq;
using Autofac.Core;
using Microsoft.CQRS;
using Microsoft.Graph;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UnifiedPlatform.Service.Graph;
using Microsoft.UnifiedPlatform.Storage.Client;
using Microsoft.UnifiedRedisPlatform.Manager.Domain;
using Microsoft.UnifiedPlatform.Service.Common.Graph;
using Microsoft.UnifiedPlatform.Service.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Secrets;
using Microsoft.UnifiedPlatform.Service.Authentication;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;
using Microsoft.UnifiedPlatform.Service.Configuration.Providers;
using Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Results;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries.Handlers;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Handlers;
using Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers;

namespace Microsoft.UnifiedRedisPlatform.Manager.API.Dependency
{
    public class DependencyResolver : Module
    {
        private const int MasterConfigurationProviderKey = 0;
        private const int AppSettingsConfigurationProviderKey = 1;
        private const int SecretsConfigurationProviderKey = 2;
        private const int StorageConfigurationProviderKey = 5;

        private const int AadAuthenticatorKey = 1;

        protected readonly IConfiguration _configuration;
        public DependencyResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterAppSettingsConfigurationProvider(builder);
            RegisterMemoryCache(builder);

            //RegisterApplicationInsights(builder);
            RegisterKeyVault(builder);
            RegisterGraph(builder);
            RegisterStorageConfigurationProvider(builder);
            RegisterConfigurationProviders(builder);
            RegisterAuthenticators(builder);

            RegisterRequestHandlerResolver(builder);
            RegisterQueries(builder);
            RegisterCommands(builder);
        }

        protected virtual void RegisterAppSettingsConfigurationProvider(ContainerBuilder builder)
        {
            builder.Register(scope => _configuration)
                .As<IConfiguration>();

            builder.RegisterType<AppSettingsConfigurationProvider>()
                .Keyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey)
                .As<BaseConfigurationProvider>();
        }

        protected virtual void RegisterMemoryCache(ContainerBuilder builder)
        {
            builder.Register(scope =>
            {
                var cacheOptions = new MemoryCacheOptions();
                var memoryCache = new MemoryCache(cacheOptions);
                return memoryCache;
            }).As<IMemoryCache>();

            builder.RegisterType<InMemoryCache>()
                .As<ICacheService>()
                .SingleInstance();
        }

        protected virtual void RegisterKeyVault(ContainerBuilder builder)
        {
            builder.RegisterType<KeyVaultProvider>()
                .As<ISecretsProvider>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "keyVaultName",
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey).GetConfiguration("KeyVault", "Name").Result))
                .SingleInstance();

            builder.RegisterType<SecretsConfigurationProvider>()
                .Keyed<BaseConfigurationProvider>(SecretsConfigurationProviderKey)
                .As<BaseConfigurationProvider>();
        }

        protected virtual void RegisterGraph(ContainerBuilder builder)
        {
            var tenant = _configuration.GetValue<string>("Graph:Tenant");
            var instance = _configuration.GetValue<string>("Graph:Instance");
            var graphClientId = _configuration.GetValue<string>("Graph:ClientId");
            var authority = string.Format(instance, tenant);
            var scopes = new string[] { _configuration.GetValue<string>("Graph:Scope") };

            builder.Register(scope =>
            {
                var configurationProvider = scope.ResolveKeyed<BaseConfigurationProvider>(MasterConfigurationProviderKey);
                var tenant = configurationProvider.GetConfiguration("Authentication", "AAD:Tenant").Result;
                var instance = configurationProvider.GetConfiguration("Authentication", "AAD:Instance").Result;
                var graphClientId = configurationProvider.GetConfiguration("Graph", "ClientId").Result;
                var scopes = new string[] { configurationProvider.GetConfiguration("Graph", "Scope").Result };
                var graphClientSecret = configurationProvider.GetConfiguration("Graph", "ClientSecret").Result;

                var confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(graphClientId)
                    .WithTenantId(tenant)
                    .WithClientSecret(graphClientSecret)
                    .Build();

                var graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (req) =>
                {
                    var authResult = await confidentialClientApplication
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                }));

                return new GraphManager(scope.Resolve<ICacheService>(), graphServiceClient);
            }).As<IGraphManager>().SingleInstance();
        }

        protected virtual void RegisterStorageConfigurationProvider(ContainerBuilder builder)
        {
            builder.RegisterType<StorageConfigurationResolver>()
                .As<IConfigurationResolver<StorageConfiguration>>()
                .WithParameter(new ResolvedParameter(
                (pi, ctx) => pi.Name.ToLowerInvariant() == "appSettingsConfigurationProvider".ToLowerInvariant(),
                (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey)))
                .WithParameter(new ResolvedParameter(
                (pi, ctx) => pi.Name.ToLowerInvariant() == "secretConfigurationProvider".ToLowerInvariant(),
                (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(SecretsConfigurationProviderKey)));

            builder.Register(ctx =>
            {
                var storageConfigResolver = ctx.Resolve<IConfigurationResolver<StorageConfiguration>>();
                return storageConfigResolver.Resolve();
            }).As<StorageConfiguration>()
            .SingleInstance();


            builder.RegisterType<StorageClientManager>()
                .As<IStorageClientManager>()
                .SingleInstance();

            builder.RegisterType<TableReader<ConfigurationEntity>>()
                .As<ITableReader<ConfigurationEntity>>()
                .SingleInstance();

            builder.RegisterType<BlobReader>()
                .As<IBlobReader>()
                .SingleInstance();

            builder.RegisterType<StorageConfigurationProvider>()
                .Keyed<BaseConfigurationProvider>(StorageConfigurationProviderKey)
                .As<BaseConfigurationProvider>()
                .SingleInstance();
        }

        protected virtual void RegisterConfigurationProviders(ContainerBuilder builder)
        {
            #region App Metadata Configuration
            builder.RegisterType<AppMetadataConfigurationResolver>()
                    .As<IConfigurationResolver<AppMetadataConfiguration>>()
                    .WithParameter(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(BaseConfigurationProvider),
                        (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey)));

            builder.Register(ctx =>
            {
                var appMetadataConfigurationResolver = ctx.Resolve<IConfigurationResolver<AppMetadataConfiguration>>();
                return appMetadataConfigurationResolver.Resolve();
            }).As<AppMetadataConfiguration>()
            .SingleInstance();
            #endregion App Metadata Configuration

            builder.RegisterType<HttpClientFactory>()
                .As<IHttpClientFactory>();

            builder.RegisterType<ConfigurationProviderChainBuilder>()
                .As<IConfigurationProviderChainBuilder>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(List<BaseConfigurationProvider>),
                    (pi, ctx) => ctx.Resolve<IEnumerable<BaseConfigurationProvider>>().ToList()));

            builder.Register(scope =>
            {
                var configurationProviderChainBuilder = scope.Resolve<IConfigurationProviderChainBuilder>();
                return configurationProviderChainBuilder.Build();
            })
            .Keyed<BaseConfigurationProvider>(MasterConfigurationProviderKey)
            .SingleInstance();

            builder.RegisterType<ClusterConfigurationProvider>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(BaseConfigurationProvider),
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(MasterConfigurationProviderKey)))
                .As<IClusterConfigurationProvider>();
        }

        protected virtual void RegisterAuthenticators(ContainerBuilder builder)
        {
            builder.RegisterType<AadAuthenticator>()
                .Keyed<IAuthenticator>(AadAuthenticatorKey)
                .As<IAuthenticator>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "authority",
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey).GetConfiguration("Authentication", "AAD:Authority").Result))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "clientId",
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey).GetConfiguration("Authentication", "AAD:Audience").Result))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "clientSecret",
                    (pi, ctx) => ctx.Resolve<ISecretsProvider>().GetSecret("Authentication-AAD-Secret").Result))
                .SingleInstance();

            builder.RegisterType<AuthorizationContext>()
                .As<IAuthorizationContext>()
                .SingleInstance();

            builder.RegisterType<ApplicationAuthenticator>()
                .As<IApplicationAuthenticator>()
                .SingleInstance()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(BaseConfigurationProvider),
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(MasterConfigurationProviderKey)
                    ))
                .SingleInstance();
        }

        protected virtual void RegisterRequestHandlerResolver(ContainerBuilder builder)
        {
            builder.RegisterType<UnifiedConnectionMultiplexerFactory>()
                .As<IUnifiedConnectionMultiplexerFactory>()
                .SingleInstance();

            builder.Register(ctx =>
            {
                var componentContext = ctx.Resolve<IComponentContext>();
                return new RequestHandlerResolver(requiredType => componentContext.Resolve(requiredType));
            })
            .As<IRequestHandlerResolver>();
        }

        protected virtual void RegisterQueries(ContainerBuilder builder)
        {
            builder.RegisterType<GetAuthroizedApplicationsQueryHandler>()
                .As<QueryHandler<GetAuthorizedApplicationsQuery, List<ClusterConfigurationDto>>>()
                .SingleInstance();
            
            builder.RegisterType<GetApplicationsKeysQueryHandler>()
                .As<QueryHandler<GetApplicationKeysQuery, List<UnifiedRedisKey>>>()
                .SingleInstance();

            builder.RegisterType<GetKeyQueryHandler>()
                .As<QueryHandler<GetKeyQuery, UnifiedRedisKey>>()
                .SingleInstance();

            builder.RegisterType<QueryService>()
                .As<IQueryService>()
                .SingleInstance();
        }

        protected virtual void RegisterCommands(ContainerBuilder builder)
        {
            builder.RegisterType<FlushKeysCommandHandler>()
                .As<CommandHandler<FlushKeysCommand, KeysResult>>()
                .SingleInstance();

            builder.RegisterType<DeleteKeyCommandHandler>()
                .As<CommandHandler<DeleteKeyCommand, IdCommandResult>>()
                .SingleInstance();

            builder.RegisterType<CreateKeyCommandHandler>()
                .As<CommandHandler<CreateKeyCommand, IdCommandResult>>()
                .SingleInstance();

            builder.RegisterType<CommandBus>()
                .As<ICommandBus>()
                .SingleInstance();
        }
    }
}
