using System.Linq;
using Microsoft.Graph;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Graph;
using Microsoft.UnifiedPlatform.Service.Common.Caching;

namespace Microsoft.UnifiedPlatform.Service.Graph
{
    public class GraphManager : IGraphManager
    {   
        private readonly ICacheService _cacheService;
        private readonly IGraphServiceClient _graphServiceClient;

        private const string CacheNamespace = "upn:GRAPH:";

        public GraphManager(ICacheService cacheService, IGraphServiceClient graphServiceClient)
        {
            _cacheService = cacheService;
            _graphServiceClient = graphServiceClient;
        }

        public async Task<bool> IsUserPartOf(string groupMail, string userPrincipalName, string correlationId, string transactionId)
        {
            if (string.IsNullOrWhiteSpace(groupMail) || string.IsNullOrWhiteSpace(userPrincipalName))
                return false;
            
            var cachedKey = $"{CacheNamespace}:{userPrincipalName}:ispartof:{groupMail}";
            var cachedResult = await _cacheService.Get<string>(cachedKey);
            if (!string.IsNullOrWhiteSpace(cachedResult))
            {
                return cachedResult.ToLowerInvariant() == bool.TrueString.ToLowerInvariant();
            }

            var groupId = await GetGroupId(groupMail);
            if (string.IsNullOrWhiteSpace(groupId))
                return false;

            var memberGroups = await _graphServiceClient.Users[userPrincipalName]
                .CheckMemberGroups(new List<string>() { groupId })
                .Request()
                .PostAsync();

            var isUserPartOf = memberGroups != null && memberGroups.Any();
            await _cacheService.Set(cachedKey, isUserPartOf.ToString());
            return isUserPartOf;
        }

        private async Task<string> GetGroupId(string groupMail)
        {
            var groupMailParts = groupMail.Split('@');
            var groupMailNickname = groupMailParts[0];
            var group = await _graphServiceClient.Groups
                .Request()
                .Filter($"mailNickname eq '{groupMailNickname}'")
                .GetAsync();

            return group.FirstOrDefault()?.Id;
        }
    }
}
