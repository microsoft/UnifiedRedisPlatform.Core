using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.AzureRegion.Models
{
    internal class AzureLocationApiResponse
    {
        [JsonProperty("value")]
        public List<RegionModel> Value { get; set; }
    }
}
