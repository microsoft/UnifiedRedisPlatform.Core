using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    /// <summary>
    /// Configuration options for direct Redis connection without using URP service.
    /// Use this when you have the Redis connection details and want to connect directly.
    /// </summary>
    public class UnifiedConfigurationLocalOptions: UnifiedConfigurationOptions
    {
        // ManagedIdentityClientId and UseManagedIdentity are inherited from base class

        public override object Clone()
        {
            return new UnifiedConfigurationLocalOptions()
            {
                AppName = this.AppName,
                ClusterName = this.ClusterName,
                WritePolicy = this.WritePolicy,
                BaseConfigurationOptions = this.BaseConfigurationOptions?.Clone(),
                ConnectionRetryProtocol = this.ConnectionRetryProtocol != null ? (RetryProtocol)this.ConnectionRetryProtocol.Clone() : null,
                DiagnosticSettings = this.DiagnosticSettings != null ? (LogConfiguration)this.DiagnosticSettings.Clone() : null,
                KeyPrefix = this.KeyPrefix,
                Logger = this.Logger,
                OperationsRetryProtocol = this.OperationsRetryProtocol != null ? (RetryProtocol)this.OperationsRetryProtocol.Clone() : null,
                Region = this.Region,
                SecondaryConfigurationsOptions = this.SecondaryConfigurationsOptions.Any() ?
                    this.SecondaryConfigurationsOptions.Select(options => options.Clone()).ToList()
                    : new List<ConfigurationOptions>(),
                ManagedIdentityClientId = this.ManagedIdentityClientId
            };
        }
    }
}
