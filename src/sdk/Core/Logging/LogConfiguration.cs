using System;
using Newtonsoft.Json;
using static Microsoft.UnifiedRedisPlatform.Core.Constants.Constant.LoggingConstants;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    public class LogConfiguration: ICloneable
    {
        public bool Enabled { get; set; }
        public string LoggingStrategy { get; set; }
        public int MinAggregatedItems { get; set; }
        public int MaxLogInternalInSeconds { get; set; }
        public int MaxRetryAttempt { get; set; }
        public string LogKey { get; set; }

        public LogConfiguration(bool enabled = true, string loggingStrategy = CacheAsideLogging, string logKey = DefaultLogKey, int maxRetryAttempty = 5, int minAggregatedItems = 10, int maxLogIntervalInSeconds = 300)
        {
            Enabled = enabled;
            LoggingStrategy = loggingStrategy;
            LogKey = logKey;
            MaxRetryAttempt = maxRetryAttempty;
            MinAggregatedItems = minAggregatedItems;
            MaxLogInternalInSeconds = maxLogIntervalInSeconds;
        }

        public static LogConfiguration GetDefault()
        {
            return new LogConfiguration();
        }

        public object Clone()
        {
            var configurationStr = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<LogConfiguration>(configurationStr);
        }
    }
}
