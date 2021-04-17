using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Services;
using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    internal class RedisConnectionBuilder
    {
        private readonly string _serviceEndpoint;
        private readonly string _clusterName;
        private readonly string _appName;
        private readonly string _appSecret;
        private readonly IUnifiedRedisPlatformServiceClient _urpClient;

        public RedisConnectionBuilder(string serviceEndpoint, string clusterName, string appName, string appSecret, string preferredLocation = null)
        {
            _clusterName = clusterName;
            _appName = appName;
            _appSecret = appSecret;
            _serviceEndpoint = !string.IsNullOrWhiteSpace(serviceEndpoint) ? serviceEndpoint : Constant.OperationApi.DefaultUrl;
            _urpClient = new UnifiedRedisPlatformServiceClient(_serviceEndpoint, _clusterName, _appName, _appSecret, preferredLocation);
        }

        public async Task<UnifiedConfigurationServerOptions> GetConfiguration()
        {
            var clusterConfiguration = await _urpClient.GetClusterConfiguration().ConfigureAwait(false);
            var applicationConfiguration = clusterConfiguration.Applications.FirstOrDefault();

            var prefix = $"{clusterConfiguration.RedisCachePrefix}:{applicationConfiguration?.RedisCachePrefix}";
            var primaryConnectionString = clusterConfiguration.RedisConnectionString;
            var unifiedConfiguration = new UnifiedConfigurationServerOptions()
            {
                ClusterName = _clusterName,
                AppName = _appName,
                AppSecret = _appSecret,
                KeyPrefix = prefix,
                ConnectionRetryProtocol = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol,
                OperationsRetryProtocol = applicationConfiguration.ConnectionPreference.OperationalRetryProtocol,
                DiagnosticSettings = applicationConfiguration.DiagnosticSettings,
                Region = clusterConfiguration.PrimaryRedisRegion,
                WritePolicy = applicationConfiguration.WritePolicy
            };
            unifiedConfiguration.BaseConfigurationOptions = CreateRedisConfigurationOption(primaryConnectionString, applicationConfiguration, isSecondaryConnection: false);

            if (clusterConfiguration.AreSecondaryConnectionsPresent)
            {
                foreach (var secondaryConnectionString in clusterConfiguration.SecondaryRedisConnectionStrings)
                {
                    unifiedConfiguration.SecondaryConfigurationsOptions.Add(CreateRedisConfigurationOption(secondaryConnectionString, applicationConfiguration, isSecondaryConnection: true));
                }

            }
            return unifiedConfiguration;
        }

        public async Task<UnifiedConfigurationOptions> GetConfiguration(UnifiedConfigurationOptions currentConfiguration)
        {
            var clusterPreferredConfiguration = await _urpClient.GetClusterConfiguration().ConfigureAwait(false);
            var applicationPreferredConfiguration = clusterPreferredConfiguration.Applications.FirstOrDefault();

            var prefix = $"{clusterPreferredConfiguration.RedisCachePrefix}:{applicationPreferredConfiguration?.RedisCachePrefix}";
            var connectionString = clusterPreferredConfiguration.RedisConnectionString;

            currentConfiguration.KeyPrefix = prefix;
            currentConfiguration.Region = clusterPreferredConfiguration.PrimaryRedisRegion;
            currentConfiguration.WritePolicy = currentConfiguration.WritePolicy ?? applicationPreferredConfiguration.WritePolicy;

            if (currentConfiguration.IsConnectionRetryProtocolSetToDefault)
                currentConfiguration.ConnectionRetryProtocol = applicationPreferredConfiguration?.ConnectionPreference?.ConnectionRetryProtocol;

            if (currentConfiguration.IsOperationsRetryProtocolSetToDefault)
                currentConfiguration.OperationsRetryProtocol = applicationPreferredConfiguration?.ConnectionPreference?.OperationalRetryProtocol;

            if (currentConfiguration.IsDiagnosticSettingsSetToDefault)
                currentConfiguration.DiagnosticSettings = applicationPreferredConfiguration?.DiagnosticSettings;

            currentConfiguration.BaseConfigurationOptions =
                CreateRedisConfigurationOptionsFromExistingConfigurations(connectionString, currentConfiguration.BaseConfigurationOptions, applicationPreferredConfiguration, isSecondaryConnection: false);

            if (clusterPreferredConfiguration.AreSecondaryConnectionsPresent)
            {
                currentConfiguration.SecondaryConfigurationsOptions = new List<ConfigurationOptions>(); // Secondary connections are added from settings
                foreach(var secondaryConnection in clusterPreferredConfiguration.SecondaryRedisConnectionStrings)
                {
                    currentConfiguration.SecondaryConfigurationsOptions.Add(CreateRedisConfigurationOption(secondaryConnection, applicationPreferredConfiguration, isSecondaryConnection: true));
                }
            }

            return currentConfiguration;
        }

        #region Private Builders
        private ConfigurationOptions CreateRedisConfigurationOption(string connectionString, ApplicationConfiguration applicationConfiguration, bool isSecondaryConnection)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            options.ClientName = $"{_appName}-{Guid.NewGuid().ToString()}";
            if (isSecondaryConnection)
                options.ClientName += "secondary";

            options.AbortOnConnectFail = false;
            options.Ssl = true;

            options.ReconnectRetryPolicy = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol;
            options.ConnectRetry = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol.MaxRetryCount;
            options.ConnectTimeout = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol.TimeoutInMs;

            options.SyncTimeout = isSecondaryConnection
                ? applicationConfiguration.ConnectionPreference.SecondaryOperationalRetryProtocol.TimeoutInMs
                : applicationConfiguration.ConnectionPreference.OperationalRetryProtocol.TimeoutInMs;

            options.AsyncTimeout = isSecondaryConnection
                ? applicationConfiguration.ConnectionPreference.SecondaryOperationalRetryProtocol.TimeoutInMs
                : applicationConfiguration.ConnectionPreference.OperationalRetryProtocol.TimeoutInMs;

            return options;
        }

        private ConfigurationOptions CreateRedisConfigurationOptionsFromExistingConfigurations(string connectionString, ConfigurationOptions existingConfiguration, ApplicationConfiguration applicationConfiguration, bool isSecondaryConnection)
        {
            var serverConfiguration = ConfigurationOptions.Parse(connectionString);

            if (existingConfiguration == null) //When client has not set the configuration
            {
                existingConfiguration = serverConfiguration;
            }
            else
            {
                existingConfiguration.Password = serverConfiguration.Password;
                existingConfiguration.EndPoints.Clear();
                foreach (var endpoint in serverConfiguration.EndPoints)
                {
                    existingConfiguration.EndPoints.Add(endpoint);
                }
                existingConfiguration.AbortOnConnectFail = false;
                existingConfiguration.Ssl = true;
                existingConfiguration.SslProtocols = serverConfiguration.SslProtocols;
                existingConfiguration.ClientName = $"{_appName}-{Guid.NewGuid().ToString()}";
                existingConfiguration.ServiceName = serverConfiguration.ServiceName;
                existingConfiguration.ConfigurationChannel = serverConfiguration.ConfigurationChannel;
                existingConfiguration.DefaultVersion = serverConfiguration.DefaultVersion;
                existingConfiguration.ResolveDns = serverConfiguration.ResolveDns;
            }

            existingConfiguration.AllowAdmin =
                existingConfiguration.AllowAdmin || serverConfiguration.AllowAdmin;

            existingConfiguration.CommandMap = existingConfiguration.CommandMap ?? serverConfiguration.CommandMap;

            existingConfiguration.ClientName = $"{_appName}-{Guid.NewGuid().ToString()}";

            if (isSecondaryConnection)
                existingConfiguration.ClientName += "-secondary";

            existingConfiguration.ConfigCheckSeconds =
                existingConfiguration.ConfigCheckSeconds > 1 ? existingConfiguration.ConfigCheckSeconds : serverConfiguration.ConfigCheckSeconds;

            existingConfiguration.ConnectRetry = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol.MaxRetryCount;

            existingConfiguration.ConnectTimeout = applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol.TimeoutInMs;

            existingConfiguration.KeepAlive =
                existingConfiguration.KeepAlive >= 1 ? existingConfiguration.KeepAlive : serverConfiguration.KeepAlive;

            existingConfiguration.Proxy =
                existingConfiguration.Proxy != Proxy.None ? existingConfiguration.Proxy : serverConfiguration.Proxy;

            existingConfiguration.ReconnectRetryPolicy = existingConfiguration.ReconnectRetryPolicy ?? applicationConfiguration.ConnectionPreference.ConnectionRetryProtocol;

            existingConfiguration.SocketManager =
                serverConfiguration.SocketManager ?? existingConfiguration.SocketManager;

            existingConfiguration.SyncTimeout = isSecondaryConnection
                ? applicationConfiguration.ConnectionPreference.SecondaryOperationalRetryProtocol.TimeoutInMs
                : applicationConfiguration.ConnectionPreference.OperationalRetryProtocol.TimeoutInMs;

            existingConfiguration.AsyncTimeout = isSecondaryConnection
                ? applicationConfiguration.ConnectionPreference.SecondaryOperationalRetryProtocol.TimeoutInMs
                : applicationConfiguration.ConnectionPreference.OperationalRetryProtocol.TimeoutInMs;

            existingConfiguration.TieBreaker =
                serverConfiguration.TieBreaker ?? existingConfiguration.TieBreaker;

            return existingConfiguration;
        }
        #endregion Private Builders
    }
}
