using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public class UnifiedConfigurationLocalOptions: UnifiedConfigurationOptions
    {
        public override object Clone()
        {
            return new UnifiedConfigurationLocalOptions()
            {
                AppName = this.AppName,
                ClusterName = this.ClusterName,
                WritePolicy = this.WritePolicy,
                BaseConfigurationOptions = this.BaseConfigurationOptions.Clone(),
                ConnectionRetryProtocol = (RetryProtocol)this.ConnectionRetryProtocol.Clone(),
                DiagnosticSettings = (LogConfiguration)this.DiagnosticSettings.Clone(),
                KeyPrefix = this.KeyPrefix,
                Logger = this.Logger,
                OperationsRetryProtocol = (RetryProtocol)this.OperationsRetryProtocol.Clone(),
                SecondaryConfigurationsOptions = this.SecondaryConfigurationsOptions.Any() ?
                    this.SecondaryConfigurationsOptions.Select(options => options.Clone()).ToList()
                    : new List<ConfigurationOptions>()
            };
        }
    }
}
