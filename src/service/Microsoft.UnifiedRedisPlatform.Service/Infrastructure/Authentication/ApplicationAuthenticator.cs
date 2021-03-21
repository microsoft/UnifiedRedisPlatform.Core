using System.Linq;
using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Graph;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedPlatform.Service.Authentication
{
    public class ApplicationAuthenticator: IApplicationAuthenticator
    {
        private readonly IGraphManager _graphManager;
        private readonly IAuthorizationContext _authenticationContext;
        private readonly IClusterConfigurationProvider _clusterConfigurationProvider;
        private readonly BaseConfigurationProvider _configurationProvider;

        public ApplicationAuthenticator(IGraphManager graphManager, IAuthorizationContext authenticationContext, IClusterConfigurationProvider clusterConfigurationProvider, BaseConfigurationProvider configurationProvider)
        {
            _graphManager = graphManager;
            _authenticationContext = authenticationContext;
            _clusterConfigurationProvider = clusterConfigurationProvider;
            _configurationProvider = configurationProvider;
        }

        public async Task<bool> IsAuthorized(AppConfigurationDto applicationDetails, string correlationId, string transactionId)
        {
            var adminSecurityGroup = await _configurationProvider.GetConfiguration("Authentication", "AdminSecurityGroup");
            var adminAppIds = (await _configurationProvider.GetConfiguration("Authentication", "AdminAppIds"))?.Split(',')?.ToList();
            var loggedInUser = _authenticationContext.GetLoggedInUserPrincipalName();
            var authenticatedAppId = _authenticationContext.GetLoggedInAppId();

            var parterAdminSecurityGroup = applicationDetails.AdminGroup;
            var partnerAdminApps = applicationDetails.AdminApplications;

            if (!string.IsNullOrWhiteSpace(loggedInUser))
            {
                return ((await _graphManager.IsUserPartOf(parterAdminSecurityGroup, loggedInUser, correlationId, transactionId))
                    || (await _graphManager.IsUserPartOf(adminSecurityGroup, loggedInUser, correlationId, transactionId)));
            }
            else
            {
                return ((partnerAdminApps != null && partnerAdminApps.Any(app => app.ToLowerInvariant() == authenticatedAppId.ToLowerInvariant()))
                    || (adminAppIds != null && adminAppIds.Any(app => app.ToLowerInvariant() == authenticatedAppId.ToLowerInvariant())));
            }
        }

        public async Task<bool> IsAuthorized(string clusterName, string appName, string correlationId, string transactionId)
        {
            var applicationDetails = await _clusterConfigurationProvider.GetApplicationDetails(clusterName, appName);
            if (applicationDetails == null)
                return false;
            return await IsAuthorized(applicationDetails, correlationId, transactionId);
        }

        public async Task EnsureAuthorized(string clusterName, string appName, string correlationId, string transactionId)
        {
            var isAuthorized = await IsAuthorized(clusterName, appName, correlationId, transactionId);
            if (!isAuthorized)
            {
                var upn = _authenticationContext.GetLoggedInUserPrincipalName() ?? _authenticationContext.GetLoggedInAppId();
                throw new UnauthorizedUserException(upn, clusterName, appName);
            }
        }

        public async Task EnsureAuthorized(string clusterName, AppConfigurationDto appConfiguration, string correlationId, string transactionId)
        {
            var isAuthorized = await IsAuthorized(appConfiguration, correlationId, transactionId);
            if (!isAuthorized)
            {
                var upn = _authenticationContext.GetLoggedInUserPrincipalName() ?? _authenticationContext.GetLoggedInAppId();
                throw new UnauthorizedUserException(upn, clusterName, appConfiguration.AppName);
            }
        }
    }
}
