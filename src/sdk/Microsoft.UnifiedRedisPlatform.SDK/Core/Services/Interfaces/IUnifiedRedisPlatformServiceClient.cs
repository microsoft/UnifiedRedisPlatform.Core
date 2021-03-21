using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces
{
    internal interface IUnifiedRedisPlatformServiceClient
    {
        Task<AuthTokenResponseModel> GetAuthToken();
        Task<ClusterConfiguration> GetClusterConfiguration();
        Task<ServerLogResult> Log(List<GenericLog> logs, int attempt, int maxAttempt);
    }
}
