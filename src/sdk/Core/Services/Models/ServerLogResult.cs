using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{
    internal class ServerLogResult
    {
        public int LogsReceived { get; set; }
        public int LogsFailed { get; set; }
        public List<GenericLog> FailedLogs { get; set; }
    }
}
