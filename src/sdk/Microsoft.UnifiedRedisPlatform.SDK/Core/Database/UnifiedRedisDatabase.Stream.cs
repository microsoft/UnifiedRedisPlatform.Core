using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.IO;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamAcknowledge(CreateAppKey(key), groupName, messageId, flags));

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamAcknowledge(CreateAppKey(key), groupName, messageIds, flags));

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamAcknowledgeAsync(CreateAppKey(key), groupName, messageId, flags));

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamAcknowledgeAsync(CreateAppKey(key), groupName, messageIds, flags));

        public RedisValue StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamAdd(CreateAppKey(key), streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags));

        public RedisValue StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamAdd(CreateAppKey(key), streamPairs, messageId, maxLength, useApproximateMaxLength, flags));

        public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamAddAsync(CreateAppKey(key), streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags));

        public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamAddAsync(CreateAppKey(key), streamPairs, messageId, maxLength, useApproximateMaxLength, flags));

        public StreamEntry[] StreamClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamClaim(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags));

        public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamClaimAsync(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags));

        public RedisValue[] StreamClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamClaimIdsOnly(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags));

        public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamClaimIdsOnlyAsync(CreateAppKey(key), consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags));

        public bool StreamConsumerGroupSetPosition(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamConsumerGroupSetPosition(CreateAppKey(key), groupName, position, flags));

        public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamConsumerGroupSetPositionAsync(CreateAppKey(key), groupName, position, flags));

        public StreamConsumerInfo[] StreamConsumerInfo(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamConsumerInfo(CreateAppKey(key), groupName, flags));

        public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamConsumerInfoAsync(CreateAppKey(key), groupName, flags));

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamCreateConsumerGroup(CreateAppKey(key), groupName, position, flags));

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamCreateConsumerGroup(CreateAppKey(key), groupName, position, createStream, flags));

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamCreateConsumerGroupAsync(CreateAppKey(key), groupName, position, flags));

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamCreateConsumerGroupAsync(CreateAppKey(key), groupName, position, createStream, flags));

        public long StreamDelete(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamDelete(CreateAppKey(key), messageIds, flags));

        public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamDeleteAsync(CreateAppKey(key), messageIds, flags));

        public long StreamDeleteConsumer(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamDeleteConsumer(CreateAppKey(key), groupName, consumerName, flags));

        public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamDeleteConsumerAsync(CreateAppKey(key), groupName, consumerName, flags));

        public bool StreamDeleteConsumerGroup(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamDeleteConsumerGroup(CreateAppKey(key), groupName, flags));

        public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamDeleteConsumerGroupAsync(CreateAppKey(key), groupName, flags));

        public StreamGroupInfo[] StreamGroupInfo(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamGroupInfo(CreateAppKey(key), flags));

        public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamGroupInfoAsync(CreateAppKey(key), flags));

        public StreamInfo StreamInfo(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamInfo(CreateAppKey(key), flags));

        public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamInfoAsync(CreateAppKey(key), flags));

        public long StreamLength(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamLength(CreateAppKey(key), flags));

        public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamLengthAsync(CreateAppKey(key), flags));

        public StreamPendingInfo StreamPending(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamPending(CreateAppKey(key), groupName, flags));

        public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamPendingAsync(CreateAppKey(key), groupName, flags));
        

        public StreamPendingMessageInfo[] StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamPendingMessages(CreateAppKey(key), groupName, count, consumerName, minId, maxId, flags));

        public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)=>
            ExecuteAsync(() => _primaryDatabase.StreamPendingMessagesAsync(CreateAppKey(key), groupName, count, consumerName, minId, maxId, flags));

        public StreamEntry[] StreamRange(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamRange(CreateAppKey(key), minId, maxId, count, messageOrder, flags));

        public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamRangeAsync(CreateAppKey(key), minId, maxId, count, messageOrder, flags));

        public StreamEntry[] StreamRead(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamRead(CreateAppKey(key), position, count, flags));

        public RedisStream[] StreamRead(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None) => throw new NotImplementedException();

        public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamReadAsync(CreateAppKey(key), position, count, flags));

        public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
            => ExecuteAsync(() => _primaryDatabase.StreamReadAsync(streamPositions, countPerStream, flags));

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamReadGroup(CreateAppKey(key), groupName, consumerName, position, count, flags));

        public RedisStream[] StreamReadGrop(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, bool noAck = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck, flags));

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, flags));

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck, flags));

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamReadGroup(CreateAppKey(key), groupName, consumerName, position, count, noAck, flags));

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamReadGroupAsync(CreateAppKey(key), groupName, consumerName, position, count, flags));

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, bool noAck = false, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck, flags));
        
        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, flags));

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamReadGroupAsync(CreateAppKey(key), groupName, consumerName, position, count, noAck, flags));

        public long StreamTrim(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.StreamTrim(CreateAppKey(key), maxLength, useApproximateMaxLength, flags));

        public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.StreamTrimAsync(CreateAppKey(key), maxLength, useApproximateMaxLength, flags));
    }
}
