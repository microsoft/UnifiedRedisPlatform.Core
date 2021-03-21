using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Client
{
    public interface IAppInsightsClientManager
    {
        IAppInsightsTelemetryClientWrapper CreateClient(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration);
    }
}
