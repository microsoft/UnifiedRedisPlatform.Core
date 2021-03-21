using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SetAdd(CreateAppKey(key), value, flags))));

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SetAdd(CreateAppKey(key), values, flags)))).Sum();

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SetAddAsync(CreateAppKey(key), value, flags))));

        public async Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SetAddAsync(CreateAppKey(key), values, flags)))))).Sum();

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue[]>)(() => db.SetCombine(operation, CreateAppKey(first), CreateAppKey(second), flags)))).FirstOrDefault();

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue[]>)(() => db.SetCombine(operation, CreateAppKeys(keys), flags)))).FirstOrDefault();

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue[]>>)(() => db.SetCombineAsync(operation, CreateAppKey(first), CreateAppKey(second), flags)))).FirstOrDefault();

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue[]>>)(() => db.SetCombineAsync(operation, CreateAppKeys(keys), flags)))).FirstOrDefault();

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SetCombineAndStore(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags)))).Sum();

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SetCombineAndStore(operation, CreateAppKey(destination), CreateAppKeys(keys), flags)))).Sum();

        public async Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SetCombineAndStoreAsync(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), flags)))))).Sum();

        public async Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SetCombineAndStoreAsync(operation, CreateAppKey(destination), CreateAppKeys(keys), flags)))))).Sum();

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetContains(CreateAppKey(key), value, flags));

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SetContainsAsync(CreateAppKey(key), value, flags));

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetLength(CreateAppKey(key), flags));

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SetLengthAsync(CreateAppKey(key), flags));

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetMembers(CreateAppKey(key), flags));

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SetMembersAsync(CreateAppKey(key), flags));

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SetMove(CreateAppKey(source), CreateAppKey(destination), value, flags))));

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SetMoveAsync(CreateAppKey(source), CreateAppKey(destination), value, flags))));

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => db.SetPop(CreateAppKey(key), flags)))).FirstOrDefault();

        public RedisValue[] SetPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue[]>)(() => db.SetPop(CreateAppKey(key), count, flags)))).FirstOrDefault();

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => db.SetPopAsync(CreateAppKey(key), flags)))).FirstOrDefault();

        public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue[]>>)(() => db.SetPopAsync(CreateAppKey(key), count, flags)))).FirstOrDefault();

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetRandomMember(CreateAppKey(key), flags));

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SetRandomMemberAsync(CreateAppKey(key), flags));

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetRandomMembers(CreateAppKey(key), count, flags));

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SetRandomMembersAsync(CreateAppKey(key), count, flags));

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SetRemove(CreateAppKey(key), value, flags))));

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SetRemove(CreateAppKey(key), values, flags)))).Sum();

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SetRemoveAsync(CreateAppKey(key), value, flags))));

        public async Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SetRemoveAsync(CreateAppKey(key), values, flags)))))).Sum();

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags) =>
            Execute(() => _primaryDatabase.SetScan(CreateAppKey(key), pattern, pageSize, flags));

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetScan(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags));

        public IAsyncEnumerable<RedisValue> SetScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SetScanAsync(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags));
    }
}
