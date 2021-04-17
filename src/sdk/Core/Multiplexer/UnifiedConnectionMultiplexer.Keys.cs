using System;
using System.Linq;
using System.Diagnostics;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Constants;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public sealed partial class UnifiedConnectionMultiplexer
    {
        #region Key Operations
        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public async Task<List<RedisKey>> GetKeysAsync(string pattern = "")
        {
            return await Task.Run(() => GetKeys(pattern));
        }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public List<RedisKey> GetKeys(string pattern = "")
        {
            return GetKeys(_baseConnectionMux, pattern);
        }

        private async Task<List<RedisKey>> GetKeysAsync(IConnectionMultiplexer mux, string pattern = "")
        {
            return await Task.Run(() => GetKeys(mux, pattern));
        }

        private List<RedisKey> GetKeys(IConnectionMultiplexer mux, string pattern = "")
        {   
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var endpoints = mux.GetEndPoints();
            if (endpoints == null || !endpoints.Any())
                return new List<RedisKey>();

            foreach (var endpoint in endpoints)
            {
                var server = mux.GetServer(endpoint);
                if (server.IsSlave || server.IsReplica)
                    continue;

                var matchingPattern = _unifiedConfigurations.KeyPrefix + ":" + pattern +
                    (string.IsNullOrWhiteSpace(pattern) && pattern.EndsWith("*") ? string.Empty : "*");
                var keys = server.Keys(pattern: matchingPattern, pageSize: 1000);
                if (keys == null || !keys.Any())
                    return new List<RedisKey>();

                stopwatch.Stop();
                _logger.LogEvent("Operation:Success:GetKeys",
                    stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string>() { { "Patten", pattern }, { "KeysCount", keys.Count().ToString() } });

                return keys.ToList();
            }

            return new List<RedisKey>();
        }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public List<RedisKey> Flush(string pattern = "", CommandFlags flags = CommandFlags.None)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var keys = GetKeys(pattern);
            if (keys == null || !keys.Any())
                return null;

            keys = keys.Where(key => !key.ToString().Contains(_unifiedConfigurations.DiagnosticSettings.LogKey)).ToList();

            var database = _baseConnectionMux.GetDatabase();
            database.KeyDelete(keys.ToArray(), flags);

            if (!string.IsNullOrWhiteSpace(_unifiedConfigurations.WritePolicy) &&
                (_unifiedConfigurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.WriteThrough.ToLowerInvariant())
                    || _unifiedConfigurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.DeleteThrough.ToLowerInvariant())
            {
                FlushSecondary();
            }

            stopwatch.Stop();
            _logger.LogEvent("Operation:Success:Flush",
                    stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string>() { { "Patten", pattern } });

            return keys;
        }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public async Task<List<RedisKey>> FlushAsync(string pattern = "", CommandFlags flags = CommandFlags.None)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var keys = await GetKeysAsync(pattern);
            if (keys == null || !keys.Any())
                return null;
            var database = _baseConnectionMux.GetDatabase();
            await database.KeyDeleteAsync(keys.ToArray(), flags);
            if (!string.IsNullOrWhiteSpace(_unifiedConfigurations.WritePolicy) &&
                (_unifiedConfigurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.WriteThrough.ToLowerInvariant())
                    || _unifiedConfigurations.WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.DeleteThrough.ToLowerInvariant())
            {
                await FlushSecondaryAsync();
            }

            stopwatch.Stop();
            _logger.LogEvent("Operation:Success:FlushAsync",
                    stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string>() { { "Patten", pattern } });

            return keys;
        }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public void FlushSecondary(string pattern = "", CommandFlags flags = CommandFlags.None)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_secondaryConnectionMuxs != null && _secondaryConnectionMuxs.Any())
            {
                foreach (var secondaryConnectionMux in _secondaryConnectionMuxs)
                {
                    var keys = GetKeys(secondaryConnectionMux, pattern);
                    if (keys == null || !keys.Any())
                        return;
                    keys = keys.Where(key => !key.ToString().Contains(_unifiedConfigurations.DiagnosticSettings.LogKey)).ToList();
                    var database = secondaryConnectionMux.GetDatabase();
                    database.KeyDelete(keys.ToArray(), flags);
                }
            }
            stopwatch.Stop();
            _logger.LogEvent("Operation:Success:FlushSecondary",
                    stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string>() { { "Patten", pattern } });
        }

        [Obsolete("May cause performance issues. Please avoid using this method.")]
        public async Task FlushSecondaryAsync(string pattern = "", CommandFlags flags = CommandFlags.None)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_secondaryConnectionMuxs != null && _secondaryConnectionMuxs.Any())
            {
                var flushTasks = new List<Task>();
                foreach (var secondaryConnectionMux in _secondaryConnectionMuxs)
                {
                    var task = Task.Run(async () =>
                    {
                        var keys = await GetKeysAsync(secondaryConnectionMux, pattern);
                        if (keys == null || !keys.Any())
                            return;
                        keys = keys.Where(key => !key.ToString().Contains(_unifiedConfigurations.DiagnosticSettings.LogKey)).ToList();
                        var database = secondaryConnectionMux.GetDatabase();
                        await database.KeyDeleteAsync(keys.ToArray(), flags);
                    });
                    flushTasks.Add(task);
                }
                await Task.WhenAll(flushTasks);
            }
            stopwatch.Stop();
            _logger.LogEvent("Operation:Success:FlushSecondary",
                    stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string>() { { "Patten", pattern } });
        }
        #endregion Key Operations
    }
}
