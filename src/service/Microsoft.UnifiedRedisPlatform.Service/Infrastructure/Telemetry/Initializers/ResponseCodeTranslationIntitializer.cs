using System.Net;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Initializers
{
    public class ResponseCodeTranslationIntitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Adds the status description along with HTTP status code
        /// </summary>
        /// <param name="telemetry" cref="ITelemetry">Application insight base telemetry type</param>
        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is RequestTelemetry requestTrace))
                return;

            if (!int.TryParse(requestTrace.ResponseCode, out int responseCode))
                return;

            var convertedHttpEnum = ConvertFromInt(responseCode);
            if (!convertedHttpEnum.HasValue)
                return;

            var httpStatusCode = convertedHttpEnum.Value;
            var httpStatusDescription = httpStatusCode.ToString();
            requestTrace.Properties.Add("Response Code Description", httpStatusDescription);
        }

        private HttpStatusCode? ConvertFromInt(int code)
        {
            if (typeof(HttpStatusCode).IsEnumDefined(code))
                return (HttpStatusCode)code;
            return null;
        }
    }
}
