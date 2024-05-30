using Azure;
using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class CloudTableWrapper : ITable
    {

        private readonly TableClient _baseTableClient;

        public CloudTableWrapper(TableClient baseTableClient)
        {
            _baseTableClient = baseTableClient;
        }

        public async Task<T> GetAsync<T>(string partitionKey, string rowKey) where T :class, ITableEntity, new()
        {
            return await _baseTableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }

        public async Task<List<T>> GetllAsync<T>(string partitionKey) where T : class, ITableEntity, new()
        {
            var result = new List<T>();
            var partitionKeyFilter = $"PartitionKey eq '{partitionKey}'";
            AsyncPageable<T> tableResult = _baseTableClient.QueryAsync<T>(partitionKeyFilter);
            var enumerator = tableResult.GetAsyncEnumerator();

            while (await enumerator.MoveNextAsync())
            {
                result.Add(enumerator.Current);
            }
            return result;
        }

    }
}
