using System.Net;
using System.Threading.Tasks;
using AppInsights.EnterpriseTelemetry;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.UnifiedPlatform.Storage.Client;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using AppInsights.EnterpriseTelemetry.Context;

namespace Microsoft.UnifiedPlatform.Storage
{
    public class TableReader<TEntity> : ITableReader<TEntity> where TEntity : ITableEntity, new()
    {
        private readonly ITable _table;
        private readonly ILogger _logger;
        private readonly StorageConfiguration _configuration;

        public TableReader(StorageConfiguration configuration, IStorageClientManager storageClientManager, ILogger logger)
            : this(configuration, storageClientManager, logger, configuration.ConfigurationTableName)
        { }

        public TableReader(StorageConfiguration configuration, IStorageClientManager storageClientManager, ILogger logger, string tableName)
        {
            _table = storageClientManager.CreateTable(tableName).Result;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<TEntity> Get(string partitionKey, string rowKey, string transactionId, string correlationId)
        {
            var performanceContext = new PerformanceContext("TableReader:Get")
            {
                CorrelationId = correlationId,
                TransactionId = transactionId
            };
            performanceContext.Start();

            var readOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var tableRequestOptions = new TableRequestOptions()
            {
                RetryPolicy = new LinearRetry(_configuration.BackoffInternal, _configuration.MaxAttempt)
            };
            var operationContext = new OperationContext()
            {
                ClientRequestID = correlationId
            };

            var response = await _table.ExecuteAsync(readOperation, tableRequestOptions, operationContext);
            performanceContext.Stop();
            _logger.Log(performanceContext);

            if (response.HttpStatusCode == (int)HttpStatusCode.NotFound)
                return default;

            return (TEntity)response.Result;
        }

        public async Task<List<TEntity>> QueryAll(string partitionKey, string transactionId, string correlationId)
        {
            var performanceContext = new PerformanceContext("TableReader:Get")
            {
                CorrelationId = correlationId,
                TransactionId = transactionId
            };
            performanceContext.Start();


            var query = new TableQuery<TEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    partitionKey));

            var tableRequestOptions = new TableRequestOptions()
            {
                RetryPolicy = new LinearRetry(_configuration.BackoffInternal, _configuration.MaxAttempt)
            };
            var operationContext = new OperationContext()
            {
                ClientRequestID = correlationId
            };
            var results = new List<TEntity>();
            TableContinuationToken token;

            do
            {
                var response = await _table.ExecuteQuerySegmentedAsync<TEntity>(query, null, tableRequestOptions, operationContext);
                results.AddRange(response.Results);
                token = response.ContinuationToken;
            } while (token != null);

            performanceContext.Stop();
            _logger.Log(performanceContext);

            return results;
        }
    }
}
