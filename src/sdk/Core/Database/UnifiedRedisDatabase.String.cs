using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringAppend(CreateAppKey(key), value, flags));

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringAppendAsync(CreateAppKey(key), value, flags));

        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringBitCount(CreateAppKey(key), start, end, flags));

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringBitCountAsync(CreateAppKey(key), start, end, flags));

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringBitOperation(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags));

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringBitOperation(operation, CreateAppKey(destination), CreateAppKeys(keys), flags));

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default, CommandFlags flags = CommandFlags.None) =>
                ExecuteAsync(() => _primaryDatabase.StringBitOperationAsync(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags));

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
                ExecuteAsync(() => _primaryDatabase.StringBitOperationAsync(operation, CreateAppKey(destination), CreateAppKeys(keys), flags));

        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringBitPosition(CreateAppKey(key), bit, start, end, flags));

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringBitPositionAsync(CreateAppKey(key), bit, start, end, flags));

        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringDecrement(CreateAppKey(key), value, flags));

        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringDecrement(CreateAppKey(key), value, flags));

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringDecrementAsync(CreateAppKey(key), value, flags));

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringDecrementAsync(CreateAppKey(key), value, flags));

        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringIncrement(CreateAppKey(key), value, flags));

        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringIncrement(CreateAppKey(key), value, flags));

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringIncrementAsync(CreateAppKey(key), value, flags));

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringIncrementAsync(CreateAppKey(key), value, flags));

        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StringLength(CreateAppKey(key), flags));

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StringLengthAsync(CreateAppKey(key), flags));
    }
}
