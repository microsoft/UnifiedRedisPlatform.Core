using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Models.Graph
{
    public class Group
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public List<User> Users { get; set; }
    }

    public class GroupResponse
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        public List<Group> Value { get; set; }
    }
}
