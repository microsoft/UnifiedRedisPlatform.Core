using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Common.Authentication
{
    public interface IApplicationAuthenticator
    {
        Task<bool> IsAuthorized(AppConfigurationDto applicationDetails, string correlationId, string transactionId);
        Task<bool> IsAuthorized(string clusterName, string appName, string correlationId, string transactionId);
        Task EnsureAuthorized(string clusterName, string appName, string correlationId, string transactionId);
    }
}
