using System.Linq;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Models
{
    internal class ClusterConfiguration
    {
        public string ClusterName { get; set; }
        public bool IsProductionCluster { get; set; }
        public string SupportContact { get; set; }
        public string RedisCachePrefix { get; set; }
        public string PrimaryRedisRegion { get; set; }
        public string RedisConnectionString { get; set; }
        public List<string> SecondaryRedisConnectionStrings { get; set; }
        public List<ApplicationConfiguration> Applications { get; set; } = new List<ApplicationConfiguration>();

        public void AddDefaultValues()
        {
            foreach (var application in Applications)
            {
                application.AddDefaultValues();
            }
        }

        public bool AreSecondaryConnectionsPresent => 
            SecondaryRedisConnectionStrings != null && 
            SecondaryRedisConnectionStrings.Any() && 
            Applications.TrueForAll(application => application.AreSecondaryConnectionsPresent);
    }
}
