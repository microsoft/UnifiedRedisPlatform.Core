using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.Exceptions;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    /// <summary>
    /// Configuration options for connecting to URP via the service endpoint.
    /// Use this when you want the SDK to fetch Redis configuration from the URP service.
    /// </summary>
    public class UnifiedConfigurationServerOptions : UnifiedConfigurationOptions
    {
        public UnifiedConfigurationServerOptions() { }

        // AppSecret, ManagedIdentityClientId, and ServiceEndpoint are inherited from base class

        public override object Clone()
        {
            return new UnifiedConfigurationServerOptions()
            {
                AppName = this.AppName,
                AppSecret = this.AppSecret,
                ManagedIdentityClientId = this.ManagedIdentityClientId,
                WritePolicy = this.WritePolicy,
                BaseConfigurationOptions = this.BaseConfigurationOptions?.Clone(),
                ClusterName = this.ClusterName,
                ConnectionRetryProtocol = this.ConnectionRetryProtocol != null ? (RetryProtocol)this.ConnectionRetryProtocol.Clone() : null,
                DiagnosticSettings = this.DiagnosticSettings != null ? (LogConfiguration)this.DiagnosticSettings.Clone() : null,
                KeyPrefix = this.KeyPrefix,
                Logger = this.Logger,
                OperationsRetryProtocol = this.OperationsRetryProtocol != null ? (RetryProtocol)this.OperationsRetryProtocol.Clone() : null,
                Region = this.Region,
                SecondaryConfigurationsOptions = this.SecondaryConfigurationsOptions.Any() ?
                    this.SecondaryConfigurationsOptions.Select(options => options.Clone()).ToList()
                    : new List<ConfigurationOptions>()
            };
        }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(ClusterName))
                throw new InvalidConfigurationException(nameof(ClusterName), new ArgumentNullException(nameof(ClusterName)));
        }
    }
}
