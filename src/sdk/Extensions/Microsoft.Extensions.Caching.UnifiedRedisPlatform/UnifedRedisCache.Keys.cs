using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    public partial class UnifiedRedisCache
    {   
        public async Task<IEnumerable<RedisKey>> GetKeys(string pattern = "")
        {
            return await _connectionMultiplexer.GetKeysAsync(pattern);
        }
    }
}
