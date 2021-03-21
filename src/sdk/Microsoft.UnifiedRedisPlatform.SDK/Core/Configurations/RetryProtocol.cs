using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public class RetryProtocol : IReconnectRetryPolicy, ICloneable
    {
        public int MaxRetryCount { get; set; }
        public int MinBackoffIntervalInMs { get; set; }
        public int MaxBackoffIntervalInMs { get; set; }
        public int TimeoutInMs { get; set; }
        public bool HardTimeoutEnabled { get; set; }

        private readonly Random _randomGenerator = new Random();

        public static RetryProtocol GetDefaultConnectionProtocol()
        {
            return new RetryProtocol()
            {
                MaxRetryCount = 50,
                MinBackoffIntervalInMs = 5000,
                MaxBackoffIntervalInMs = 15000,
                TimeoutInMs = 15000,
                HardTimeoutEnabled = false
            };
        }

        public static RetryProtocol GetDefaultOperationProtocol()
        {
            return new RetryProtocol()
            {
                MaxRetryCount = 50,
                MinBackoffIntervalInMs = 2000,
                MaxBackoffIntervalInMs = 15000,
                TimeoutInMs = 3000,
                HardTimeoutEnabled = false
            };
        }

        public bool ShouldRetry(long currentRetryCount, int timeElapsedMillisecondsSinceLastRetry)
        {
            var waitPeriod = _randomGenerator.Next(MinBackoffIntervalInMs, MaxBackoffIntervalInMs);
            return currentRetryCount < MaxRetryCount
                && timeElapsedMillisecondsSinceLastRetry >= waitPeriod;
        }

        public object Clone()
        {
            var retryProcolStr = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<RetryProtocol>(retryProcolStr);
        }
    }
}
