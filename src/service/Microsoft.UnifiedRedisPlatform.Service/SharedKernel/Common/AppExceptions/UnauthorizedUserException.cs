using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    public class UnauthorizedUserException: Exception
    {
        public string UserPrincipalName { get; set; }
        public string ClusterName { get; set; }
        public string AppName { get; set; }
        
        public UnauthorizedUserException(string userPrincipalName, string clusterName, string appName):
            base($"User {userPrincipalName} is not authorized to access app {appName} in {clusterName}")
        {
            UserPrincipalName = userPrincipalName;
            ClusterName = clusterName;
            AppName = appName;
        }
    }
}
