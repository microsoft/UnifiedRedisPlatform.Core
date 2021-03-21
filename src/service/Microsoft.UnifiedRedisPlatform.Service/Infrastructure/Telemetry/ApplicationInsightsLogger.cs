using System;
using System.Diagnostics;
using System.Globalization;
using Msit.Telemetry.Extensions.AI;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.UnifiedPlatform.Service.Common.Telemetry;
using Microsoft.UnifiedPlatform.Service.Telemetry.Client;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Telemetry
{
    public class ApplicationInsightsLogger : ILogger
    {
        private readonly IAppInsightsTelemetryClientWrapper _client;
        private readonly ApplicationInsightsConfiguration _configuration;

        public ApplicationInsightsLogger(ApplicationInsightsConfiguration applicationInsightsConfiguration, AppMetadataConfiguration appMetadataConfiguration)
        {
            _configuration = applicationInsightsConfiguration;
            var _clientManager = new AppInsightsClientManager();
            _client = _clientManager.CreateClient(applicationInsightsConfiguration, appMetadataConfiguration);
        }

        public ApplicationInsightsLogger(IAppInsightsTelemetryClientWrapper client, ApplicationInsightsConfiguration applicationInsightsConfiguration)
        {
            _configuration = applicationInsightsConfiguration;
            _client = client;
        }

        public void Log(string message, string correlationId, string transactionId, string source, string userId, string e2eTrackingId)
        {
            var messageContext = new MessageContext(message, TraceLevel.Verbose, correlationId, transactionId, source, userId, e2eTrackingId);
            Log(messageContext);
        }

        public void Log(MessageContext message)
        {
            try
            {
                if (!(ValidateLog(message)))
                    return;
                var traceTelemetry = new TraceTelemetry(message.Message, GetAppInsightsSevLevel(message.TraceLevel));
                if (message.Properties != null && message.Properties.Count > 0)
                {
                    foreach (var property in message.Properties)
                    {
                        traceTelemetry.Properties.Add(property);
                    }
                }

                _client.TrackTrace(traceTelemetry);
            }
            catch (Exception exception)
            {
                LogInternal(exception);
            }
        }

        public void Log(Exception exception, string correlationId, string transactionId, string source, string userId, string e2eTrackingId)
        {
            var exceptionContext = new ExceptionContext(exception, TraceLevel.Error, correlationId, transactionId, source, userId, e2eTrackingId);
            Log(exceptionContext);
        }

        public void Log(ExceptionContext exception)
        {
            try
            {
                if (!(ValidateLog(exception)))
                    return;
                var exceptionTelemetry = new ExceptionTelemetry(exception.Exception)
                {
                    SeverityLevel = GetAppInsightsSevLevel(exception.TraceLevel)
                };
                if (exception.Properties != null && exception.Properties.Count > 0)
                {
                    foreach (var property in exception.Properties)
                    {
                        exceptionTelemetry.Properties.Add(property);
                    }
                }

                _client.TrackException(exceptionTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }

        }

        public void Log(EventContext eventContext)
        {
            try
            {
                if (eventContext.IsBusinessEvent)
                {
                    LogBusinessEvent(eventContext);
                }

                if (!(ValidateLog(eventContext)))
                    return;
                var eventTelemetry = new EventTelemetry(eventContext.EventName);
                if (eventContext.Properties != null && eventContext.Properties.Count > 0)
                {
                    foreach (var property in eventContext.Properties)
                    {
                        eventTelemetry.Properties.Add(property);
                    }
                }
                if (eventContext.BusinessProperties != null && eventContext.BusinessProperties.Count > 0)
                {
                    foreach (var property in eventContext.BusinessProperties)
                    {
                        eventTelemetry.Properties.Add(property.Key, property.Value);
                    }
                }
                if (eventContext.Metrics != null && eventContext.Metrics.Count > 0)
                {
                    foreach (var metric in eventContext.Metrics)
                    {
                        eventTelemetry.Metrics.Add(metric);
                    }
                }

                _client.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        private void LogBusinessEvent(EventContext eventContext)
        {
            var businessEventData = new BusinessProcessEvent(eventContext.EventName, Msit.Telemetry.Extensions.ComponentType.WorkflowComponent)
            {
                Xcv = eventContext.CorrelationId,
                MessageId = eventContext.TransactionId,
                SenderId = eventContext.BusinessProperties.ContainsKey("SenderId") ? eventContext.BusinessProperties["SenderId"] : "N/A",
                ReceiverId = eventContext.BusinessProperties.ContainsKey("ReceiverId") ? eventContext.BusinessProperties["ReceiverId"] : "N/A",
                ActionUri = eventContext.BusinessProperties.ContainsKey("ActionUri") ? eventContext.BusinessProperties["ActionUri"] : "N/A",
                AppAction = eventContext.BusinessProperties.ContainsKey("AppAction") ? eventContext.BusinessProperties["AppAction"] : "N/A",
                BusinessProcessName = eventContext.BusinessProperties.ContainsKey("BusinessProcessName") ? eventContext.BusinessProperties["BusinessProcessName"] : "N/A",
                UserRoleName = eventContext.BusinessProperties.ContainsKey("UserRoleName") ? eventContext.BusinessProperties["UserRoleName"] : "N/A",
                TargetEntityKey = eventContext.BusinessProperties.ContainsKey("TargetEntityKey") ? eventContext.BusinessProperties["TargetEntityKey"] : "N/A",
                StartDateTime = DateTime.Parse(eventContext.BusinessProperties.ContainsKey("StartDateTime") ? eventContext.BusinessProperties["StartDateTime"] : new DateTime(1971, 1, 1).ToString(CultureInfo.InvariantCulture)),
                EndDateTime = DateTime.Parse(eventContext.BusinessProperties.ContainsKey("EndDateTime") ? eventContext.BusinessProperties["EndDateTime"] : new DateTime(1971, 1, 1).ToString(CultureInfo.InvariantCulture)),
                EventOccurrenceTime = DateTime.Parse(eventContext.BusinessProperties.ContainsKey("EventOccurrenceTime") ? eventContext.BusinessProperties["EventOccurrenceTime"] : new DateTime(1971, 1, 1).ToString(CultureInfo.InvariantCulture)),
                EventType = Msit.Telemetry.Extensions.ItEventType.BusinessProcessEvent
            };
            _client.TrackBusinessProcessEvent(businessEventData);
        }

        public void Log(MetricContext metric)
        {
            try
            {
                if (!(ValidateLog(metric)))
                    return;
                var metricTelemetry = new MetricTelemetry(metric.MetricName, metric.Value);
                if (metric.Properties != null && metric.Properties.Count > 0)
                {
                    foreach (var property in metric.Properties)
                    {
                        metricTelemetry.Properties.Add(property);
                    }
                }
                _client.TrackMetric(metricTelemetry);
            }
            catch (Exception ex)
            {
                LogInternal(ex);
            }
        }

        private void LogInternal(Exception exception)
        {
            //Try to log exception in App Insights
            try
            {
                _client.TrackException(new ExceptionTelemetry(exception));
            }
            catch (Exception) //Write message to trace listener
            {
                Debug.Write($"UNHANDLED EXCEPTION IN TELEMETRY: {exception.ToString()}");
                //Don't rethrow exception
            }
        }

        private SeverityLevel GetAppInsightsSevLevel(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Critical: return SeverityLevel.Critical;
                case TraceLevel.Error: return SeverityLevel.Error;
                case TraceLevel.Warning: return SeverityLevel.Warning;
                case TraceLevel.Verbose: return SeverityLevel.Verbose;
                default: return SeverityLevel.Information;
            }
        }

        private bool ValidateLog(LogContext context)
        {
            var currentTraceLevel = _configuration.LogLevel;
            if (context.TraceLevel < currentTraceLevel)
                return false;
            return true;
        }
    }
}
