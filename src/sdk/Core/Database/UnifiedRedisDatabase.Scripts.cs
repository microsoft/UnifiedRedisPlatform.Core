using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Attributes;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        [Obsolete("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        public RedisResult Execute(string command, params object[] args) =>
            throw new NotImplementedException();

        [Obsolete("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        public RedisResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        public Task<RedisResult> ExecuteAsync(string command, params object[] args) =>
            throw new NotImplementedException();

        [Obsolete("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("Commands can contain key operations which may have been changed by the Unfied Platform. For script execution best practices please visit ")]
        public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("Script can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("Script can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();

        [Obsolete("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        [Risk("LuaScript can contain key operations which has been changed by the Unfied Platform. For script execution best practices please visit ")]
        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None) =>
            throw new NotImplementedException();
    }
}
