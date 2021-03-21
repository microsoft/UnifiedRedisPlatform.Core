using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.Graph
{
    public interface IGraphManager
    {
        Task<bool> IsUserPartOf(string groupMail, string userPrincipalName, string correlationId, string transactionId);
    }
}
