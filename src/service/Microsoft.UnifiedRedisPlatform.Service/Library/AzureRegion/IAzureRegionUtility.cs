using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AzureRegion.Models;

namespace Microsoft.AzureRegion
{
    public interface IAzureRegionUtility
    {
        Task<bool> Exists(string regionName);
        Task<List<RegionModel>> GetAllAzureRegions(bool useInMemCache = true);
        Task<RegionModel> GetNearestRegion(string regionName);
        Task<RegionModel> GetNearestRegion(string regionName, List<string> availableRegionNames);
    }
}
