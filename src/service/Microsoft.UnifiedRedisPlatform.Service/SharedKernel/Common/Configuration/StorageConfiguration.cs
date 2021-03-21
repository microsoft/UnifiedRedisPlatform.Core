using System;

namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public class StorageConfiguration: BaseConfiguration
    {
        public override string Name => "Storage Configuration";

        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public TimeSpan BackoffInternal { get; set; }
        public int MaxAttempt { get; set; }
        public string ConfigurationTableName { get; set; }
    }
}
