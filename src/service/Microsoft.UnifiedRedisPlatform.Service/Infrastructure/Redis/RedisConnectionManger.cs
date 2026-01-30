using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.StackExchangeRedis;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Redis;
using AppInsights.EnterpriseTelemetry;
using AppInsights.EnterpriseTelemetry.Context;

namespace Microsoft.UnifiedPlatform.Service.Redis
{
    public class RedisConnectionManger: IRedisConnectionManager
    {
        private static readonly ConcurrentDictionary<string, IConnectionMultiplexer> Multiplexers = new ConcurrentDictionary<string, IConnectionMultiplexer>();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> ConnectionLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly TokenCredential _tokenCredential;
        private readonly bool _useManagedIdentity;
        private readonly ILogger _logger;

        public RedisConnectionManger(string managedIdentityClientId = null, ILogger logger = null)
        {
            _logger = logger;
            _useManagedIdentity = !string.IsNullOrWhiteSpace(managedIdentityClientId);
            
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
            return new ManagedIdentityCredential(managedIdentityClientId);
#endif
        }

        public async Task<IConnectionMultiplexer> CreateConnectionAsync(string connectionString)
        {
            var redisHost = connectionString?.Split(',')[0] ?? "unknown";
            
            // Fast path: return existing healthy connection
            if (Multiplexers.TryGetValue(connectionString, out var existingMux) && existingMux.IsConnected)
                return existingMux;

            // Get or create a lock for this connection string to prevent duplicate connections
            var connectionLock = ConnectionLocks.GetOrAdd(connectionString, _ => new SemaphoreSlim(1, 1));
            await connectionLock.WaitAsync().ConfigureAwait(false);
            
            try
            {
                // Double-check after acquiring lock
                if (Multiplexers.TryGetValue(connectionString, out existingMux) && existingMux.IsConnected)
                    return existingMux;

                var perfContext = new PerformanceContext($"RedisConnection:{(_useManagedIdentity ? "MI" : "AccessKey")}");
                perfContext.Start();
                
                try
                {
                    var options = await CreateConfigurationOptionsAsync(connectionString).ConfigureAwait(false);
                    var mux = await ConnectionMultiplexer.ConnectAsync(options).ConfigureAwait(false);
                    Multiplexers[connectionString] = mux;
                    
                    perfContext.Stop();
                    perfContext.Properties["RedisHost"] = redisHost;
                    _logger?.Log(perfContext);
                    
                    return mux;
                }
                catch (Exception ex)
                {
                    perfContext.Stop();
                    _logger?.Log(new ExceptionContext { Exception = ex });
                    throw;
                }
            }
            finally
            {
                connectionLock.Release();
            }
        }

        private async Task<ConfigurationOptions> CreateConfigurationOptionsAsync(string connectionString)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.Ssl = true;

            if (_useManagedIdentity && _tokenCredential != null)
            {
                await options.ConfigureForAzureWithTokenCredentialAsync(_tokenCredential);
            }

            return options;
        }
    }
}
