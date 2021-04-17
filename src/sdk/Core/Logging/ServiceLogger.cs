using System;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Services;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    internal class ServiceLogger : ILogger
    {
        private readonly LogConfiguration _configuration;
        private readonly IUnifiedRedisPlatformServiceClient _client;

        public ServiceLogger(string serviceEndpoint, string clusterName, string appName, string appSecret, string location, LogConfiguration configuration)
            : this(configuration, new UnifiedRedisPlatformServiceClient(serviceEndpoint, clusterName, appName, appSecret, location))
        { }

        public ServiceLogger(LogConfiguration configuration, IUnifiedRedisPlatformServiceClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public void LogEvent(string eventName, double timeTaken = 0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            try
            {
                var genericLog = GenericLogProvider.CreateFromEvent(eventName, timeTaken, properties, metrics);
                _client.Log(new List<GenericLog>() { genericLog }, attempt: 1, maxAttempt: _configuration.MaxRetryAttempt).Wait();
            }
            catch (Exception) { }
        }

        public void LogException(Exception exception, IDictionary<string, string> properties = null)
        {
            try
            {
                var genericLog = GenericLogProvider.CreateFromException(exception, properties);
                _client.Log(new List<GenericLog>() { genericLog }, attempt: 1, maxAttempt: _configuration.MaxRetryAttempt).Wait();
            }
            catch (Exception) { }
        }

        public void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            try
            {
                var genericLog = GenericLogProvider.CreateFromMetric(metricName, duration, properties, metrics);
                _client.Log(new List<GenericLog>() { genericLog }, attempt: 1, maxAttempt: _configuration.MaxRetryAttempt).Wait();
            }
            catch (Exception) { }
        }

        public void Flush()
        { }
    }
}
