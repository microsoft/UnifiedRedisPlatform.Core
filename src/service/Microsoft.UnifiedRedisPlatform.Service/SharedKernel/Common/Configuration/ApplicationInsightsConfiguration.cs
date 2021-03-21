namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public class ApplicationInsightsConfiguration : BaseConfiguration
    {
        public override string Name => "Application Insights Configuration";

        public string InstrumentationKey { get; set; }
        public TraceLevel LogLevel { get; set; }
    }
}
