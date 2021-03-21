using Microsoft.CQRS;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Results
{
    public class KeysResult : CommandResult
    {
        public List<UnifiedRedisKey> DeletedKeys { get; set; }
        public KeysResult(List<UnifiedRedisKey> deletedKeys)
            :base(isSuccesfull: true, message: $"{deletedKeys.Count} keys has been deleted")
        {
            DeletedKeys = deletedKeys;
        }
    }
}
