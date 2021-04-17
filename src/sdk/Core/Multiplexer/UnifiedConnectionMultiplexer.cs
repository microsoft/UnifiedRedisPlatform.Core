using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

        private UnifiedConnectionMultiplexer(string clusterName, string appName, string appSecret, ILogger logger = null, string serviceEndpoint = null, string preferredLocation = null)
        {
            ClusterName = clusterName;
            AppName = appName;
            AppSecret = appSecret;
            _serviceEndpoint = !string.IsNullOrWhiteSpace(serviceEndpoint) && Uri.IsWellFormedUriString(serviceEndpoint, UriKind.Absolute)
                ? serviceEndpoint : Constant.OperationApi.DefaultUrl;

            _redisConnectionProvider = new RedisConnectionBuilder(_serviceEndpoint, ClusterName, appName, appSecret, preferredLocation);
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

            _redisConnectionProvider = new RedisConnectionBuilder(_serviceEndpoint, ClusterName, AppName, AppSecret, serverConfigurationOptions.Region);
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

        public static UnifiedConnectionMultiplexer Connect(string clusterName, string appName, string appSecret, ILogger logger = null, string serviceEndpoint = null, string preferredLocation = null)
        {
            UnifiedConnectionMultiplexer pooledConnection = _pool.FirstOrDefault(connection => connection.ClusterName == clusterName && connection.AppName == appName);
            if (pooledConnection != null)
            {
                if (!pooledConnection.IsConnected)
                {
                    pooledConnection.Close();
                    pooledConnection = new UnifiedConnectionMultiplexer(clusterName, appName, appSecret, logger, serviceEndpoint, preferredLocation);
                }
                return pooledConnection;
            }
            var newConnection = new UnifiedConnectionMultiplexer(clusterName, appName, appSecret, logger, serviceEndpoint, preferredLocation);
            _pool.Add(newConnection);
            return newConnection;
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
            UnifiedConnectionMultiplexer pooledConnection = _pool.FirstOrDefault(connection => connection.ClusterName == localConfiguration.ClusterName && connection.AppName == localConfiguration.AppName);
            if (pooledConnection != null)
            {
                if (!pooledConnection.IsConnected)
                {
                    pooledConnection = new UnifiedConnectionMultiplexer(localConfiguration);
                }

                return pooledConnection;
            }
            UnifiedConnectionMultiplexer newConnection = new UnifiedConnectionMultiplexer(localConfiguration);
            _pool.Add(newConnection);
            return newConnection;
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
