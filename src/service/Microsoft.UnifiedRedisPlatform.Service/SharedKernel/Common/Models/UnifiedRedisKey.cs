using Newtonsoft.Json;
using System;

namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class UnifiedRedisKey
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public DateTime CreatedOn { get; set; }

        public UnifiedRedisKey(string name, string value)
        {
            Name = name;
            Value = value;
            DataType = "default";
            CreatedOn = DateTime.MinValue;
        }

        public UnifiedRedisKey(string name)
            : this(name, null)
        { }
    }
}
