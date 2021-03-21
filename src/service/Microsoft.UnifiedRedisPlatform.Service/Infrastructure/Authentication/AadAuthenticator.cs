using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedPlatform.Service.Authentication
{
    public class AadAuthenticator: IAuthenticator
    {
        private readonly string _authority;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AadAuthenticator(string authority, string clientId, string clientSecret)
        {
            _authority = authority;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<string> GenerateToken(string resourceId, Dictionary<string, string> additionalClaims)
        {
            var authContext = new AuthenticationContext(_authority);
            var authResult = await authContext.AcquireTokenAsync(resourceId, new ClientCredential(_clientId, _clientSecret));
            return authResult.AccessToken;
        }
    }
}
