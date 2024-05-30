using Azure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class DefaultAzureCredentialProvider : IDefaultAzureCredentialProvider
    {
        public DefaultAzureCredential GetDefaultAzureCredential()
        {
            return new DefaultAzureCredential();
        }
    }
}
