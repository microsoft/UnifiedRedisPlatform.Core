using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    public class CacheLogger : ILogger
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _logKey;

        public CacheLogger(IDatabase redisDatabase, string logKey)
        {
            _redisDatabase = redisDatabase;
            _logKey = logKey;
        }

        public void LogEvent(string eventName, double timeTaken = 0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {   
            var genericLog = GenericLogProvider.CreateFromEvent(eventName, timeTaken, properties, metrics);
            _redisDatabase.ListRightPush(_logKey, genericLog.ToString(), flags: CommandFlags.FireAndForget);
        }

        public void LogException(Exception exception, IDictionary<string, string> properties = null)
        {
            var genericLog = GenericLogProvider.CreateFromException(exception, properties);
            _redisDatabase.ListRightPush(_logKey, genericLog.ToString(), flags: CommandFlags.FireAndForget);
        }

        public void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            var genericLog = GenericLogProvider.CreateFromMetric(metricName, duration, properties, metrics);
            _redisDatabase.ListRightPush(_logKey, genericLog.ToString(), flags: CommandFlags.FireAndForget);
        }

        public void Flush()
        { }
    }
}
