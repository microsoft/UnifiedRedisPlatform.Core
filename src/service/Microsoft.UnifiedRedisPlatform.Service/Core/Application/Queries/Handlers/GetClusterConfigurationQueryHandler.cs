using System.Linq;
using Microsoft.CQRS;
using Microsoft.AzureRegion;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries.Handlers
{
    public class GetClusterConfigurationQueryHandler : QueryHandler<GetClusterConfigurationQuery, ClusterConfigurationDto>
    {
        private readonly IAzureRegionUtility _azureRegionUtility;
        private readonly IClusterConfigurationProvider _configurationProvider;
        private readonly AppMetadataConfiguration _appMetadataConfiguration;

        public GetClusterConfigurationQueryHandler(AppMetadataConfiguration appConfiguration, IAzureRegionUtility azureRegionUtility, IClusterConfigurationProvider configurationProvider)
        {
            _appMetadataConfiguration = appConfiguration;
            _azureRegionUtility = azureRegionUtility;
            _configurationProvider = configurationProvider;
        }

        protected override async Task<ClusterConfigurationDto> ProcessRequest(GetClusterConfigurationQuery request)
        {
            var clusterConfiguration = await _configurationProvider.GetClusterDetails(request.ClusterName);
            var applicationConfiguration = await _configurationProvider.GetApplicationDetails(request.ClusterName, request.AppName);
            clusterConfiguration.Applications = new List<AppConfigurationDto>() { applicationConfiguration };

            var clusterConnectionStrings = await _configurationProvider.GetClusterConnectionStrings(request.ClusterName, request.AppName);
            if (clusterConnectionStrings.Count == 1)
            {
                clusterConfiguration.RedisConnectionString = clusterConnectionStrings.First().ConnectionString;
                clusterConfiguration.PrimaryRedisRegion = clusterConnectionStrings.First().Region?.Name;
                clusterConfiguration.SecondaryRedisConnectionStrings = null;
                clusterConfiguration.ConnectionStrings = null;
                return clusterConfiguration;
            }

            var preferredLocaltion = !string.IsNullOrWhiteSpace(request.PreferredLocation) && (await _azureRegionUtility.Exists(request.PreferredLocation))
                ? request.PreferredLocation
                : _appMetadataConfiguration.Region.Name;

            var availableRegions = clusterConnectionStrings.Select(connectionString => connectionString.Region.Name).ToList();
            var nearestRegion = await _azureRegionUtility.GetNearestRegion(preferredLocaltion, availableRegions);

            clusterConfiguration.RedisConnectionString = clusterConnectionStrings
                .First(connectionStringConfig => connectionStringConfig.Region.Name == nearestRegion.Name).ConnectionString;
            clusterConfiguration.PrimaryRedisRegion = clusterConnectionStrings
                .First(connectionStringConfig => connectionStringConfig.Region.Name == nearestRegion.Name).Region.Name;
            
            clusterConfiguration.SecondaryRedisConnectionStrings =
                clusterConnectionStrings.Where(connectionStringConfig => connectionStringConfig.Region.Name != nearestRegion.Name)
                .Select(connectionStringConfig => connectionStringConfig.ConnectionString)
                .ToList();
            clusterConfiguration.ConnectionStrings = null;
            return clusterConfiguration;
        }
    }
}
