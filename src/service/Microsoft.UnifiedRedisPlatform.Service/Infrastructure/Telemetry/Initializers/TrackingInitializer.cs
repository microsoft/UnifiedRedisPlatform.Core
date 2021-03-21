using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Telemetry.Initializers
{
    public class TrackingInitializer : ITelemetryInitializer
    {
        const string CORRELATION_KEY = "XCV";
        const string TRANSACTION_KEY = "MessageId";

        private readonly AppMetadataConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;

        public TrackingInitializer(IHttpContextAccessor httpContextAccessor, AppMetadataConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public TrackingInitializer(AppMetadataConfiguration configuration)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _configuration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            AddCorrelationId(telemetry, Guid.NewGuid().ToString());
            AddTransactionId(telemetry, Guid.NewGuid().ToString());
            AddCallerDetails(telemetry);
        }

        private void AddCorrelationId(ITelemetry telemetry, string defaultValue)
        {
            var telemetryCorrelationId = telemetry.Context.GlobalProperties.ContainsKey(CORRELATION_KEY) ? telemetry.Context.GlobalProperties[CORRELATION_KEY] : string.Empty;
            var httpContextCorrelationId = GetTrackingIdFromHttpContext(_configuration.CorrelationIdHeaderKey);

            if (!string.IsNullOrWhiteSpace(telemetryCorrelationId))
            {
                if (!string.IsNullOrWhiteSpace(httpContextCorrelationId) && telemetryCorrelationId != httpContextCorrelationId)
                {
                    ((ISupportProperties)telemetry).Properties[CORRELATION_KEY] = httpContextCorrelationId;
                    ((ISupportProperties)telemetry).Properties[$"{CORRELATION_KEY}:Alternate"] = telemetryCorrelationId;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(httpContextCorrelationId))
                {
                    ((ISupportProperties)telemetry).Properties[CORRELATION_KEY] = httpContextCorrelationId;
                }
                else
                {
                    ((ISupportProperties)telemetry).Properties[CORRELATION_KEY] = defaultValue;
                }
            }
        }

        private void AddTransactionId(ITelemetry telemetry, string defaultValue)
        {
            var telemetryTransactionId = telemetry.Context.GlobalProperties.ContainsKey(TRANSACTION_KEY) ? telemetry.Context.GlobalProperties[TRANSACTION_KEY] : string.Empty;
            var httpContextTransactionId = GetTrackingIdFromHttpContext(_configuration.TransactionIdHeaderKey);

            if (!string.IsNullOrWhiteSpace(telemetryTransactionId))
            {
                if (!string.IsNullOrWhiteSpace(httpContextTransactionId) && telemetryTransactionId != httpContextTransactionId)
                {
                    ((ISupportProperties)telemetry).Properties[TRANSACTION_KEY] = httpContextTransactionId;
                    ((ISupportProperties)telemetry).Properties[$"{TRANSACTION_KEY}:Alternate"] = telemetryTransactionId;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(httpContextTransactionId))
                {
                    ((ISupportProperties)telemetry).Properties[TRANSACTION_KEY] = httpContextTransactionId;
                }
                else
                {
                    ((ISupportProperties)telemetry).Properties[TRANSACTION_KEY] = defaultValue;
                }
            }
        }

        private void AddCallerDetails(ITelemetry telemetry)
        {
            var clusterKey = GetClusterFromContext();
            var appKey = GetAppFromContext();

            if (telemetry.Context.GlobalProperties.ContainsKey("App"))
                telemetry.Context.GlobalProperties["App"] = appKey;
            else
                telemetry.Context.GlobalProperties.Add("App", appKey);

            if (telemetry.Context.GlobalProperties.ContainsKey("Cluster"))
                telemetry.Context.GlobalProperties["Cluster"] = clusterKey;
            else
                telemetry.Context.GlobalProperties.Add("Cluster", clusterKey);
        }

        private string GetTrackingIdFromHttpContext(string contextHeaderKey)
        {
            if (_httpContextAccessor.HttpContext != null
                && _httpContextAccessor.HttpContext.Request != null
                && _httpContextAccessor.HttpContext.Request.Headers != null
                && _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(contextHeaderKey))
            {
                return _httpContextAccessor.HttpContext.Request.Headers[contextHeaderKey].FirstOrDefault();
            }
            return string.Empty;
        }

        private string GetClusterFromContext()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // Trying to re-initialize the HTTP Context accessot
                _httpContextAccessor = new HttpContextAccessor();
            }

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == _configuration.ClusterClaimsKey)?.Value;
            }
            return null;
        }

        private string GetAppFromContext()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // Trying to re-initialize the HTTP Context accessot
                _httpContextAccessor = new HttpContextAccessor();
            }

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == _configuration.AppClaimsKey)?.Value;
            }
            return null;
        }
    }
}
