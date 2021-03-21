namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
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
}
