using System;
using System.Net;
using System.Linq;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.IsConnected(CreateAppKey(key), flags));

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_deleteDatabases.Select(db => (Func<bool>)(() => db.KeyDelete(CreateAppKey(key), flags))));

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_deleteDatabases.Select(db => (Func<long>)(() => db.KeyDelete(CreateAppKeys(keys), flags)))).Sum();

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_deleteDatabases.Select(db => (Func<Task<bool>>)(() => db.KeyDeleteAsync(CreateAppKey(key), flags))));

        public async Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            (await Task.WhenAll(ExecuteWriteAsync(_deleteDatabases.Select(db => (Func<Task<long>>)(() => db.KeyDeleteAsync(CreateAppKeys(keys), flags)))))).Sum();

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyDump(CreateAppKey(key), flags));

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyDumpAsync(CreateAppKey(key), flags));

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyExists(CreateAppKey(key), flags));

        public long KeyExists(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyExists(CreateAppKeys(keys), flags));

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyExistsAsync(CreateAppKey(key), flags));

        public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyExistsAsync(CreateAppKeys(keys), flags));

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.KeyExpire(CreateAppKey(key), expiry, flags))));

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.KeyExpire(CreateAppKey(key), expiry, flags))));

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.KeyExpireAsync(CreateAppKey(key), expiry, flags))));

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.KeyExpireAsync(CreateAppKey(key), expiry, flags))));

        public TimeSpan? KeyIdleTime(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyIdleTime(CreateAppKey(key), flags));

        public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyIdleTimeAsync(CreateAppKey(key), flags));

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { _primaryDatabase.KeyMigrate(CreateAppKey(key), toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags); return true; })));

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { _primaryDatabase.KeyMigrateAsync(CreateAppKey(key), toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags); return Task.FromResult(true); })));

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.KeyMove(CreateAppKey(key), database, flags))));

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.KeyMoveAsync(CreateAppKey(key), database, flags))));

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db =>  (Func<bool>)(() => _primaryDatabase.KeyPersist(CreateAppKey(key), flags))));

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => _primaryDatabase.KeyPersistAsync(CreateAppKey(key), flags))));

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyRandom(flags));

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyRandomAsync(flags));

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => db.KeyRename(CreateAppKey(key), CreateAppKey(newKey), when, flags))));

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => db.KeyRenameAsync(CreateAppKey(key), CreateAppKey(newKey), when, flags))));

        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWrite(_writeDatabases.Select(db => (Func<bool>)(() => { db.KeyRestore(CreateAppKey(key), value, expiry, flags); return true; })));

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteWriteAsync(_writeDatabases.Select(db => (Func<Task<bool>>)(() => { db.KeyRestore(CreateAppKey(key), value, expiry, flags); return Task.FromResult(true); })));

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyTimeToLive(CreateAppKey(key), flags));

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyTimeToLiveAsync(CreateAppKey(key), flags));

        public bool KeyTouch(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyTouch(key, flags));

        public Task<bool> KeyTouchAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyTouchAsync(key, flags));

        public long KeyTouch(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyTouch(keys, flags));

        public Task<long> KeyTouchAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyTouchAsync(keys, flags));

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.KeyType(CreateAppKey(key), flags));

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.KeyTypeAsync(CreateAppKey(key), flags));

        public EndPoint IdentifyEndpoint(RedisKey key = default, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.IdentifyEndpoint(CreateAppKey(key), flags));

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.IdentifyEndpointAsync(CreateAppKey(key), flags));
    }
}
