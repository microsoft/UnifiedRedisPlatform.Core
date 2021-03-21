using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Initializers
{
    public class ClientSideErrorInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Overrides the Success property for client side error
        /// </summary>
        /// <param name="telemetry" cref="ITelemetry">Application insight base telemetry type</param>
        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is RequestTelemetry requestTrace))
                return;

            if (!int.TryParse(requestTrace.ResponseCode, out int responseCode))
                return;

            if (responseCode >= 400 && responseCode < 500)
            {
                requestTrace.Success = true;
                requestTrace.Properties["Overridden400s"] = "true"; //Allow AI to filter property
            }
        }
    }
}
