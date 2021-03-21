using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.UnifiedPlatform.Service.Common.Storage
{
    public class ConfigurationEntity: TableEntity
    {
        public bool IsBlob { get; set; }
        public string Value { get; set; }
    }
}
