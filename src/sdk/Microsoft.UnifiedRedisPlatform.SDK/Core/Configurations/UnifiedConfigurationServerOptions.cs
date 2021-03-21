using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Constants;
using Microsoft.UnifiedRedisPlatform.Core.Exceptions;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public class UnifiedConfigurationServerOptions : UnifiedConfigurationOptions
    {
        public UnifiedConfigurationServerOptions() { }

        
        public string AppSecret { get; set; }

        private string _serviceEndpoint;
        public string ServiceEndpoint
        {
            get => !string.IsNullOrWhiteSpace(_serviceEndpoint) && Uri.IsWellFormedUriString(_serviceEndpoint, UriKind.Absolute) ? _serviceEndpoint : Constant.OperationApi.DefaultUrl;
            set => _serviceEndpoint = value;
        }

        public override object Clone()
        {
            return new UnifiedConfigurationServerOptions()
            {
                AppName = this.AppName,
                AppSecret = this.AppSecret,
                WritePolicy = this.WritePolicy,
                BaseConfigurationOptions = this.BaseConfigurationOptions.Clone(),
                ClusterName = this.ClusterName,
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

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(ClusterName))
                throw new InvalidConfigurationException(nameof(ClusterName), new ArgumentNullException(nameof(ClusterName)));
        }
    }
}
