using System;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Logging;
using Microsoft.UnifiedRedisPlatform.Core.Exceptions;
using Microsoft.UnifiedRedisPlatform.Core.Constants;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    /// <summary>
    /// Configuration options for connecting to Unified Redis Platform.
    /// Can be instantiated directly for backward compatibility, or use derived classes.
    /// </summary>
    public class UnifiedConfigurationOptions: ICloneable
    {
        public string ClusterName { get; set; }
        public string AppName { get; set; }
        public string KeyPrefix { get; set; }
        public string WritePolicy { get; set; }
        public string Region { get; set; }

        /// <summary>
        /// Application secret for authentication with URP service.
        /// Not required when using Managed Identity.
        /// </summary>
        public virtual string AppSecret { get; set; }

        /// <summary>
        /// The client ID of the User-Assigned Managed Identity to use for Redis authentication.
        /// When set, the SDK will automatically configure Azure AD authentication.
        /// Leave null/empty to use access key authentication (backward compatible).
        /// </summary>
        public virtual string ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Returns true if Managed Identity should be used for authentication.
        /// </summary>
        public bool UseManagedIdentity => !string.IsNullOrWhiteSpace(ManagedIdentityClientId);

        private string _serviceEndpoint;
        /// <summary>
        /// The URP service endpoint URL for fetching configuration.
        /// Defaults to the production endpoint if not specified.
        /// </summary>
        public virtual string ServiceEndpoint
        {
            get => !string.IsNullOrWhiteSpace(_serviceEndpoint) && Uri.IsWellFormedUriString(_serviceEndpoint, UriKind.Absolute) ? _serviceEndpoint : Constant.OperationApi.DefaultUrl;
            set => _serviceEndpoint = value;
        }

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

        /// <summary>
        /// Creates a deep copy of this configuration.
        /// </summary>
        public virtual object Clone()
        {
            return new UnifiedConfigurationOptions()
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
