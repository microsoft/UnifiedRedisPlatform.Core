using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.HyperLogLogAdd(CreateAppKey(key), value, flags))));

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.HyperLogLogAdd(CreateAppKey(key), values, flags))));

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.HyperLogLogAddAsync(CreateAppKey(key), value, flags))));

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.HyperLogLogAddAsync(CreateAppKey(key), values, flags))));

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HyperLogLogLength(CreateAppKey(key), flags));

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HyperLogLogLength(CreateAppKeys(keys), flags));

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HyperLogLogLengthAsync(CreateAppKey(key), flags));

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HyperLogLogLengthAsync(CreateAppKeys(keys), flags));

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.HyperLogLogMerge(CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags); return true; })));

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.HyperLogLogMerge(CreateAppKey(destination), CreateAppKeys(sourceKeys), flags); return true; })));

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.HyperLogLogMergeAsync(CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags); return Task.FromResult(true); })));

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.HyperLogLogMergeAsync(CreateAppKey(destination), CreateAppKeys(sourceKeys), flags); return Task.FromResult(true); })));
    }
}
