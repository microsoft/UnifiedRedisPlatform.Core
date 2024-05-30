using AppInsights.EnterpriseTelemetry;
using AppInsights.EnterpriseTelemetry.Context;
using Azure;
using Azure.Data.Tables;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using Microsoft.UnifiedPlatform.Storage.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Storage
{
    public class TableReader<TEntity> : ITableReader<TEntity> where TEntity : class, ITableEntity, new()
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
            TEntity response=null;
            try
            {
                var performanceContext = new PerformanceContext("TableReader:Get")
                {
                    CorrelationId = correlationId,
                    TransactionId = transactionId
                };
                performanceContext.Start();
                response = await _table.GetAsync<TEntity>(partitionKey, rowKey);
                performanceContext.Stop();
                _logger.Log(performanceContext);
                
            }
            catch (RequestFailedException exception)
            {
                if (exception.Status == Convert.ToInt32(HttpStatusCode.NotFound))
                    return default;
                _logger.Log(exception, correlationId, transactionId);
            
            }
            return response;
        }

        public async Task<List<TEntity>> QueryAll(string partitionKey, string transactionId, string correlationId)
        {
            List<TEntity> response = null;
            try
            {               
                var performanceContext = new PerformanceContext("TableReader:Get")
                {
                    CorrelationId = correlationId,
                    TransactionId = transactionId
                };
                performanceContext.Start();

                response = await _table.GetllAsync<TEntity>(partitionKey);

                performanceContext.Stop();
                _logger.Log(performanceContext);               
            }
            catch (RequestFailedException exception)
            {
                if (exception.Status == Convert.ToInt32(HttpStatusCode.NotFound))
                    return default;
                _logger.Log(exception, correlationId, transactionId);              
            }
            return response;
        }



    }
}
