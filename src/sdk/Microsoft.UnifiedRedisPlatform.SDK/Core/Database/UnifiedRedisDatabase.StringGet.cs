using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringGet(CreateAppKey(key), flags));

        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringGet(CreateAppKeys(keys), flags));

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetAsync(CreateAppKey(key), flags));

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetAsync(CreateAppKeys(keys), flags));

        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringGetBit(CreateAppKey(key), offset, flags));

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetBitAsync(CreateAppKey(key), offset, flags));

        public Lease<byte> StringGetLease(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringGetLease(CreateAppKey(key), flags));

        public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetLeaseAsync(CreateAppKey(key), flags));

        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringGetRange(CreateAppKey(key), start, end, flags));

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetRangeAsync(CreateAppKey(key), start, end, flags));

        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            _primaryDatabase.StringGetWithExpiry(CreateAppKey(key), flags);

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringGetWithExpiryAsync(CreateAppKey(key), flags));
    }
}
