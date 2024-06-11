using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public interface ITable
    {
        Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();

        Task<List<T>> GetllAsync<T>(string partitionKey) where T : class, ITableEntity, new();
       
    }
}