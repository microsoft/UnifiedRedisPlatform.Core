using Msit.Telemetry.Extensions.AI;
using Microsoft.ApplicationInsights.DataContracts;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Client
{
    public interface IAppInsightsTelemetryClientWrapper
    {
        void TrackTrace(TraceTelemetry traceTelemetry);
        void TrackException(ExceptionTelemetry exceptionTelemetry);
        void TrackEvent(EventTelemetry eventTelemetry);
        void TrackMetric(MetricTelemetry metricTelemetry);
        void TrackBusinessProcessEvent(BusinessProcessEvent businessProcessEvent);
    }
}
