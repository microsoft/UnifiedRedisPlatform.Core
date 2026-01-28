using Azure.Core;
using Azure.Identity;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.UnifiedPlatform.Service.Common.Helpers
{
    /// <summary>
    /// Credential provider for production/Release builds only.
    /// Uses ManagedIdentityCredential with user-assigned managed identity.
    /// This provider is only registered in Release builds.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserAssignedIdentityCredentialProvider : IDefaultAzureCredentialProvider
    {
        public TokenCredential GetDefaultAzureCredential(string userAssignedClientId = "")
        {
            // Use provided value, or fall back to AZURE_CLIENT_ID environment variable
            var clientId = !string.IsNullOrEmpty(userAssignedClientId) 
                ? userAssignedClientId 
                : Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            
            if (string.IsNullOrEmpty(clientId))
            {
                // Fall back to system-assigned managed identity
                return new ManagedIdentityCredential();
            }
            return new ManagedIdentityCredential(
                ManagedIdentityId.FromUserAssignedClientId(clientId));
        }
    }
}
