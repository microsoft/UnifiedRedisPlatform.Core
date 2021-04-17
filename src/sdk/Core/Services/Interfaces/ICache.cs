using System;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces
{
    internal interface ICache
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, DateTime expireOn);
        void Remove(string key);
    }
}
