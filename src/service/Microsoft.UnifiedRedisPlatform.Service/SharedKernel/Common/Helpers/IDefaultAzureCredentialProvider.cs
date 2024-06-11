using Azure.Identity;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    public interface IDefaultAzureCredentialProvider
    {
        DefaultAzureCredential GetDefaultAzureCredential();
    }
}
