using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Models
{
    public class ClusterConfigurationDto
    {
        public string ClusterName { get; set; }
        public bool IsProductionCluster { get; set; }
        public string SupportContact { get; set; }
        public string RedisCachePrefix { get; set; }
        public string PrimaryRedisRegion { get; set; }

        public string RedisConnectionString { get; set; }
        public List<string> SecondaryRedisConnectionStrings { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ConnectionStringDto> ConnectionStrings { get; set; }

        public List<AppConfigurationDto> Applications { get; set; }
    }
}
