using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.UnifiedRedisPlatform.Core.Services;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    internal class ServiceAggregatedLogger: ILogger
    {
        private readonly LogConfiguration _configuration;
        private readonly IUnifiedRedisPlatformServiceClient _client;

        private readonly static ConcurrentBag<GenericLog> UncomittedLogs = new ConcurrentBag<GenericLog>();
        private static DateTime LastLoggedOn = DateTime.UtcNow;
        private static readonly object lockObject = new object();

        public ServiceAggregatedLogger(string serviceEndpoint, string clusterName, string appName, string appSecret, string location, LogConfiguration configuration)
            :this(configuration, new UnifiedRedisPlatformServiceClient(serviceEndpoint, clusterName, appName, appSecret, location))
        { }

        public ServiceAggregatedLogger(LogConfiguration configuration, IUnifiedRedisPlatformServiceClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public void LogEvent(string eventName, double timeTaken = 0.0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            var genericLog = GenericLogProvider.CreateFromEvent(eventName, timeTaken, properties, metrics);
            UncomittedLogs.Add(genericLog);
            TryLog();
        }

        public void LogException(Exception exception, IDictionary<string, string> properties = null)
        {
            var genericLog = GenericLogProvider.CreateFromException(exception, properties);
            UncomittedLogs.Add(genericLog);
            TryLog();
        }

        public void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            var genericLog = GenericLogProvider.CreateFromMetric(metricName, duration, properties, metrics);
            UncomittedLogs.Add(genericLog);
            TryLog();
        }

        public void Flush()
        {
            SendLogsToServer();
        }

        private void TryLog()
        {
            var timeSinceLastLog = (DateTime.UtcNow - LastLoggedOn).TotalSeconds;
            if (timeSinceLastLog > _configuration.MaxLogInternalInSeconds)
                SendLogsToServer();
            else if (UncomittedLogs.Count >= _configuration.MinAggregatedItems)
                SendLogsToServer();
        }

        private void SendLogsToServer()
        {
            lock (lockObject)
            {
                if (UncomittedLogs.Count == 0)
                return;

                var logs = UncomittedLogs.ToList();
                _client.Log(logs, attempt: 1, maxAttempt: _configuration.MaxRetryAttempt).Wait();
                while (!UncomittedLogs.IsEmpty)
                    UncomittedLogs.TryTake(out GenericLog committedLog);

                LastLoggedOn = DateTime.UtcNow;
            }
        }
    }
}
