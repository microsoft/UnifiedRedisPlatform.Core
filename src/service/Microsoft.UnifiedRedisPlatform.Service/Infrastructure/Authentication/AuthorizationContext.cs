using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedPlatform.Service.Authentication
{
    public class AuthorizationContext : IAuthorizationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Checks if the logged in service princial in AAD Application
        /// </summary>
        /// <returns></returns>
        public bool IsAppContext()
        {
            return string.IsNullOrWhiteSpace(GetLoggedInUserPrincipalName());
        }

        /// <summary>
        /// Gets the User Principal Name of the logged in User
        /// </summary>
        /// <returns>User Principal Name</returns>
        public string GetLoggedInUserPrincipalName()
        {
            var upn = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type.Contains("upn"))?.Value;
            if (string.IsNullOrWhiteSpace(upn))
                upn = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            return upn;
        }

        /// <summary>
        /// Gets the AAD App ID of the logged in Application
        /// </summary>
        /// <returns>AAD Application ID</returns>
        public string GetLoggedInAppId()
        {
            var appId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type.Contains("appid"))?.Value;
            return appId;
        }
    }
}
