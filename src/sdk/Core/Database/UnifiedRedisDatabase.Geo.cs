using System;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.GeoAdd(CreateAppKey(key), longitude, latitude, member, flags))));

        public bool GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.GeoAdd(CreateAppKey(key), value, flags))));

        public long GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<long>)(() => db.GeoAdd(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.GeoAddAsync(CreateAppKey(key), longitude, latitude, member, flags))));

        public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.GeoAddAsync(CreateAppKey(key), value, flags))));

        public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<long>>)(() => db.GeoAddAsync(CreateAppKey(key), values, flags)))).FirstOrDefault();

        public double? GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoDistance(CreateAppKey(key), member1, member2, unit, flags));

        public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoDistanceAsync(CreateAppKey(key), member1, member2, unit, flags));

        public string[] GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoHash(CreateAppKey(key), members, flags));

        public string GeoHash(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoHash(CreateAppKey(key), member, flags));

        public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoHashAsync(CreateAppKey(key), members, flags));

        public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoHashAsync(CreateAppKey(key), member, flags));

        public GeoPosition?[] GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoPosition(CreateAppKey(key), members, flags));

        public GeoPosition? GeoPosition(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoPosition(CreateAppKey(key), member, flags));

        public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoPositionAsync(CreateAppKey(key), members, flags));

        public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoPositionAsync(CreateAppKey(key), member, flags));

        public GeoRadiusResult[] GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoRadius(CreateAppKey(key), member, radius, unit, count, order, options, flags));

        public GeoRadiusResult[] GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.GeoRadius(CreateAppKey(key), longitude, latitude, radius, unit, count, order, options, flags));

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoRadiusAsync(CreateAppKey(key), member, radius, unit, count, order, options, flags));

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.GeoRadiusAsync(CreateAppKey(key), longitude, latitude, radius, unit, count, order, options, flags));

        public bool GeoRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.GeoRemove(CreateAppKey(key), member, flags))));

        public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.GeoRemoveAsync(CreateAppKey(key), member, flags))));
    }
}
