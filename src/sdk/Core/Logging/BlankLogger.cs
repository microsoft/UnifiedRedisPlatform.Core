using System;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    public class BlankLogger : ILogger
    {
        public static BlankLogger Get()
        {
            return new BlankLogger();
        }

        public void Flush()
        {   
        }

        public void LogEvent(string eventName, double timeTaken = 0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {   
        }

        public void LogException(Exception exception, IDictionary<string, string> properties = null)
        {   
        }

        public void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {   
        }
    }
}
