using System;
using System.IO;
using System.Net;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using StackExchange.Redis.Profiling;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public partial class UnifiedConnectionMultiplexer
    {
        #region Original Connection Mux fields
        public string ClientName => _baseConnectionMux.ClientName;
        public string Configuration => _baseConnectionMux.Configuration;
        public int TimeoutMilliseconds => _baseConnectionMux.TimeoutMilliseconds;
        public long OperationCount => _baseConnectionMux.OperationCount;
        [Obsolete]
        public bool PreserveAsyncOrder { get => _baseConnectionMux.PreserveAsyncOrder; set => _baseConnectionMux.PreserveAsyncOrder = value; }
        public bool IsConnected
        {
            get
            {
                var isConnected = true;
                foreach (var connection in GetAllConnectionMultiplexers())
                    isConnected &= connection.IsConnected;
                return isConnected;
            }
        }

        public bool IsConnecting
        {
            get
            {
                var isConnecting = true;
                foreach (var connection in GetAllConnectionMultiplexers())
                    isConnecting &= connection.IsConnecting;
                return isConnecting;
            }
        }

        public bool IncludeDetailInExceptions { get => _baseConnectionMux.IncludeDetailInExceptions; set => _baseConnectionMux.IncludeDetailInExceptions = value; }
        public int StormLogThreshold { get => _baseConnectionMux.StormLogThreshold; set => _baseConnectionMux.StormLogThreshold = value; }

        public string AppSecret { get; }
        #endregion Original Connection Mux fields



        #region Connection Mux Methods
        public void Close(bool allowCommandsToComplete = true) => GetAllConnectionMultiplexers().ForEach(connection => connection.Close());

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            var closeTasks = new List<Task>();
            GetAllConnectionMultiplexers().ForEach(connection => closeTasks.Add(connection.CloseAsync()));
            return Task.WhenAll(closeTasks);
        }


        public bool Configure(TextWriter log = null) => _baseConnectionMux.Configure(log);

        public Task<bool> ConfigureAsync(TextWriter log = null) => _baseConnectionMux.ConfigureAsync(log);

        public void Dispose() => GetAllConnectionMultiplexers().ForEach(connection => connection.Dispose());

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1)) => _baseConnectionMux.ExportConfiguration(destination, options);

        public ServerCounters GetCounters() => _baseConnectionMux.GetCounters();

        public EndPoint[] GetEndPoints(bool configuredOnly = false) => _baseConnectionMux.GetEndPoints(configuredOnly);

        public int GetHashSlot(RedisKey key) => _baseConnectionMux.GetHashSlot(key);

        public IServer GetServer(string host, int port, object asyncState = null) => _baseConnectionMux.GetServer(host, port, asyncState);

        public IServer GetServer(string hostAndPort, object asyncState = null) => _baseConnectionMux.GetServer(hostAndPort, asyncState);

        public IServer GetServer(IPAddress host, int port) => _baseConnectionMux.GetServer(host, port);

        public IServer GetServer(EndPoint endpoint, object asyncState = null) => _baseConnectionMux.GetServer(endpoint, asyncState);

        public string GetStatus() => _baseConnectionMux.GetStatus();

        public void GetStatus(TextWriter log) => _baseConnectionMux.GetStatus(log);

        public string GetStormLog() => _baseConnectionMux.GetStormLog();

        public ISubscriber GetSubscriber(object asyncState = null) => _baseConnectionMux.GetSubscriber(asyncState);

        public int HashSlot(RedisKey key) => _baseConnectionMux.HashSlot(key);

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None) => _baseConnectionMux.PublishReconfigure(flags);

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None) => _baseConnectionMux.PublishReconfigureAsync(flags);

        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider) => _baseConnectionMux.RegisterProfiler(profilingSessionProvider);

        public void ResetStormLog() => _baseConnectionMux.ResetStormLog();

        public void Wait(Task task) => _baseConnectionMux.Wait(task);

        public T Wait<T>(Task<T> task) => _baseConnectionMux.Wait<T>(task);

        public void WaitAll(params Task[] tasks) => _baseConnectionMux.WaitAll(tasks);
        #endregion Connection Mux Methods
    }
}
