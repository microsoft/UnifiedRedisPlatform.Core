using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public partial class UnifiedConnectionMultiplexer
    {   
        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;

        private void SetupEventHandlers()
        {
            foreach (var connection in GetAllConnectionMultiplexers())
            {
                connection.ErrorMessage += (sender, e) => ErrorMessage(sender, e);
                connection.ErrorMessage += (sender, e) => _logger.LogException(new Exception(e.Message), new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "Error", e.Message } });
                connection.ErrorMessage += (sender, e) => _logger.LogEvent("Redis:ErrorMessage", 0.0, new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "Error", e.Message } });

                connection.ConnectionFailed += (sender, e) => ConnectionFailed(sender, e);
                connection.ConnectionFailed += (sender, e) => _logger.LogException(
                    e.Exception,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "FailureType", e.FailureType.ToString() }, { "ConnectionType", e.ConnectionType.ToString() } });
                connection.ConnectionFailed += (sender, e) => _logger.LogEvent(
                    "Redis:ConnectionFailed", 0.0,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "FailureType", e.FailureType.ToString() }, { "ConnectionType", e.ConnectionType.ToString() }, { "Error", e.Exception.Message } });

                connection.InternalError += (sender, e) => InternalError(sender, e);
                connection.InternalError += (sender, e) => _logger.LogException(
                    e.Exception,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "Origin", e.Origin }, { "ConnectionType", e.ConnectionType.ToString() } });
                connection.InternalError += (sender, e) => _logger.LogEvent(
                    "Redis:InternalError", 0.0,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "Origin", e.Origin }, { "ConnectionType", e.ConnectionType.ToString() }, { "Error", e.Exception.Message } });

                connection.ConnectionRestored += (sender, e) => ConnectionRestored(sender, e);
                connection.ConnectionRestored += (sender, e) => _logger.LogEvent("Redis:ConnectionRestored", 0.0,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() }, { "FailureType", e.FailureType.ToString() }, { "ConnectionType", e.ConnectionType.ToString() } });

                connection.ConfigurationChanged += (sender, e) => ConfigurationChanged(sender, e);
                connection.ConfigurationChanged += (sender, e) => _logger.LogEvent("Redis:ConfigurationChanged", 0.0,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() } });

                connection.ConfigurationChangedBroadcast += (sender, e) => ConfigurationChangedBroadcast(sender, e);
                connection.ConfigurationChangedBroadcast += (sender, e) => _logger.LogEvent("Redis:ConfigurationChangedBroadcast", 0.0,
                    new Dictionary<string, string>() { { "Endpoint", e.EndPoint.ToString() } });

                connection.HashSlotMoved += (sender, e) => HashSlotMoved(sender, e);
                connection.HashSlotMoved += (sender, e) => _logger.LogEvent("Redis:HashSlotMoved", 0.0,
                    new Dictionary<string, string>() { { "OldEndpoint", e.OldEndPoint.ToString() }, { "NewEndpoint", e.NewEndPoint.ToString() }, { "HashSlot", e.HashSlot.ToString() } });
            }
        }
    }
}
