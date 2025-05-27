using Azure.Core;
using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class DefaultAzureCredentialProvider : IDefaultAzureCredentialProvider
    {
        public TokenCredential GetDefaultAzureCredential(string userManagedIdentity)
        {
            return new ChainedTokenCredential(
            new VisualStudioCredential(),
            new AzureCliCredential(),
            new AzurePowerShellCredential());
        }
    }
}
