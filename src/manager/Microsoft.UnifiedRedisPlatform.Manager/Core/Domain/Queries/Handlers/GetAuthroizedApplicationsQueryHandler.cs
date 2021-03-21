using System.Linq;
using Microsoft.CQRS;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Authentication;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries.Handlers
{
    public class GetAuthroizedApplicationsQueryHandler : QueryHandler<GetAuthorizedApplicationsQuery, List<ClusterConfigurationDto>>
    {
        private readonly IApplicationAuthenticator _applicationAuthenticator;
        private readonly IClusterConfigurationProvider _clusterConfigurationProvider;

        public GetAuthroizedApplicationsQueryHandler(IApplicationAuthenticator applicationAuthenticator, IClusterConfigurationProvider clusterConfigurationProvider)
        {
            _applicationAuthenticator = applicationAuthenticator;
            _clusterConfigurationProvider = clusterConfigurationProvider;
        }

        protected override async Task<List<ClusterConfigurationDto>> ProcessRequest(GetAuthorizedApplicationsQuery request)
        {
            var authorizedClusters = new ConcurrentDictionary<string, ClusterConfigurationDto>();
            var clusters = await _clusterConfigurationProvider.GetAllClusters();
            var isAuthorizedTasks = new List<Task>();
            foreach (var cluster in clusters)
            {
                cluster.Applications = new List<AppConfigurationDto>();
                var applications = await _clusterConfigurationProvider.GetAllApplications(cluster.ClusterName);
                foreach (var application in applications)
                {
                    isAuthorizedTasks.Add(Task.Run(async () =>
                    {
                        if (await _applicationAuthenticator.IsAuthorized(application, request.CorrelationId, request.TransactionId))
                        {
                            if (!authorizedClusters.ContainsKey(cluster.ClusterName))
                                authorizedClusters.AddOrUpdate(cluster.ClusterName, cluster, (clusterName, existingConfig) => existingConfig);
                                
                            authorizedClusters[cluster.ClusterName].Applications.Add(application);
                        }
                    }));
                }
            }

            await Task.WhenAll(isAuthorizedTasks);
            return authorizedClusters.Values.ToList();
        }
    }
}
