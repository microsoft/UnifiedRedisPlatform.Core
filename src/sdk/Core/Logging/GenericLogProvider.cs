using System;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Extensions;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    internal static class GenericLogProvider
    {
        internal static GenericLog CreateFromEvent(string eventName, double timeTaken = 0.0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null) 
        {
            var genericLog = new GenericLog()
            {
                LogType = LogTypes.Event,
                Message = eventName,
                Duration = timeTaken,
                Time = DateTimeOffset.UtcNow
            };

            if (properties != null)
                genericLog.Properties.AddRange(properties);

            if (metrics != null)
                genericLog.Properties.AddRange(metrics);

            return genericLog;
        }

        internal static GenericLog CreateFromException(Exception exception, IDictionary<string, string> properties = null)
        {
            var genericLog = new GenericLog()
            {
                LogType = LogTypes.Exception,
                Message = exception.ToString(),
                Time = DateTimeOffset.UtcNow
            };

            if (properties != null)
                genericLog.Properties.AddRange(properties);

            return genericLog;
        }

        internal static GenericLog CreateFromMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            var genericLog = new GenericLog()
            {
                LogType = LogTypes.Event,
                Message = metricName,
                Duration = duration,
                Time = DateTimeOffset.UtcNow
            };

            if (properties != null)
                genericLog.Properties.AddRange(properties);

            if (metrics != null)
                genericLog.Properties.AddRange(metrics);

            return genericLog;
        }
    }
}
