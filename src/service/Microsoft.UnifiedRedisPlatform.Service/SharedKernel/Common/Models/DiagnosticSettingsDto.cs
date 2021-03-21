namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class DiagnosticSettingsDto
    {
        public bool Enabled { get; set; }
        public string LogKey { get; set; }
        public string LoggingStrategy { get; set; }
        public int MaxRetryAttempt { get; set; }
        public int MinAggregatedItems { get; set; }
        public int MaxLogInternalInSeconds { get; set; }
    }
}
