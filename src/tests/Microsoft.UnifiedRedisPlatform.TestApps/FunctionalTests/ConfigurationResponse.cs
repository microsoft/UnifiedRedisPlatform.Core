using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.FunctionalTests
{
    public class ConfigurationResponse
    {
        public string ClusterName { get; set; }
        public bool IsProductionCluster { get; set; }
        public string SupportContact { get; set; }
        public string RedisCachePrefix { get; set; }
        public string PrimaryRedisRegion { get; set; }
        public string RedisConnectionString { get; set; }
        public string[] SecondaryRedisConnectionStrings { get; set; }
        public List<ApplicationConfiguration> Applications { get; set; }
    }

    public class ApplicationConfiguration
    {
        public string AppName { get; set; }
        public string SupportContact { get; set; }
        public string RedisCachePrefix { get; set; }
        public string WritePolicy { get; set; }
        public ConnectionPreference ConnectionPreference { get; set; }
        public DiagnosticSettings DiagnosticSettings { get; set; }
    }

    public class ConnectionPreference
    {
        public RetryProtocol ConnectionRetryProtocol { get; set; }
        public RetryProtocol OperationalRetryProtocol { get; set; }
        public RetryProtocol SecondaryOperationalRetryProtocol { get; set; }
        public bool VerboseLoggingEnabled { get; set; }
    }

    public class RetryProtocol
    {
        public int MaxRetryCount { get; set; }
        public int MinBackoffIntervalInMs { get; set; }
        public int MaxBackoffIntervalInMs { get; set; }
        public int TimeoutInMs { get; set; }
        public bool HardTimeoutEnabled { get; set; }
    }

    public class DiagnosticSettings
    {
        public bool Enabled { get; set; }
        public string LogKey { get; set; }
        public string LoggingStrategy { get; set; }
        public int MaxRetryAttempt { get; set; }
        public int MinAggregatedItems { get; set; }
        public int MaxLogInternalInSeconds { get; set; }

    }
}
