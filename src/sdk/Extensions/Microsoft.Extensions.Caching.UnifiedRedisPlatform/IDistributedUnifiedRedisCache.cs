using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    public interface IDistributedUnifiedRedisCache: IDistributedCache
    {
        /// <summary>
        /// Gets all the keys from URP matching the given pattern. The input pattern accepts '*' wildcard.
        /// </summary>
        /// <param name="pattern">Pattern agains which the keys need to be matched. Leave the pattern blank to get all keys</param>
        /// <remarks>SCAN operation is used internally to get the keys from Redis Cache. SCAN operation is an expensive operation so usage of this method can cause performance latencies.</remarks>
        /// <returns cref="IEnumerable{RedisKey}">List of keys matching the patterns</returns>
        Task<IEnumerable<RedisKey>> GetKeys(string pattern = "");
    }
}
