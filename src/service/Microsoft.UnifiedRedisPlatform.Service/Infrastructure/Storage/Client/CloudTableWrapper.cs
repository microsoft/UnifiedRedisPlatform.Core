using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.UnifiedPlatform.Storage.Client
{
    public class CloudTableWrapper : ITable
    {
        private readonly CloudTable _baseCloudTable;

        public CloudTableWrapper(CloudTable baseCloudTable)
        {
            _baseCloudTable = baseCloudTable;
        }

        public Task<TableResult> ExecuteAsync(TableOperation operation, TableRequestOptions requestOptions, OperationContext operationContext)
        {
            return _baseCloudTable.ExecuteAsync(operation, requestOptions, operationContext);
        }

        public Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken token, TableRequestOptions requestOptions, OperationContext operationContext) where T : ITableEntity, new()
        {
            return _baseCloudTable.ExecuteQuerySegmentedAsync<T>(query, token, requestOptions, operationContext);
        }
    }

}
