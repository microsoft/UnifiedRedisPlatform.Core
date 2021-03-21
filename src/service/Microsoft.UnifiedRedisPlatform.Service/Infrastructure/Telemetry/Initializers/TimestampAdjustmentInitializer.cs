using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Initializers
{
    public class TimestampAdjustmentInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (!((ISupportProperties)telemetry).Properties.ContainsKey("ActualTimestamp"))
                return;

            var actualTimestampStr = ((ISupportProperties)telemetry).Properties["ActualTimestamp"];
            if (!DateTimeOffset.TryParse(actualTimestampStr, out DateTimeOffset actualTimestamp))
                return;

            telemetry.Timestamp = actualTimestamp;
            ((ISupportProperties)telemetry).Properties.Add("LoggedOn", DateTime.UtcNow.ToString());
        }
    }
}
