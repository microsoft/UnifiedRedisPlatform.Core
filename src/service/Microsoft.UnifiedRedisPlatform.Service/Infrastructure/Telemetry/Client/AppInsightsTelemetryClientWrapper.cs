using Msit.Telemetry.Extensions.AI;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Client
{
    public class AppInsightsTelemetryClientWrapper : IAppInsightsTelemetryClientWrapper
    {
        private readonly TelemetryClient _client;

        public AppInsightsTelemetryClientWrapper(TelemetryClient client)
        {
            _client = client;
        }

        public void TrackEvent(EventTelemetry eventTelemetry)
        {
            _client.TrackEvent(eventTelemetry);
        }

        public void TrackException(ExceptionTelemetry exceptionTelemetry)
        {
            _client.TrackException(exceptionTelemetry);
        }

        public void TrackMetric(MetricTelemetry metricTelemetry)
        {
            _client.TrackMetric(metricTelemetry);
        }

        public void TrackTrace(TraceTelemetry traceTelemetry)
        {
            _client.TrackTrace(traceTelemetry);
        }

        public void TrackBusinessProcessEvent(BusinessProcessEvent businessProcessEvent)
        {
            _client.TrackBusinessProcessEvent(businessProcessEvent);
        }
    }
}
