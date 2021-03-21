using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{   
    internal class GenericLog
    {
        public GenericLog()
        {
            Properties = new Dictionary<string, string>();
        }

        public string LogType { get; set; }
        public string Message { get; set; }
        public double Duration { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public DateTimeOffset Time { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    internal struct LogTypes
    {
        public const string Exception = "Exception";
        public const string Event = "Event";
        public const string Metric = "Metric";
    }
}
