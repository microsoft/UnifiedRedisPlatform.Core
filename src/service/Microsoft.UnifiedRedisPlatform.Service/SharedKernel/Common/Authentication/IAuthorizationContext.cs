namespace Microsoft.UnifiedPlatform.Service.Common.Authentication
{
    public interface IAuthorizationContext
    {
        /// <summary>
        /// Checks if the logged in service princial in AAD Application
        /// </summary>
        /// <returns></returns>
        bool IsAppContext();

        /// <summary>
        /// Gets the User Principal Name of the logged in User
        /// </summary>
        /// <returns>User Principal Name</returns>
        string GetLoggedInUserPrincipalName();

        /// <summary>
        /// Gets the AAD App ID of the logged in Application
        /// </summary>
        /// <returns>AAD Application ID</returns>
        string GetLoggedInAppId();
    }
}
