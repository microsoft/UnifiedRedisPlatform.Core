using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    public partial class UnifiedRedisCache
    {   
#pragma warning disable CS0618 // GetKeysAsync is intentionally exposed for cache management scenarios
        public async Task<IEnumerable<RedisKey>> GetKeys(string pattern = "")
        {
            return await _connectionMultiplexer.GetKeysAsync(pattern);
        }
#pragma warning restore CS0618
    }
}
