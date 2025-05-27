using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Authentication
{
    public class AadAuthenticator : IAuthenticator
    {
        private readonly string _authority;
        private readonly string _clientId;
        private readonly ConcurrentDictionary<string, IConfidentialClientApplication> confidentialApps = new ConcurrentDictionary<string, IConfidentialClientApplication>();
        private readonly string _certificateThumprint;
        private readonly string _userAssignedClientId;
        public AadAuthenticator(string authority, string clientId, string certificateThumbprint,string userAssignedClientId)
        {
            _authority = authority;
            _clientId = clientId;
            _certificateThumprint = certificateThumbprint;
            _userAssignedClientId= userAssignedClientId;
        }

        public async Task<string> GenerateToken(string resourceId, Dictionary<string, string> additionalClaims)
        {
            IConfidentialClientApplication app = GetOrCreateConfidentialApp(_authority, _clientId);

            var authResult = await app.AcquireTokenForClient(new[] { $"{resourceId}/.default" }).ExecuteAsync();
            return authResult.AccessToken;
        }

        private IConfidentialClientApplication GetOrCreateConfidentialApp(string authority, string clientId)
        {
            string confidentialAppCacheKey = CreateConfidentialAppCacheKey(authority, clientId);
            if (confidentialApps.ContainsKey(confidentialAppCacheKey))
            {
                return confidentialApps[confidentialAppCacheKey];
            }
#if DEBUG
            var certificate = GetCertificate(_certificateThumprint);

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId)
                                                                            .WithAuthority(AzureCloudInstance.AzurePublic, "microsoft.onmicrosoft.com")
                                                                            .WithCertificate(certificate, true)
                                                                            .Build();
            confidentialApps.TryAdd(confidentialAppCacheKey, confidentialClientApplication);
            return confidentialClientApplication;
#else
            var managedIdentityId = ManagedIdentityId.FromUserAssignedClientId(_userAssignedClientId);
            var credential = new ManagedIdentityCredential(managedIdentityId);
            IConfidentialClientApplication clientApplicationWithMI = ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(new Uri(authority))
              .WithClientAssertion((AssertionRequestOptions options) =>
              {
                  var accessToken = credential.GetToken(new TokenRequestContext(new string[] { $"api://AzureADTokenExchange/.default" }), CancellationToken.None);
                  return Task.FromResult(accessToken.Token);
              }).Build();
            confidentialApps.TryAdd(confidentialAppCacheKey, clientApplicationWithMI);
            return clientApplicationWithMI;

#endif
        }

        private string CreateConfidentialAppCacheKey(string authority, string clientId)
        {
            return $"{authority}-{clientId}";
        }

        public X509Certificate2 GetCertificate(string certificateThumbprint)
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var cert = store.Certificates.OfType<X509Certificate2>()
                    .FirstOrDefault(x => x.Thumbprint?.ToUpper() == certificateThumbprint?.ToUpper());
                return cert;
            }
        }
    }
}
