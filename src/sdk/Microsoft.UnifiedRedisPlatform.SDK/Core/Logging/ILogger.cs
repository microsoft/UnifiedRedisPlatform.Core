using System;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    public interface ILogger
    {
        void LogException(Exception exception, IDictionary<string, string> properties = null);
        void LogEvent(string eventName, double timeTaken = 0.0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null);
        void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null);
        void Flush();
    }
}
