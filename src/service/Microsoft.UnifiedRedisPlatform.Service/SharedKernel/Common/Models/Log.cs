using System;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class Log
    {
        public string LogType { get; set; }
        public string Message { get; set; }
        public double Duration { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public DateTimeOffset Time { get; set; }

        public string Error { get; set; }
    }
}
