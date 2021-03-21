using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.StringSet(CreateAppKey(key), value, expiry, when, flags))));
        //{
        //    //IEnumerable<Func<bool>> actions = _writeDatabases.Select(db => { bool act() => db.StringSet(key, value, expiry, when, flags); return (Func<bool>)act; });
        //    //Func<bool>[] actions = _writeDatabases.Select(db => (Func<bool>)(() => db.StringSet(key, value, expiry, when, flags))).ToArray();
        //    return ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.StringSet(key, value, expiry, when, flags))).ToArray()).First();
        //}
        //database => database.StringSet(CreateAppKey(key), value, expiry, when, flags)

        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var appValues = values.Select(pair => new KeyValuePair<RedisKey, RedisValue>(CreateAppKey(pair.Key), pair.Value));
            return ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.StringSet(appValues.ToArray(), when, flags))));
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.StringSetAsync(CreateAppKey(key), value, expiry, when, flags))));

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var appValues = values.Select(pair => new KeyValuePair<RedisKey, RedisValue>(CreateAppKey(pair.Key), pair.Value)).ToArray();
            return ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.StringSetAsync(appValues, when, flags))));
        }

        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None) =>
           ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => _primaryDatabase.StringSetBit(CreateAppKey(key), offset, bit, flags))));

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => _primaryDatabase.StringSetBitAsync(CreateAppKey(key), offset, bit, flags))));

        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None) =>
           ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => db.StringSetRange(CreateAppKey(key), offset, value, flags)))).First();

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => db.StringSetRangeAsync(CreateAppKey(key), offset, value, flags)))).First();

        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => _primaryDatabase.StringGetSet(CreateAppKey(key), value, flags)))).First();

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => _primaryDatabase.StringGetSetAsync(CreateAppKey(key), value, flags)))).First();
    }
}
