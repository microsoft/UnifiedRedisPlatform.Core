using System;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.Caching
{
    public interface ICacheService
    {
        Task<T> Get<T>(string key);
        Task Set<T>(string key, T value);
        Task Set<T>(string key, T value, TimeSpan cacheDuration);
    }
}
