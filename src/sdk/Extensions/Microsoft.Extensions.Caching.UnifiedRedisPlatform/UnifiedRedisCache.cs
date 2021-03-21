using System;
using System.Threading;
using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.UnifiedRedisPlatform.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    public class UnifiedRedisCache : IDistributedCache
    {
        private readonly IUnifiedDatabase _database;

        public UnifiedRedisCache(IOptions<UnifedRedisPlatformOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _database = Connect(options.Value);
        }


        private IUnifiedDatabase Connect(UnifedRedisPlatformOptions options)
        {
            IConnectionMultiplexer connectionMux;
            if (options.ConfigurationOptions != null)
            {
                connectionMux = UnifiedConnectionMultiplexer.Connect(options.ConfigurationOptions);
                return connectionMux.GetDatabase() as IUnifiedDatabase;
            }

            if (string.IsNullOrWhiteSpace(options.Cluster)
                || string.IsNullOrWhiteSpace(options.Application)
                || string.IsNullOrWhiteSpace(options.AppSecret))
                throw new ArgumentNullException("Either ConfigurationOptions or Cluster-App details must be provided");

            string preferredLocation = !string.IsNullOrWhiteSpace(options.PreferredLocation) ? options.PreferredLocation : null;
            connectionMux = UnifiedConnectionMultiplexer.Connect(options.Cluster, options.Application, options.AppSecret, preferredLocation: preferredLocation);
            return connectionMux.GetDatabase() as IUnifiedDatabase;
        }

        public byte[] Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _database.Get(key);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return await _database.GetAsync(key);
        }

        public void Refresh(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _database.ResetWindow(key);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            await _database.ResetWindowAsync(key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _database.KeyDelete(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            await _database.KeyDeleteAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var cachingOptions = CreateCachingOptionsFromDistributedOptions(options);
            _database.Set(key, value, cachingOptions);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var cachingOptions = CreateCachingOptionsFromDistributedOptions(options);
            await _database.SetAsync(key, value, cachingOptions);
        }

        private CachingOptions CreateCachingOptionsFromDistributedOptions(DistributedCacheEntryOptions options)
        {
            if (options == null)
                return null;

            return new CachingOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                RelativeAbsoluteExpiration = options.AbsoluteExpirationRelativeToNow,
                SlidingWindow = options.SlidingExpiration
            };
        }
    }
}
