using System;
using System.Linq;
using CQRS.Mediatr.Lite;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Application.Commands.Handlers
{
    public class LogClientInfoCommandHandler : CommandHandler<LogClientInfoCommand, LogResult>
    {
        private readonly ILogger _logger;

        public LogClientInfoCommandHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<LogResult> ProcessRequest(LogClientInfoCommand request)
        {
            var logs = request.Logs;
            var failedLogs = new List<Log>();
            
            var exceptionLogs = logs.Where(log => log.LogType == "Exception");
            var eventLogs = logs.Where(log => log.LogType == "Event");
            var metricLogs = logs.Where(log => log.LogType == "Metric");

            var exceptionLoggingTask = LogExceptions(request, exceptionLogs);
            var eventLoggingTask = LogEvents(request, eventLogs);
            var metricLoggingTask = LogMetrics(request, metricLogs);

            await Task.WhenAll(exceptionLoggingTask, eventLoggingTask, metricLoggingTask);
            
            failedLogs.AddRange(exceptionLoggingTask.Result);
            failedLogs.AddRange(eventLoggingTask.Result);
            failedLogs.AddRange(metricLoggingTask.Result);

            return new LogResult(logs.Count, failedLogs);
        }

        private async Task<List<Log>> LogExceptions(LogClientInfoCommand request, IEnumerable<Log> exceptionLogs)
        {
            if (exceptionLogs == null || !exceptionLogs.Any())
                return new List<Log>();
            var failedLogs = new ConcurrentBag<Log>();

            var logTasks = new List<Task>();

            foreach (var exceptionLog in exceptionLogs)
            {
                logTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        var exceptionContext = new ExceptionContext()
                        {
                            Exception = new Exception(exceptionLog.Message),
                            TraceLevel = TraceLevel.Error,
                            UserId = request.App,
                            Source = $"Log.API.{request.Cluster}.{request.App}"
                        };
                        exceptionContext.AddProperties(exceptionLog.Properties != null && exceptionLog.Properties.Any() ? exceptionLog.Properties : new Dictionary<string, string>());
                        exceptionContext.AddProperty("ActualTimestamp", exceptionLog.Time.ToString());
                        _logger.Log(exceptionContext);
                    }
                    catch (Exception exception)
                    {
                        exceptionLog.Error = exception.ToString();
                        failedLogs.Add(exceptionLog);
                    }
                }));
            }

            await Task.WhenAll(logTasks);
            return failedLogs.ToList();
        }

        private async Task<List<Log>> LogEvents(LogClientInfoCommand request, IEnumerable<Log> eventLogs)
        {
            if (eventLogs == null || !eventLogs.Any())
                return new List<Log>();
            var failedLogs = new ConcurrentBag<Log>();

            var logTasks = new List<Task>();

            foreach (var eventLog in eventLogs)
            {
                logTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        var eventContext = new EventContext()
                        {
                            EventName = $"{request.Cluster}:{request.App}:{eventLog.Message}",
                            Source = $"Log.API.{request.Cluster}.{request.App}",
                            UserId = request.App
                        };
                        eventContext.AddProperties(eventLog.Properties != null && eventLog.Properties.Any() ? eventLog.Properties : new Dictionary<string, string>());
                        eventContext.AddProperty("ActualTimestamp", eventLog.Time.ToString());
                        eventContext.AddProperty("Duration", eventLog.Duration > 0.0 ? eventLog.Duration.ToString() : "N/A");
                        _logger.Log(eventContext);
                    }
                    catch (Exception exception)
                    {
                        eventLog.Error = exception.ToString();
                        failedLogs.Add(eventLog);
                    }
                }));
            }

            await Task.WhenAll(logTasks);
            return failedLogs.ToList();
        }

        private async Task<List<Log>> LogMetrics(LogClientInfoCommand request, IEnumerable<Log> metricLogs)
        {
            if (metricLogs == null || !metricLogs.Any())
                return new List<Log>();
            var failedLogs = new ConcurrentBag<Log>();

            var logTasks = new List<Task>();

            foreach (var metricLog in metricLogs)
            {
                logTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        var metricContext = new MetricContext()
                        {
                            MetricName = $"{request.Cluster}:{request.App}:{metricLog.Message}",
                            Value = metricLog.Duration,
                            Source = $"Log.API.{request.Cluster}.{request.App}",
                            UserId = request.App
                        };
                        metricContext.AddProperties(metricLog.Properties != null && metricLog.Properties.Any() ? metricLog.Properties : new Dictionary<string, string>());
                        metricContext.AddProperty("ActualTimestamp", metricLog.Time.ToString());
                        _logger.Log(metricContext);
                    }
                    catch (Exception exception)
                    {
                        metricLog.Error = exception.ToString();
                        failedLogs.Add(metricLog);
                    }
                }));
            }

            await Task.WhenAll(logTasks);
            return failedLogs.ToList();
        }
    }
}
