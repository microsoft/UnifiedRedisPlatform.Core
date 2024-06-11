using Azure;
using Azure.Data.Tables;
using System;

namespace Microsoft.UnifiedPlatform.Service.Common.Storage
{
    public class ConfigurationEntity: ITableEntity
    {
        public bool IsBlob { get; set; }
        public string Value { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
