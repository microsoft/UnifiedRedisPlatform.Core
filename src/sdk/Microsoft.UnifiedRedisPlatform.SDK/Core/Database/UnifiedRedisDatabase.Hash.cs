using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.HashDecrement(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<double>)(() => db.HashDecrement(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.HashDecrementAsync(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<double>>)(() => db.HashDecrementAsync(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.HashDelete(CreateAppKey(key), hashField, flags))));

        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.HashDelete(CreateAppKey(key), hashFields, flags)))).FirstOrDefault();

        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.HashDeleteAsync(CreateAppKey(key), hashField, flags))));

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.HashDeleteAsync(CreateAppKey(key), hashFields, flags)))).FirstOrDefault();

        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashExists(CreateAppKey(key), hashField, flags));

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashExistsAsync(CreateAppKey(key), hashField, flags));

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashGet(CreateAppKey(key), hashField, flags));

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashGet(CreateAppKey(key), hashFields, flags));

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashGetAll(CreateAppKey(key), flags));

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashGetAllAsync(CreateAppKey(key), flags));

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashGetAsync(CreateAppKey(key), hashField, flags));

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashGetAsync(CreateAppKey(key), hashFields, flags));

        public Lease<byte> HashGetLease(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashGetLease(CreateAppKey(key), hashField, flags));

        public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashGetLeaseAsync(CreateAppKey(key), hashField, flags));

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.HashIncrement(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<double>)(() => db.HashIncrement(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.HashIncrementAsync(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<double>>)(() => db.HashIncrementAsync(CreateAppKey(key), hashField, value, flags)))).FirstOrDefault();

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashKeys(CreateAppKey(key), flags));

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashKeysAsync(CreateAppKey(key), flags));

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashLength(CreateAppKey(key), flags));

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashLengthAsync(CreateAppKey(key), flags));

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags) =>
            Execute(() => _primaryDatabase.HashScan(CreateAppKey(key), pattern, pageSize, flags));

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashScan(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags));

        public IAsyncEnumerable<HashEntry> HashScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashScanAsync(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags));

        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.HashSet(CreateAppKey(key), hashFields, flags); return true; })));

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.HashSet(CreateAppKey(key), hashField, value, when, flags))));

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.HashSetAsync(CreateAppKey(key), hashFields, flags); return Task.FromResult(true); })));

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.HashSetAsync(CreateAppKey(key), hashField, value, when, flags))));

        public long HashStringLength(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashStringLength(key, value, flags));

        public Task<long> HashStringLengthAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashStringLengthAsync(key, value, flags));

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.HashValues(CreateAppKey(key), flags));

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.HashValuesAsync(CreateAppKey(key), flags));
    }
}
