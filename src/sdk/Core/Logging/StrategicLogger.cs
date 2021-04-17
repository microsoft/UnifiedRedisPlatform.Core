using System;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Extensions;
using static Microsoft.UnifiedRedisPlatform.Core.Constants.Constant;

namespace Microsoft.UnifiedRedisPlatform.Core.Logging
{
    internal class StrategicLogger : ILogger
    {
        //private readonly string _defaultLoggingStrategy;
        private readonly UnifiedConfigurationOptions _configuration;
        
        private readonly List<ILogger> _defaultLoggers;
        private readonly List<ILogger> _errorLoggers;

        public StrategicLogger(ILogger clientLogger, CacheLogger cacheLogger, ServiceAggregatedLogger aggregatedLogger, ServiceLogger serviceLogger, UnifiedConfigurationOptions configuration)
        {
            _configuration = configuration;

            _defaultLoggers = new List<ILogger>();
            _errorLoggers = new List<ILogger>();

            if (_configuration.DiagnosticSettings.Enabled)
            {
                if (clientLogger != null)
                {
                    _defaultLoggers.Add(clientLogger);
                    _errorLoggers.Add(clientLogger);
                }

                var defaultLoggingStrategy = configuration.DiagnosticSettings.LoggingStrategy;

                if (defaultLoggingStrategy == LoggingConstants.ImmediateLogging)
                    _defaultLoggers.Add(serviceLogger);
                else if (defaultLoggingStrategy == LoggingConstants.AggregateLogging)
                    _defaultLoggers.Add(aggregatedLogger);
                else
                    _defaultLoggers.Add(cacheLogger);

                _errorLoggers.Add(serviceLogger);
            }
        }


        public void Flush()
        {
            try
            {
                foreach (var logger in _defaultLoggers)
                {
                    logger.Flush();
                }
                foreach (var logger in _errorLoggers)
                {
                    logger.Flush();
                }
            }
            catch (Exception)
            {
                // Do nothing when logging fails
            }
        }

        public void LogEvent(string eventName, double timeTaken = 0, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            try
            {
                properties = AddDefaultProperties(properties);
                var loggers = _defaultLoggers;
                if (properties != null && properties.ContainsKey("Error"))
                {
                    loggers = _errorLoggers;
                }
                foreach (var logger in loggers)
                {
                    logger.LogEvent(eventName, timeTaken, properties, metrics);
                }
            }
            catch (Exception)
            {
                // Do nothing when logging fails
            }
        }

        public void LogException(Exception exception, IDictionary<string, string> properties = null)
        {
            try
            {
                properties = AddDefaultProperties(properties);
                foreach (var logger in _errorLoggers)
                {
                    logger.LogException(exception, properties);
                }
            }
            catch (Exception)
            {
                // Do nothing when logging fails
            }
        }

        public void LogMetric(string metricName, double duration, IDictionary<string, string> properties = null, IDictionary<string, string> metrics = null)
        {
            try
            {
                properties = AddDefaultProperties(properties);
                foreach (var logger in _defaultLoggers)
                {
                    logger.LogMetric(metricName, duration, properties, metrics);
                }
            }
            catch (Exception)
            {
                // Do nothing when logging fails
            }
        }

        private IDictionary<string, string> AddDefaultProperties(IDictionary<string, string> properties)
        {
            if (properties == null)
                properties = new Dictionary<string, string>();

            properties.AddOrUpdate("Cluster", _configuration.ClusterName);
            properties.AddOrUpdate("App", _configuration.AppName);
            return properties;
        }
    }
}
