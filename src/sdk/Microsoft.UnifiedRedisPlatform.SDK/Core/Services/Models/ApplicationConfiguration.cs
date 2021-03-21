using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.Logging;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{
    internal class ApplicationConfiguration
    {
        public string AppName { get; set; }
        public string SupportContact { get; set; }
        public string RedisCachePrefix { get; set; }
        public string WritePolicy { get; set; }
        public ConnectionPreference ConnectionPreference { get; set; }
        public LogConfiguration DiagnosticSettings { get; set; }

        public bool AreSecondaryConnectionsPresent => 
            !string.IsNullOrWhiteSpace(WritePolicy) &&
            (WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.WriteThrough.ToLowerInvariant()
                || WritePolicy.ToLowerInvariant() == Constant.WritePolicyConstants.DeleteThrough.ToLowerInvariant());

        public void AddDefaultValues()
        {
            if (string.IsNullOrWhiteSpace(WritePolicy))
                WritePolicy = Constant.WritePolicyConstants.NotManaged;

            if (ConnectionPreference == null)
                ConnectionPreference = new ConnectionPreference();

            if (DiagnosticSettings == null)
                DiagnosticSettings = LogConfiguration.GetDefault();

            ConnectionPreference.AddDefaultValues();
        }
    }

    public class ConnectionPreference
    {
        public RetryProtocol ConnectionRetryProtocol { get; set; }
        public RetryProtocol OperationalRetryProtocol { get; set; }
        public RetryProtocol SecondaryOperationalRetryProtocol { get; set; }
        public bool VerboseLoggingEnabled { get; set; }

        public void AddDefaultValues()
        {
            if (ConnectionRetryProtocol == null)
                ConnectionRetryProtocol = RetryProtocol.GetDefaultConnectionProtocol();
            if (OperationalRetryProtocol == null)
                OperationalRetryProtocol = RetryProtocol.GetDefaultOperationProtocol();
            if (SecondaryOperationalRetryProtocol == null)
                SecondaryOperationalRetryProtocol = RetryProtocol.GetDefaultOperationProtocol();
        }
    }
}
