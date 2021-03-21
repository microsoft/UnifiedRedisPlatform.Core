using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.Authentication
{
    public interface IAuthenticator
    {
        Task<string> GenerateToken(string resourceId, Dictionary<string, string> additionalClaims);
    }
}
