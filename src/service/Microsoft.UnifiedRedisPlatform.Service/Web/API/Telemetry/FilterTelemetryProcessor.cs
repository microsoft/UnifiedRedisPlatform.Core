namespace Microsoft.UnifiedRedisPlatform.Service.API.Telemetry
{
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.DataContracts;
    using System;

    public class FilterTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor _next;

        public FilterTelemetryProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            // To filter out an item, return without calling the next processor.
            if (!OktoSend(item)) { return; }

            this._next.Process(item);
        }

        private static bool OktoSend(ITelemetry item)
        {
            var request = item as RequestTelemetry;

            if (request == null) { return true; }

            if (request != null)
            {
                //filter out telemetry if its specific to ping API
                if (!string.IsNullOrEmpty(request?.Context?.Operation?.Name) && request.Context.Operation.Name.Contains("Probe/Ping", StringComparison.OrdinalIgnoreCase)) return false;
            }

            return true;
        }
    }

}
