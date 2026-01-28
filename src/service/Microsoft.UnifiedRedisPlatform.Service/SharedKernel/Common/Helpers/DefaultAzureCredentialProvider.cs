using Azure.Core;
using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    /// <summary>
    /// Credential provider for local development/debugging only.
    /// Uses VisualStudioCredential to authenticate via Visual Studio login.
    /// This provider is only registered in DEBUG builds.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultAzureCredentialProvider : IDefaultAzureCredentialProvider
    {
        public TokenCredential GetDefaultAzureCredential(string userManagedIdentity = "")
        {
            // For local development, use Visual Studio credentials
            return new VisualStudioCredential();
        }
    }
}
