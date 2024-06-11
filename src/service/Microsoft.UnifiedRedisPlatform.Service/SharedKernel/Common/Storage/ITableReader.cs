using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Data.Tables;
namespace Microsoft.UnifiedPlatform.Service.Common.Storage
{
    public interface ITableReader<TEntity> where TEntity : class, ITableEntity, new()
    {
        Task<TEntity> Get(string partitionKey, string rowKey, string transactionId, string correlationId);
        Task<List<TEntity>> QueryAll(string partitionKey, string transactionId, string correlationId);
    }
}
