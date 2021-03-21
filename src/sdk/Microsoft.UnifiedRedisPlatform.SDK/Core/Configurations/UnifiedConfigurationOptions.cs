using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Exceptions;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public abstract class UnifiedConfigurationOptions: ICloneable
    {
        public string ClusterName { get; set; }
        public string AppName { get; set; }
        public string KeyPrefix { get; set; }
        public string WritePolicy { get; set; }
        public string Region { get; set; }

        private RetryProtocol _operationsRetryProtocol;
        public RetryProtocol OperationsRetryProtocol
        {
            get => _operationsRetryProtocol ?? RetryProtocol.GetDefaultOperationProtocol();
            set => _operationsRetryProtocol = value;
        }
        public bool IsOperationsRetryProtocolSetToDefault => _operationsRetryProtocol == null;

        private RetryProtocol _connectionRetryProtocol;
        public RetryProtocol ConnectionRetryProtocol
        {
            get => _connectionRetryProtocol ?? RetryProtocol.GetDefaultConnectionProtocol();
            set => _connectionRetryProtocol = value;
        }
        public bool IsConnectionRetryProtocolSetToDefault => _connectionRetryProtocol == null;

        private LogConfiguration _diagnosticSettings;
        public LogConfiguration DiagnosticSettings
        {
            get => _diagnosticSettings ?? LogConfiguration.GetDefault();
            set => _diagnosticSettings = value;
        }
        public bool IsDiagnosticSettingsSetToDefault => _diagnosticSettings == null;

        public ILogger Logger { get; set; }

        public ConfigurationOptions BaseConfigurationOptions { get; set; }

        public List<ConfigurationOptions> SecondaryConfigurationsOptions { get; set; } = new List<ConfigurationOptions>();

        public abstract object Clone();

        public virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(AppName))
                throw new InvalidConfigurationException(nameof(AppName), new ArgumentNullException(nameof(AppName)));

            if (string.IsNullOrWhiteSpace(KeyPrefix))
                throw new InvalidConfigurationException(nameof(KeyPrefix), new ArgumentNullException(nameof(KeyPrefix)));

            if (BaseConfigurationOptions == null)
                throw new InvalidConfigurationException(nameof(BaseConfigurationOptions), new ArgumentNullException(nameof(BaseConfigurationOptions)));
        }
    }
}
