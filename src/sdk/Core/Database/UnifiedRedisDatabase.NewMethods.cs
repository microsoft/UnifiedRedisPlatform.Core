using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    /// <summary>
    /// Contains new interface methods added in StackExchange.Redis 2.6+
    /// All methods are passthrough to the underlying _primaryDatabase
    /// </summary>
    public partial class UnifiedRedisDatabase
    {
        #region Geo Operations (New)

        public GeoRadiusResult[] GeoSearch(RedisKey key, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearch(CreateAppKey(key), member, shape, count, demandClosest, order, options, flags);

        public GeoRadiusResult[] GeoSearch(RedisKey key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearch(CreateAppKey(key), longitude, latitude, shape, count, demandClosest, order, options, flags);

        public long GeoSearchAndStore(RedisKey sourceKey, RedisKey destinationKey, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAndStore(CreateAppKey(sourceKey), CreateAppKey(destinationKey), member, shape, count, demandClosest, order, storeDistances, flags);

        public long GeoSearchAndStore(RedisKey sourceKey, RedisKey destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAndStore(CreateAppKey(sourceKey), CreateAppKey(destinationKey), longitude, latitude, shape, count, demandClosest, order, storeDistances, flags);

        public Task<GeoRadiusResult[]> GeoSearchAsync(RedisKey key, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAsync(CreateAppKey(key), member, shape, count, demandClosest, order, options, flags);

        public Task<GeoRadiusResult[]> GeoSearchAsync(RedisKey key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAsync(CreateAppKey(key), longitude, latitude, shape, count, demandClosest, order, options, flags);

        public Task<long> GeoSearchAndStoreAsync(RedisKey sourceKey, RedisKey destinationKey, RedisValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAndStoreAsync(CreateAppKey(sourceKey), CreateAppKey(destinationKey), member, shape, count, demandClosest, order, storeDistances, flags);

        public Task<long> GeoSearchAndStoreAsync(RedisKey sourceKey, RedisKey destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.GeoSearchAndStoreAsync(CreateAppKey(sourceKey), CreateAppKey(destinationKey), longitude, latitude, shape, count, demandClosest, order, storeDistances, flags);

        #endregion

        #region Hash Operations (New)

        public ExpireResult[] HashFieldExpire(RedisKey key, RedisValue[] hashFields, TimeSpan expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldExpire(CreateAppKey(key), hashFields, expiry, when, flags);

        public ExpireResult[] HashFieldExpire(RedisKey key, RedisValue[] hashFields, DateTime expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldExpire(CreateAppKey(key), hashFields, expiry, when, flags);

        public long[] HashFieldGetExpireDateTime(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldGetExpireDateTime(CreateAppKey(key), hashFields, flags);

        public PersistResult[] HashFieldPersist(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldPersist(CreateAppKey(key), hashFields, flags);

        public long[] HashFieldGetTimeToLive(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldGetTimeToLive(CreateAppKey(key), hashFields, flags);

        public RedisValue HashRandomField(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomField(CreateAppKey(key), flags);

        public RedisValue[] HashRandomFields(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomFields(CreateAppKey(key), count, flags);

        public HashEntry[] HashRandomFieldsWithValues(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomFieldsWithValues(CreateAppKey(key), count, flags);

        public IEnumerable<RedisValue> HashScanNoValues(RedisKey key, RedisValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashScanNoValues(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags);

        public Task<ExpireResult[]> HashFieldExpireAsync(RedisKey key, RedisValue[] hashFields, TimeSpan expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldExpireAsync(CreateAppKey(key), hashFields, expiry, when, flags);

        public Task<ExpireResult[]> HashFieldExpireAsync(RedisKey key, RedisValue[] hashFields, DateTime expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldExpireAsync(CreateAppKey(key), hashFields, expiry, when, flags);

        public Task<long[]> HashFieldGetExpireDateTimeAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldGetExpireDateTimeAsync(CreateAppKey(key), hashFields, flags);

        public Task<PersistResult[]> HashFieldPersistAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldPersistAsync(CreateAppKey(key), hashFields, flags);

        public Task<long[]> HashFieldGetTimeToLiveAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashFieldGetTimeToLiveAsync(CreateAppKey(key), hashFields, flags);

        public Task<RedisValue> HashRandomFieldAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomFieldAsync(CreateAppKey(key), flags);

        public Task<RedisValue[]> HashRandomFieldsAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomFieldsAsync(CreateAppKey(key), count, flags);

        public Task<HashEntry[]> HashRandomFieldsWithValuesAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashRandomFieldsWithValuesAsync(CreateAppKey(key), count, flags);

        public IAsyncEnumerable<RedisValue> HashScanNoValuesAsync(RedisKey key, RedisValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.HashScanNoValuesAsync(CreateAppKey(key), pattern, pageSize, cursor, pageOffset, flags);

        #endregion

        #region Key Operations (New)

        public bool KeyCopy(RedisKey sourceKey, RedisKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyCopy(CreateAppKey(sourceKey), CreateAppKey(destinationKey), destinationDatabase, replace, flags);

        public string KeyEncoding(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyEncoding(CreateAppKey(key), flags);

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpire(CreateAppKey(key), expiry, when, flags);

        public bool KeyExpire(RedisKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpire(CreateAppKey(key), expiry, when, flags);

        public DateTime? KeyExpireTime(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpireTime(CreateAppKey(key), flags);

        public long? KeyFrequency(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyFrequency(CreateAppKey(key), flags);

        public long? KeyRefCount(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyRefCount(CreateAppKey(key), flags);

        public Task<bool> KeyCopyAsync(RedisKey sourceKey, RedisKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyCopyAsync(CreateAppKey(sourceKey), CreateAppKey(destinationKey), destinationDatabase, replace, flags);

        public Task<string> KeyEncodingAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyEncodingAsync(CreateAppKey(key), flags);

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpireAsync(CreateAppKey(key), expiry, when, flags);

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpireAsync(CreateAppKey(key), expiry, when, flags);

        public Task<DateTime?> KeyExpireTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyExpireTimeAsync(CreateAppKey(key), flags);

        public Task<long?> KeyFrequencyAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyFrequencyAsync(CreateAppKey(key), flags);

        public Task<long?> KeyRefCountAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.KeyRefCountAsync(CreateAppKey(key), flags);

        #endregion

        #region List Operations (New)

        public RedisValue[] ListLeftPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListLeftPop(CreateAppKey(key), count, flags);

        public ListPopResult ListLeftPop(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListLeftPop(CreateAppKeys(keys), count, flags);

        public long ListPosition(RedisKey key, RedisValue element, long rank = 1, long count = 1, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListPosition(CreateAppKey(key), element, rank, count, flags);

        public long[] ListPositions(RedisKey key, RedisValue element, long rank = 1, long count = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListPositions(CreateAppKey(key), element, rank, count, maxLength, flags);

        public RedisValue ListMove(RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListMove(CreateAppKey(sourceKey), CreateAppKey(destinationKey), sourceSide, destinationSide, flags);

        public RedisValue[] ListRightPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListRightPop(CreateAppKey(key), count, flags);

        public ListPopResult ListRightPop(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListRightPop(CreateAppKeys(keys), count, flags);

        public Task<RedisValue[]> ListLeftPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListLeftPopAsync(CreateAppKey(key), count, flags);

        public Task<ListPopResult> ListLeftPopAsync(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListLeftPopAsync(CreateAppKeys(keys), count, flags);

        public Task<long> ListPositionAsync(RedisKey key, RedisValue element, long rank = 1, long count = 1, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListPositionAsync(CreateAppKey(key), element, rank, count, flags);

        public Task<long[]> ListPositionsAsync(RedisKey key, RedisValue element, long rank = 1, long count = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListPositionsAsync(CreateAppKey(key), element, rank, count, maxLength, flags);

        public Task<RedisValue> ListMoveAsync(RedisKey sourceKey, RedisKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListMoveAsync(CreateAppKey(sourceKey), CreateAppKey(destinationKey), sourceSide, destinationSide, flags);

        public Task<RedisValue[]> ListRightPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListRightPopAsync(CreateAppKey(key), count, flags);

        public Task<ListPopResult> ListRightPopAsync(RedisKey[] keys, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ListRightPopAsync(CreateAppKeys(keys), count, flags);

        #endregion

        #region Script Operations (New)

        public RedisResult ScriptEvaluateReadOnly(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ScriptEvaluateReadOnly(script, keys != null ? CreateAppKeys(keys) : null, values, flags);

        public RedisResult ScriptEvaluateReadOnly(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ScriptEvaluateReadOnly(hash, keys != null ? CreateAppKeys(keys) : null, values, flags);

        public Task<RedisResult> ScriptEvaluateReadOnlyAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ScriptEvaluateReadOnlyAsync(script, keys != null ? CreateAppKeys(keys) : null, values, flags);

        public Task<RedisResult> ScriptEvaluateReadOnlyAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.ScriptEvaluateReadOnlyAsync(hash, keys != null ? CreateAppKeys(keys) : null, values, flags);

        #endregion

        #region Set Operations (New)

        public bool[] SetContains(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SetContains(CreateAppKey(key), values, flags);

        public long SetIntersectionLength(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SetIntersectionLength(CreateAppKeys(keys), limit, flags);

        public Task<bool[]> SetContainsAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SetContainsAsync(CreateAppKey(key), values, flags);

        public Task<long> SetIntersectionLengthAsync(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SetIntersectionLengthAsync(CreateAppKeys(keys), limit, flags);

        #endregion

        #region Sorted Set Operations (New)

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, SortedSetWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetAdd(CreateAppKey(key), member, score, when, flags);

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, SortedSetWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetAdd(CreateAppKey(key), values, when, flags);

        public RedisValue[] SortedSetCombine(SetOperation operation, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetCombine(operation, CreateAppKeys(keys), weights, aggregate, flags);

        public SortedSetEntry[] SortedSetCombineWithScores(SetOperation operation, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetCombineWithScores(operation, CreateAppKeys(keys), weights, aggregate, flags);

        public long SortedSetIntersectionLength(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetIntersectionLength(CreateAppKeys(keys), limit, flags);

        public RedisValue SortedSetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMember(CreateAppKey(key), flags);

        public RedisValue[] SortedSetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMembers(CreateAppKey(key), count, flags);

        public SortedSetEntry[] SortedSetRandomMembersWithScores(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMembersWithScores(CreateAppKey(key), count, flags);

        public long SortedSetRangeAndStore(RedisKey sourceKey, RedisKey destinationKey, RedisValue start, RedisValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRangeAndStore(CreateAppKey(sourceKey), CreateAppKey(destinationKey), start, stop, sortedSetOrder, exclude, order, skip, take, flags);

        public double?[] SortedSetScores(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetScores(CreateAppKey(key), members, flags);

        public SortedSetPopResult SortedSetPop(RedisKey[] keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetPop(CreateAppKeys(keys), count, order, flags);

        public bool SortedSetUpdate(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetUpdate(CreateAppKey(key), member, score, when, flags);

        public long SortedSetUpdate(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetUpdate(CreateAppKey(key), values, when, flags);

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, SortedSetWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetAddAsync(CreateAppKey(key), member, score, when, flags);

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, SortedSetWhen when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetAddAsync(CreateAppKey(key), values, when, flags);

        public Task<RedisValue[]> SortedSetCombineAsync(SetOperation operation, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetCombineAsync(operation, CreateAppKeys(keys), weights, aggregate, flags);

        public Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetCombineWithScoresAsync(operation, CreateAppKeys(keys), weights, aggregate, flags);

        public Task<long> SortedSetIntersectionLengthAsync(RedisKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetIntersectionLengthAsync(CreateAppKeys(keys), limit, flags);

        public Task<RedisValue> SortedSetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMemberAsync(CreateAppKey(key), flags);

        public Task<RedisValue[]> SortedSetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMembersAsync(CreateAppKey(key), count, flags);

        public Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRandomMembersWithScoresAsync(CreateAppKey(key), count, flags);

        public Task<long> SortedSetRangeAndStoreAsync(RedisKey sourceKey, RedisKey destinationKey, RedisValue start, RedisValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetRangeAndStoreAsync(CreateAppKey(sourceKey), CreateAppKey(destinationKey), start, stop, sortedSetOrder, exclude, order, skip, take, flags);

        public Task<double?[]> SortedSetScoresAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetScoresAsync(CreateAppKey(key), members, flags);

        public Task<SortedSetPopResult> SortedSetPopAsync(RedisKey[] keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetPopAsync(CreateAppKeys(keys), count, order, flags);

        public Task<bool> SortedSetUpdateAsync(RedisKey key, RedisValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetUpdateAsync(CreateAppKey(key), member, score, when, flags);

        public Task<long> SortedSetUpdateAsync(RedisKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.SortedSetUpdateAsync(CreateAppKey(key), values, when, flags);

        #endregion

        #region Stream Operations (New)

        public StreamAutoClaimResult StreamAutoClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StreamAutoClaim(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);

        public StreamAutoClaimIdsOnlyResult StreamAutoClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StreamAutoClaimIdsOnly(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);

        public Task<StreamAutoClaimResult> StreamAutoClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StreamAutoClaimAsync(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);

        public Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StreamAutoClaimIdsOnlyAsync(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count, flags);

        #endregion

        #region String Operations (New)

        public long StringBitCount(RedisKey key, long start, long end, StringIndexType indexType, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringBitCount(CreateAppKey(key), start, end, indexType, flags);

        public long StringBitPosition(RedisKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringBitPosition(CreateAppKey(key), bit, start, end, indexType, flags);

        public RedisValue StringGetSetExpiry(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetSetExpiry(CreateAppKey(key), expiry, flags);

        public RedisValue StringGetSetExpiry(RedisKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetSetExpiry(CreateAppKey(key), expiry, flags);

        public RedisValue StringGetDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetDelete(CreateAppKey(key), flags);

        public string StringLongestCommonSubsequence(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequence(CreateAppKey(first), CreateAppKey(second), flags);

        public long StringLongestCommonSubsequenceLength(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequenceLength(CreateAppKey(first), CreateAppKey(second), flags);

        public LCSMatchResult StringLongestCommonSubsequenceWithMatches(RedisKey first, RedisKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequenceWithMatches(CreateAppKey(first), CreateAppKey(second), minLength, flags);

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, When when)
            => _primaryDatabase.StringSet(CreateAppKey(key), value, expiry, when);

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry, bool keepTtl, When when = When.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSet(CreateAppKey(key), value, expiry, keepTtl, when, flags);

        public RedisValue StringSetAndGet(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSetAndGet(CreateAppKey(key), value, expiry, when, flags);

        public RedisValue StringSetAndGet(RedisKey key, RedisValue value, TimeSpan? expiry, bool keepTtl, When when = When.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSetAndGet(CreateAppKey(key), value, expiry, keepTtl, when, flags);

        public Task<long> StringBitCountAsync(RedisKey key, long start, long end, StringIndexType indexType, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringBitCountAsync(CreateAppKey(key), start, end, indexType, flags);

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringBitPositionAsync(CreateAppKey(key), bit, start, end, indexType, flags);

        public Task<RedisValue> StringGetSetExpiryAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetSetExpiryAsync(CreateAppKey(key), expiry, flags);

        public Task<RedisValue> StringGetSetExpiryAsync(RedisKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetSetExpiryAsync(CreateAppKey(key), expiry, flags);

        public Task<RedisValue> StringGetDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringGetDeleteAsync(CreateAppKey(key), flags);

        public Task<string> StringLongestCommonSubsequenceAsync(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequenceAsync(CreateAppKey(first), CreateAppKey(second), flags);

        public Task<long> StringLongestCommonSubsequenceLengthAsync(RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequenceLengthAsync(CreateAppKey(first), CreateAppKey(second), flags);

        public Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(RedisKey first, RedisKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringLongestCommonSubsequenceWithMatchesAsync(CreateAppKey(first), CreateAppKey(second), minLength, flags);

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when)
            => _primaryDatabase.StringSetAsync(CreateAppKey(key), value, expiry, when);

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, bool keepTtl, When when = When.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSetAsync(CreateAppKey(key), value, expiry, keepTtl, when, flags);

        public Task<RedisValue> StringSetAndGetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSetAndGetAsync(CreateAppKey(key), value, expiry, when, flags);

        public Task<RedisValue> StringSetAndGetAsync(RedisKey key, RedisValue value, TimeSpan? expiry, bool keepTtl, When when = When.Always, CommandFlags flags = CommandFlags.None)
            => _primaryDatabase.StringSetAndGetAsync(CreateAppKey(key), value, expiry, keepTtl, when, flags);

        #endregion
    }
}
