using System;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.LockExtend(CreateAppKey(key), value, expiry, flags));
        
        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.LockExtendAsync(CreateAppKey(key), value, expiry, flags));

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.LockQuery(CreateAppKey(key), flags));

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.LockQueryAsync(CreateAppKey(key), flags));

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.LockRelease(CreateAppKey(key), value, flags));

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.LockReleaseAsync(CreateAppKey(key), value, flags));

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.LockTake(CreateAppKey(key), value, expiry, flags));

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.LockTakeAsync(CreateAppKey(key), value, expiry, flags));
    }
}
