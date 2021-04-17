using System;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Extensions;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core.Services
{
    internal class InternalCache : ICache
    {
        private static readonly Dictionary<string, object> Memory = new Dictionary<string, object>();
        private static readonly Dictionary<string, DateTime> ExpirationKeys = new Dictionary<string, DateTime>();

        public T Get<T>(string key)
        {
            var value = Memory.GetOrDefault(key);
            if (value == null)
                return default;

            var expireOn = ExpirationKeys.GetOrDefault(key);
            if (expireOn == default)
                return (T)value;

            if (DateTime.UtcNow <= expireOn)
                return (T)value;

            return default;
        }

        public void Remove(string key)
        {
            Memory.TryRemove(key);
            ExpirationKeys.TryRemove(key);
        }

        public void Set<T>(string key, T value)
        {
            Memory.AddOrUpdate(key, value);
        }

        public void Set<T>(string key, T value, DateTime expireOn)
        {
            if (DateTime.UtcNow >= expireOn)
                return;

            Memory.AddOrUpdate(key, value);
            ExpirationKeys.AddOrUpdate(key, expireOn);
        }
    }
}
