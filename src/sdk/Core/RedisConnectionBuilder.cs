using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Services;
using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.StackExchangeRedis;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    internal class RedisConnectionBuilder
    {
        private readonly string _serviceEndpoint;
        private readonly string _clusterName;
        private readonly string _appName;
        private readonly string _appSecret;
        private readonly string _managedIdentityClientId;
        private readonly IUnifiedRedisPlatformServiceClient _urpClient;
        private readonly TokenCredential _tokenCredential;
        private readonly bool _useManagedIdentity;
        private static readonly string[] RedisScopes = new[] { "https://redis.azure.com/.default" };

        public RedisConnectionBuilder(string serviceEndpoint, string clusterName, string appName, string appSecret, string preferredLocation = null, string managedIdentityClientId = null)
        {
            _clusterName = clusterName;
            _appName = appName;
            _appSecret = appSecret;
            _managedIdentityClientId = managedIdentityClientId;
            _serviceEndpoint = !string.IsNullOrWhiteSpace(serviceEndpoint) ? serviceEndpoint : Constant.OperationApi.DefaultUrl;

            // Client decides whether to use MI based on managedIdentityClientId parameter
            _useManagedIdentity = !string.IsNullOrWhiteSpace(managedIdentityClientId);
            _urpClient = new UnifiedRedisPlatformServiceClient(_serviceEndpoint, _clusterName, _appName, _appSecret, preferredLocation, _useManagedIdentity);

            if (_useManagedIdentity)
            {
                _tokenCredential = CreateTokenCredential(managedIdentityClientId);
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
            unifiedConfiguration.BaseConfigurationOptions = await CreateRedisConfigurationOptionAsync(primaryConnectionString, applicationConfiguration, isSecondaryConnection: false);

            if (clusterConfiguration.AreSecondaryConnectionsPresent)
            {
                foreach (var secondaryConnectionString in clusterConfiguration.SecondaryRedisConnectionStrings)
                {
                    unifiedConfiguration.SecondaryConfigurationsOptions.Add(await CreateRedisConfigurationOptionAsync(secondaryConnectionString, applicationConfiguration, isSecondaryConnection: true));
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
                await CreateRedisConfigurationOptionsFromExistingConfigurationsAsync(connectionString, currentConfiguration.BaseConfigurationOptions, applicationPreferredConfiguration, isSecondaryConnection: false);

            if (clusterPreferredConfiguration.AreSecondaryConnectionsPresent)
            {
                currentConfiguration.SecondaryConfigurationsOptions = new List<ConfigurationOptions>(); // Secondary connections are added from settings
                foreach (var secondaryConnection in clusterPreferredConfiguration.SecondaryRedisConnectionStrings)
                {
                    currentConfiguration.SecondaryConfigurationsOptions.Add(await CreateRedisConfigurationOptionAsync(secondaryConnection, applicationPreferredConfiguration, isSecondaryConnection: true));
                }
            }

            return currentConfiguration;
        }

        #region Private Builders
        /// <summary>
        /// Gets an access token for Redis using Azure Managed Identity.
        /// Only called when _useManagedIdentity is true.
        /// </summary>
        private async Task<string> GetRedisAccessTokenAsync()
        {
            if (!_useManagedIdentity || _tokenCredential == null)
            {
                return null;
            }
            var tokenRequestContext = new TokenRequestContext(RedisScopes);
            var accessToken = await _tokenCredential.GetTokenAsync(tokenRequestContext, default);
            return accessToken.Token;
        }

        private async Task<ConfigurationOptions> CreateRedisConfigurationOptionAsync(string connectionString, ApplicationConfiguration applicationConfiguration, bool isSecondaryConnection)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            options.ClientName = $"{_appName}-{Guid.NewGuid().ToString()}";
            if (isSecondaryConnection)
                options.ClientName += "secondary";

            options.AbortOnConnectFail = false;
            options.Ssl = true;
            options.KeepAlive = 60;  // Send keep-alive every 60 seconds to prevent stale connections

            // Use Managed Identity for Redis authentication if configured
            // ConfigureForAzureWithTokenCredentialAsync handles automatic token refresh
            // Otherwise, keep the password from the connection string (legacy behavior)
            if (_useManagedIdentity && _tokenCredential != null)
            {
                await options.ConfigureForAzureWithTokenCredentialAsync(_tokenCredential).ConfigureAwait(false);
            }
            else
            {
                var token = await GetRedisAccessTokenAsync();
                options.Password = token;
            }

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

        private async Task<ConfigurationOptions> CreateRedisConfigurationOptionsFromExistingConfigurationsAsync(string connectionString, ConfigurationOptions existingConfiguration, ApplicationConfiguration applicationConfiguration, bool isSecondaryConnection)
        {
            var serverConfiguration = ConfigurationOptions.Parse(connectionString);

            if (existingConfiguration == null) //When client has not set the configuration
            {
                existingConfiguration = serverConfiguration;
            }
            else
            {
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

            // Use Managed Identity for Redis authentication if configured
            // ConfigureForAzureWithTokenCredentialAsync handles automatic token refresh
            // Otherwise, keep the password from the connection string (legacy behavior)
            if (_useManagedIdentity && _tokenCredential != null)
            {
                await existingConfiguration.ConfigureForAzureWithTokenCredentialAsync(_tokenCredential).ConfigureAwait(false);
            }
            else
            {
                existingConfiguration.Password = serverConfiguration.Password;
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

            // Ensure keep-alive is set to prevent stale connections (default 60 seconds)
            existingConfiguration.KeepAlive =
                existingConfiguration.KeepAlive >= 1 ? existingConfiguration.KeepAlive : (serverConfiguration.KeepAlive >= 1 ? serverConfiguration.KeepAlive : 60);

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
