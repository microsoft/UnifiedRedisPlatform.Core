using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.StackExchangeRedis;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Constants;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public sealed partial class UnifiedConnectionMultiplexer : IUnifiedConnectionMultiplexer
    {
        public string ClusterName { get; }
        public string AppName { get; }

        private readonly RedisConnectionBuilder _redisConnectionProvider;
        private readonly UnifiedConfigurationOptions _unifiedConfigurations;
        private readonly string _serviceEndpoint;

        private IConnectionMultiplexer _baseConnectionMux;
        private List<IConnectionMultiplexer> _secondaryConnectionMuxs = new List<IConnectionMultiplexer>();
        private ILogger _logger;

        private static readonly ConcurrentBag<UnifiedConnectionMultiplexer> _pool = new ConcurrentBag<UnifiedConnectionMultiplexer>();

        private UnifiedConnectionMultiplexer(string clusterName, string appName, string appSecret, string serviceEndpoint = null, string preferredLocation = null, string managedIdentityClientId = null)
        {
            ClusterName = clusterName;
            AppName = appName;
            AppSecret = appSecret;
            _serviceEndpoint = !string.IsNullOrWhiteSpace(serviceEndpoint) && Uri.IsWellFormedUriString(serviceEndpoint, UriKind.Absolute)
                ? serviceEndpoint : Constant.OperationApi.DefaultUrl;

            _redisConnectionProvider = new RedisConnectionBuilder(_serviceEndpoint, ClusterName, appName, appSecret, preferredLocation, managedIdentityClientId);
            _unifiedConfigurations = _redisConnectionProvider.GetConfiguration().Result;
            ConnectToBaseMultiplexer();
            SetupTelemetry();
        }

        private UnifiedConnectionMultiplexer(UnifiedConfigurationServerOptions serverConfigurationOptions)
        {
            ClusterName = serverConfigurationOptions.ClusterName;
            AppName = serverConfigurationOptions.AppName;
            AppSecret = serverConfigurationOptions.AppSecret;
            _serviceEndpoint = serverConfigurationOptions.ServiceEndpoint;

            _redisConnectionProvider = new RedisConnectionBuilder(_serviceEndpoint, ClusterName, AppName, AppSecret, serverConfigurationOptions.Region, serverConfigurationOptions.ManagedIdentityClientId);
            _unifiedConfigurations = _redisConnectionProvider.GetConfiguration(serverConfigurationOptions).Result;
            ConnectToBaseMultiplexer();
            SetupTelemetry();
        }


        private UnifiedConnectionMultiplexer(UnifiedConfigurationLocalOptions localConfigurationOptions)
        {
            _unifiedConfigurations = localConfigurationOptions;
            ConnectToBaseMultiplexer();
            SetupTelemetry();
        }

        public static UnifiedConnectionMultiplexer Connect(string clusterName, string appName, string appSecret, string serviceEndpoint = null, string preferredLocation = null, string managedIdentityClientId = null)
        {
            UnifiedConnectionMultiplexer pooledConnection = _pool.FirstOrDefault(connection => connection.ClusterName == clusterName && connection.AppName == appName);
            if (pooledConnection != null)
            {
                if (!pooledConnection.IsConnected)
                {
                    pooledConnection.Close();
                    pooledConnection = new UnifiedConnectionMultiplexer(clusterName, appName, appSecret, serviceEndpoint, preferredLocation, managedIdentityClientId);
                }
                return pooledConnection;
            }
            var newConnection = new UnifiedConnectionMultiplexer(clusterName, appName, appSecret, serviceEndpoint, preferredLocation, managedIdentityClientId);
            _pool.Add(newConnection);
            return newConnection;
        }

        /// <summary>
        /// Connects using base configuration options. Automatically determines the correct connection method.
        /// This overload provides backward compatibility for clients using UnifiedConfigurationOptions directly.
        /// </summary>
        /// <param name="configurations">Configuration options (base class, ServerOptions, or LocalOptions)</param>
        /// <returns>A connected UnifiedConnectionMultiplexer instance</returns>
        public static UnifiedConnectionMultiplexer Connect(UnifiedConfigurationOptions configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            // If it's already a derived type, use the specific overload
            if (configurations is UnifiedConfigurationServerOptions serverOptions)
            {
                return Connect(serverOptions);
            }
            else if (configurations is UnifiedConfigurationLocalOptions localOptions)
            {
                return Connect(localOptions);
            }

            // For backward compatibility: treat base UnifiedConfigurationOptions as server options
            // This allows existing clients using "new UnifiedConfigurationOptions()" to continue working
            var serverConfig = new UnifiedConfigurationServerOptions
            {
                ClusterName = configurations.ClusterName,
                AppName = configurations.AppName,
                AppSecret = configurations.AppSecret,
                ManagedIdentityClientId = configurations.ManagedIdentityClientId,
                ServiceEndpoint = configurations.ServiceEndpoint,
                KeyPrefix = configurations.KeyPrefix,
                WritePolicy = configurations.WritePolicy,
                Region = configurations.Region,
                OperationsRetryProtocol = configurations.OperationsRetryProtocol,
                ConnectionRetryProtocol = configurations.ConnectionRetryProtocol,
                DiagnosticSettings = configurations.DiagnosticSettings,
                Logger = configurations.Logger,
                BaseConfigurationOptions = configurations.BaseConfigurationOptions,
                SecondaryConfigurationsOptions = configurations.SecondaryConfigurationsOptions
            };

            return Connect(serverConfig);
        }

        public static UnifiedConnectionMultiplexer Connect(UnifiedConfigurationServerOptions serverConfigurations)
        {
            UnifiedConnectionMultiplexer pooledConnection = _pool.FirstOrDefault(connection => connection.ClusterName == serverConfigurations.ClusterName && connection.AppName == serverConfigurations.AppName);
            if (pooledConnection != null)
            {
                if (!pooledConnection.IsConnected)
                {
                    pooledConnection = new UnifiedConnectionMultiplexer(serverConfigurations);
                }

                return pooledConnection;
            }
            var newConnection = new UnifiedConnectionMultiplexer(serverConfigurations);
            _pool.Add(newConnection);
            return newConnection;
        }

        public static UnifiedConnectionMultiplexer Connect(UnifiedConfigurationLocalOptions localConfiguration)
        {
            return ConnectAsync(localConfiguration).GetAwaiter().GetResult();
        }

        public static async Task<UnifiedConnectionMultiplexer> ConnectAsync(UnifiedConfigurationLocalOptions localConfiguration)
        {
            UnifiedConnectionMultiplexer pooledConnection = _pool.FirstOrDefault(connection => connection.ClusterName == localConfiguration.ClusterName && connection.AppName == localConfiguration.AppName);
            if (pooledConnection != null)
            {
                if (!pooledConnection.IsConnected)
                {
                    await ConfigureManagedIdentityIfNeeded(localConfiguration).ConfigureAwait(false);
                    pooledConnection = new UnifiedConnectionMultiplexer(localConfiguration);
                }

                return pooledConnection;
            }
            
            await ConfigureManagedIdentityIfNeeded(localConfiguration).ConfigureAwait(false);
            UnifiedConnectionMultiplexer newConnection = new UnifiedConnectionMultiplexer(localConfiguration);
            _pool.Add(newConnection);
            return newConnection;
        }

        private static async Task ConfigureManagedIdentityIfNeeded(UnifiedConfigurationLocalOptions localConfiguration)
        {
            if (localConfiguration.UseManagedIdentity)
            {
                var credential = CreateTokenCredential(localConfiguration.ManagedIdentityClientId);
                await localConfiguration.BaseConfigurationOptions.ConfigureForAzureWithTokenCredentialAsync(credential).ConfigureAwait(false);
                
                // Also configure secondary connections if any
                foreach (var secondaryConfig in localConfiguration.SecondaryConfigurationsOptions)
                {
                    await secondaryConfig.ConfigureForAzureWithTokenCredentialAsync(credential).ConfigureAwait(false);
                }
            }
        }

        private static TokenCredential CreateTokenCredential(string managedIdentityClientId)
        {
#if DEBUG
            // Use AzureCliCredential for local development (works with VS Code + az login)
            return new AzureCliCredential();
#else
            // Use User-Assigned Managed Identity for production
            return new ManagedIdentityCredential(managedIdentityClientId);
#endif
        }

        private void ConnectToBaseMultiplexer()
        {
            _unifiedConfigurations.Validate();
            _baseConnectionMux = ConnectionMultiplexer.Connect(_unifiedConfigurations.BaseConfigurationOptions);
            if (_unifiedConfigurations.SecondaryConfigurationsOptions.Any())
            {
                foreach (var secondaryConfiguration in _unifiedConfigurations.SecondaryConfigurationsOptions)
                    _secondaryConnectionMuxs.Add(ConnectionMultiplexer.Connect(secondaryConfiguration));
            }
        }

        private void SetupTelemetry()
        {
            _logger = CreateStrategicLogger(_unifiedConfigurations.Logger);
            _logger.LogEvent("ConnectionCreated", properties: new Dictionary<string, string>() { { "Mode", "Configuration" }, { "ClientLogger", (_unifiedConfigurations.Logger != null).ToString() }, { "ServiceEndoint", _serviceEndpoint }, { "Region", _unifiedConfigurations.Region } });
            SetupEventHandlers();
        }

        private List<IConnectionMultiplexer> GetAllConnectionMultiplexers()
        {
            var connections = new List<IConnectionMultiplexer>() { _baseConnectionMux };
            connections.AddRange(_secondaryConnectionMuxs);
            return connections;
        }
    }
}
