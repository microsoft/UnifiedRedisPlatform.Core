using Autofac;
using System.Linq;
using Autofac.Core;
using CQRS.Mediatr.Lite;
using Microsoft.AzureRegion;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UnifiedPlatform.Service.Redis;
using Microsoft.UnifiedPlatform.Storage.Client;
using Microsoft.UnifiedPlatform.Service.Common.Redis;
using Microsoft.UnifiedPlatform.Service.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Secrets;
using Microsoft.UnifiedPlatform.Service.Authentication;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using Microsoft.UnifiedPlatform.Service.Application.Queries;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Application.Commands;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;
using Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient;
using Microsoft.UnifiedPlatform.Service.Configuration.Providers;
using Microsoft.UnifiedPlatform.Service.Application.Queries.Handlers;
using Microsoft.UnifiedPlatform.Service.Application.Commands.Handlers;
using Microsoft.UnifiedPlatform.Service.Common.Configuration.Resolvers;

namespace Microsoft.UnifiedRedisPlatform.Service.Dependencies.DependencyResolution
{
    public class CommonDependencyModule : Module
    {
        private const int MasterConfigurationProviderKey = 0;
        private const int AppSettingsConfigurationProviderKey = 1;
        private const int SecretsConfigurationProviderKey = 2;
        private const int StorageConfigurationProviderKey = 5;

        private const int AadAuthenticatorKey = 1;
        private const int RedisClusterAuthenticatorKey = 2;

        protected readonly IConfiguration _configuration;
        public CommonDependencyModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterAppSettingsConfigurationProvider(builder);
            RegisterMemoryCache(builder);

            RegisterApplicationInsights(builder);
            RegisterKeyVault(builder);
            RegisterAuthenticators(builder);
            RegisterStorageConfigurationProvider(builder);
            RegisterConfigurationProviders(builder);
            RegisterConfigurations(builder);
            RegisterAzureRegionUtility(builder);
            RegisterRedisProviders(builder);

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

        protected virtual void RegisterApplicationInsights(ContainerBuilder builder)
        { 
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

            builder.RegisterType<RedisClusterAuthenticator>()
                .Keyed<IAuthenticator>(RedisClusterAuthenticatorKey)
                .As<IAuthenticator>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "audience",
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey).GetConfiguration("Authentication", "RedisCluster:Audience").Result))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "issuer",
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(AppSettingsConfigurationProviderKey).GetConfiguration("Authentication", "RedisCluster:Issuer").Result))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name == "secret",
                    (pi, ctx) => ctx.Resolve<ISecretsProvider>().GetSecret("Authentication-RedisCluster-Secret").Result))
                .SingleInstance();
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

        protected virtual void RegisterConfigurations(ContainerBuilder builder)
        {   
        }

        protected virtual void RegisterAzureRegionUtility(ContainerBuilder builder)
        {
            builder.RegisterType<AzureRegionUtility>()
                .As<IAzureRegionUtility>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.Name.ToLowerInvariant() == "clientSecret".ToLowerInvariant(),
                    (pi, ctx) => ctx.ResolveKeyed<BaseConfigurationProvider>(SecretsConfigurationProviderKey).GetConfiguration("AzureContributor-SPN", "Secret").Result))
                .SingleInstance();
        }

        protected virtual void RegisterRedisProviders(ContainerBuilder builder)
        {
            builder.RegisterType<RedisConnectionManger>()
                .As<IRedisConnectionManager>()
                .SingleInstance();

            builder.RegisterType<RedisStream>()
                .As<IRedisLogsStream>()
                .SingleInstance();
        }

        protected virtual void RegisterRequestHandlerResolver(ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var componentContext = ctx.Resolve<IComponentContext>();
                return new RequestHandlerResolver(requiredType => componentContext.Resolve(requiredType));
            })
            .As<IRequestHandlerResolver>();
        }

        protected virtual void RegisterQueries(ContainerBuilder builder)
        {
            builder.RegisterType<GetClusterConfigurationQueryHandler>()
                .As<QueryHandler<GetClusterConfigurationQuery, ClusterConfigurationDto>>()
                .SingleInstance();

            builder.RegisterType<GetAllClustersQueryHandler>()
                .As<QueryHandler<GetAllClustersQuery, List<ClusterConfigurationDto>>>()
                .SingleInstance();

            builder.RegisterType<StreamUncommittedLogsQueryHandler>()
                .As<QueryHandler<StreamUncommitedLogsQuery, bool>>()
                .SingleInstance();

            builder.RegisterType<QueryService>()
                .As<IQueryService>()
                .SingleInstance();
        }

        protected virtual void RegisterCommands(ContainerBuilder builder)
        {
            builder.RegisterType<AuthenticateClientCommandHandler>()
                .As<CommandHandler<AuthenticateClientCommand, TokenCommandResult>>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IAuthenticator),
                    (pi, ctx) => ctx.ResolveKeyed<IAuthenticator>(RedisClusterAuthenticatorKey)))
                .SingleInstance();

            builder.RegisterType<LogClientInfoCommandHandler>()
                .As<CommandHandler<LogClientInfoCommand, LogResult>>()
                .SingleInstance();

            builder.RegisterType<CommandBus>()
                .As<ICommandBus>()
                .SingleInstance();
        }
    }
}
