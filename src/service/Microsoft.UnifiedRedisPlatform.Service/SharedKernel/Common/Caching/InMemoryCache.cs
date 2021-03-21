using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.UnifiedPlatform.Service.Common.Caching
{
    public class InMemoryCache : ICacheService
    {
        private readonly IMemoryCache _cache;
        public InMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T> Get<T>(string key) => Task.FromResult<T>((T)_cache.Get(key));

        public Task Set<T>(string key, T value)
        {
            _cache.Set(key, value);
            return Task.CompletedTask;
        }

        public Task Set<T>(string key, T value, TimeSpan cacheDuration)
        {
            var expiryOffset = new DateTimeOffset(DateTime.Now).Add(cacheDuration);
            _cache.Set(key, value, expiryOffset);
            return Task.CompletedTask;
        }
    }
}
