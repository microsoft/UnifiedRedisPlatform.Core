using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Models.Graph
{
    public class User
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string UserPrincipalName { get; set; }
    }

    public class UserResponse
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        public List<User> Value;
    }
}
