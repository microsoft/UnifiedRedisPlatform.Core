using System;
using System.Linq;
using Microsoft.CQRS;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Application.Commands
{
    public class LogClientInfoCommand : Command<LogResult>
    {
        public override string DisplayName => "Log Client Data";

        private readonly string _commandId;
        public override string Id => _commandId;

        public string Cluster { get; set; }
        public string App { get; set; }
        public List<Log> Logs { get; set; }

        public LogClientInfoCommand(List<Log> logs, string cluster, string app)
        {
            _commandId = Guid.NewGuid().ToString();
            Logs = logs;
            Cluster = cluster;
            App = app;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;

            if (Logs == null || !Logs.Any())
            {
                ValidationErrorMessage = "No logs present";
                return false;
            }

            return true;
        }
    }

    public class LogResult : CommandResult
    {
        public int LogsReceived { get; set; }
        public int LogsFailed { get; set; }
        public List<Log> FailedLogs { get; set; }

        public LogResult(int logsReceived, List<Log> failedLogs)
            : base(failedLogs == null || !failedLogs.Any())
        {
            LogsReceived = logsReceived;
            LogsFailed = FailedLogs != null && FailedLogs.Any() ? FailedLogs.Count : 0;
            FailedLogs = failedLogs;
        }
    }
}
