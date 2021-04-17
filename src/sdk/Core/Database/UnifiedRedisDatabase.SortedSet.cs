using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SortedSetAdd(CreateAppKey(key), member, score, flags))));

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SortedSetAdd(CreateAppKey(key), member, score, when, flags))));

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetAdd(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetAdd(CreateAppKey(key), values, when, flags)))).FirstOrDefault();

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SortedSetAddAsync(CreateAppKey(key), member, score, flags))));

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SortedSetAddAsync(CreateAppKey(key), member, score, when, flags))));

        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetAddAsync(CreateAppKey(key), values, flags)))))).FirstOrDefault();

        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWrite(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetAddAsync(CreateAppKey(key), values, when, flags)))))).FirstOrDefault();

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetCombineAndStore(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), aggregate, flags)))).FirstOrDefault();

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetCombineAndStore(operation, CreateAppKey(destination), CreateAppKeys(keys), weights, aggregate, flags)))).FirstOrDefault();

        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetCombineAndStoreAsync(operation, CreateAppKey(destination), CreateAppKey(first), CreateAppKey(second), aggregate, flags)))))).FirstOrDefault();

        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetCombineAndStoreAsync(operation, CreateAppKey(destination), CreateAppKeys(keys), weights, aggregate, flags)))))).FirstOrDefault();

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<double>)(() => db.SortedSetDecrement(CreateAppKey(key), member, value, flags)))).First();

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<double>>)(() => db.SortedSetDecrementAsync(CreateAppKey(key), member, value, flags)))).First();

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<double>)(() => db.SortedSetIncrement(CreateAppKey(key), member, value, flags)))).First();

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<double>>)(() => db.SortedSetIncrementAsync(CreateAppKey(key), member, value, flags)))).First();

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetLength(CreateAppKey(key), min, max, exclude, flags));

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetLengthAsync(CreateAppKey(key), min, max, exclude, flags));

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetLengthByValue(CreateAppKey(key), min, max, exclude, flags));

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetLengthByValueAsync(CreateAppKey(key), min, max, exclude, flags));

        public SortedSetEntry? SortedSetPop(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<SortedSetEntry?>)(() => db.SortedSetPop(CreateAppKey(key), order, flags)))).FirstOrDefault();

        public SortedSetEntry[] SortedSetPop(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<SortedSetEntry[]>)(() => db.SortedSetPop(CreateAppKey(key), count, order, flags)))).FirstOrDefault();

        public Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<SortedSetEntry?>>)(() => db.SortedSetPopAsync(CreateAppKey(key), order, flags)))).FirstOrDefault();

        public Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<SortedSetEntry[]>>)(() => db.SortedSetPopAsync(CreateAppKey(key), count, order, flags)))).FirstOrDefault();

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByRank(CreateAppKey(key), start, stop, order, flags));

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByRankAsync(CreateAppKey(key), start, stop, order, flags));

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByRankWithScores(CreateAppKey(key), start, stop, order, flags));

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByRankWithScoresAsync(CreateAppKey(key), start, stop, order, flags));

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByScore(CreateAppKey(key), start, stop, exclude, order, skip, take, flags));

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByScoreAsync(CreateAppKey(key), start, stop, exclude, order, skip, take, flags));

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByScoreWithScores(CreateAppKey(key), start, stop, exclude, order, skip, take, flags));

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByScoreWithScoresAsync(CreateAppKey(key), start, stop, exclude, order, skip, take, flags));

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByValue(CreateAppKey(key), min, max, exclude, skip, take, flags));

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = default, RedisValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRangeByValue(CreateAppKey(key), min, max, exclude, order, skip, take, flags));

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByValueAsync(CreateAppKey(key), min, max, exclude, skip, take, flags));

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default, RedisValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRangeByValueAsync(CreateAppKey(key), min, max, exclude, order, skip, take, flags));

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetRank(CreateAppKey(key), member, order, flags));

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetRankAsync(CreateAppKey(key), member, order, flags));

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.SortedSetRemove(CreateAppKey(key), member, flags))));

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetRemove(CreateAppKey(key), members, flags)))).FirstOrDefault();

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.SortedSetRemoveAsync(CreateAppKey(key), member, flags))));

        public async Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetRemoveAsync(CreateAppKey(key), members, flags)))))).FirstOrDefault();

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetRemoveRangeByRank(CreateAppKey(key), start, stop, flags)))).FirstOrDefault();

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetRemoveRangeByRankAsync(CreateAppKey(key), start, stop, flags)))).FirstOrDefault();

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetRemoveRangeByScore(CreateAppKey(key), start, stop, exclude, flags)))).FirstOrDefault();

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetRemoveRangeByScoreAsync(CreateAppKey(key), start, stop, exclude, flags)))).FirstOrDefault();

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortedSetRemoveRangeByValue(CreateAppKey(key), min, max, exclude, flags)))).FirstOrDefault();

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortedSetRemoveRangeByValueAsync(CreateAppKey(key), min, max, exclude, flags)))).FirstOrDefault();

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags) =>
            Execute(() => _primaryDatabase.SortedSetScan(CreateAppKey(key), pattern, pageSize, flags));

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetScan(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags));

        public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(RedisKey key, RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset, flags));

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.SortedSetScore(CreateAppKey(key), member, flags));

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.SortedSetScoreAsync(CreateAppKey(key), member, flags));

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<RedisValue[]>)(() => db.Sort(CreateAppKey(key), skip, take, order, sortType, by, get, flags)))).FirstOrDefault();

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<RedisValue[]>>)(() => db.SortAsync(CreateAppKey(key), skip, take, order, sortType, by, get, flags)))).FirstOrDefault();

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.SortAndStore(CreateAppKey(destination), CreateAppKey(key), skip, take, order, sortType, by, get, flags)))).FirstOrDefault();

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default, RedisValue[] get = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.SortAndStoreAsync(CreateAppKey(destination), CreateAppKey(key), skip, take, order, sortType, by, get, flags)))).FirstOrDefault();
    }
}
