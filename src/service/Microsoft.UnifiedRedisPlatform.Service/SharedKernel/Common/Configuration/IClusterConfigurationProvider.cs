using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Common.Configuration
{
    public interface IClusterConfigurationProvider
    {
        Task<List<ClusterConfigurationDto>> GetAllClusters();
        Task<List<AppConfigurationDto>> GetAllApplications(string clusterName);
        Task<ClusterConfigurationDto> GetClusterDetails(string clusterName);
        Task<AppConfigurationDto> GetApplicationDetails(string clusterName, string appName);
        Task<string> GetApplicationSecret(string clusterName, string appName);
        Task<List<ConnectionStringDto>> GetClusterConnectionStrings(string clusterName, string appName);
    }
}
