using Azure.Core;
using Azure.Identity;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    public interface IDefaultAzureCredentialProvider
    {
        TokenCredential GetDefaultAzureCredential(string userAssignedClientId="");
    }
}
