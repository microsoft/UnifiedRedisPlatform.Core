using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.ListGetByIndex(CreateAppKey(key), index, flags));

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.ListGetByIndexAsync(CreateAppKey(key), index, flags));

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListInsertAfter(CreateAppKey(key), pivot, value, flags)))).FirstOrDefault();

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListInsertAfterAsync(CreateAppKey(key), pivot, value, flags)))).FirstOrDefault();

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListInsertBefore(CreateAppKey(key), pivot, value, flags)))).FirstOrDefault();

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListInsertBeforeAsync(CreateAppKey(key), pivot, value, flags)))).FirstOrDefault();

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => _primaryDatabase.ListLeftPop(CreateAppKey(key), flags)))).FirstOrDefault();

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => _primaryDatabase.ListLeftPopAsync(CreateAppKey(key), flags)))).FirstOrDefault();

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListLeftPush(CreateAppKey(key), value, when, flags)))).FirstOrDefault();

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListLeftPush(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public long ListLeftPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListLeftPush(CreateAppKey(key), values, when, flags)))).FirstOrDefault();

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListLeftPushAsync(CreateAppKey(key), value, when, flags)))).FirstOrDefault();

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListLeftPushAsync(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListLeftPushAsync(CreateAppKey(key), values, when, flags)))).FirstOrDefault();

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.ListLength(CreateAppKey(key), flags));

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.ListLengthAsync(CreateAppKey(key), flags));

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.ListRange(CreateAppKey(key), start, stop, flags));

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.ListRangeAsync(CreateAppKey(key), start, stop, flags));

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListRemove(CreateAppKey(key), value, count, flags)))).FirstOrDefault();

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListRemoveAsync(CreateAppKey(key), value, count, flags)))).FirstOrDefault();

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => db.ListRightPop(CreateAppKey(key), flags)))).FirstOrDefault();

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => db.ListRightPopAsync(CreateAppKey(key), flags)))).FirstOrDefault();

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue>)(() => db.ListRightPopLeftPush(CreateAppKey(source), CreateAppKey(destination), flags)))).FirstOrDefault();

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue>>)(() => db.ListRightPopLeftPushAsync(CreateAppKey(source), CreateAppKey(destination), flags)))).FirstOrDefault();

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListRightPush(CreateAppKey(key), value, when, flags)))).FirstOrDefault();

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListRightPush(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public long ListRightPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.ListRightPush(CreateAppKey(key), values, when, flags)))).FirstOrDefault();

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListRightPushAsync(CreateAppKey(key), value, when, flags)))).FirstOrDefault();

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListRightPushAsync(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.ListRightPushAsync(CreateAppKey(key), values, when, flags)))).FirstOrDefault();

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.ListSetByIndex(CreateAppKey(key), index, value, flags); return true; })));

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.ListSetByIndexAsync(CreateAppKey(key), index, value, flags); return Task.FromResult(true); })));

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.ListTrim(key, start, stop, flags); return true; })));

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.ListTrimAsync(key, start, stop, flags); return Task.FromResult(true); })));
    }
}
