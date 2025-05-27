using Azure.Core;
using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class UserAssignedIdentityCredentialProvider : IDefaultAzureCredentialProvider
    {
        public TokenCredential GetDefaultAzureCredential(string userAssignedClientId)
        {
            return new ManagedIdentityCredential(
            ManagedIdentityId.FromUserAssignedClientId(userAssignedClientId));
        }
    }
}
