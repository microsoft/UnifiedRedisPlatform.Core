using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries.Handlers
{
    public class GetAllClustersQueryHandler : QueryHandler<GetAllClustersQuery, List<ClusterConfigurationDto>>
    {
        private readonly IClusterConfigurationProvider _configurationProvider;

        public GetAllClustersQueryHandler(IClusterConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        protected override async Task<List<ClusterConfigurationDto>> ProcessRequest(GetAllClustersQuery request)
        {
            var clusters = await _configurationProvider.GetAllClusters();
            if (clusters == null || !clusters.Any())
                return null;

            foreach (var cluster in clusters)
            {
                cluster.Applications = new List<AppConfigurationDto>();
                var applications = await _configurationProvider.GetAllApplications(cluster.ClusterName);
                if (applications == null || !applications.Any())
                    continue;
                cluster.Applications.AddRange(applications);
            }

            return clusters;
        }
    }
}
