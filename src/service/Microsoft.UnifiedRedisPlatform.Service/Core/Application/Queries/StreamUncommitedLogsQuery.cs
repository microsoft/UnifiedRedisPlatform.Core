using System;
using Microsoft.CQRS;
using System.Threading.Channels;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries
{
    public class StreamUncommitedLogsQuery : Query<bool>
    {
        public override string DisplayName => "Get Uncommited Logs Query";

        private readonly string _id;
        public override string Id => _id;

        public string ClusterName { get; set; }
        public string AppName { get; set; }

        public int BatchSize { get; set; }

        public Channel<List<Log>> LogChannel { get; private set; }

        public StreamUncommitedLogsQuery(string cluster, string app, Channel<List<Log>> logChannel, int batchSize)
        {
            _id = Guid.NewGuid().ToString();
            ClusterName = cluster;
            AppName = app;
            LogChannel = logChannel;
            BatchSize = batchSize;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            if (ClusterName == null)
            {
                ValidationErrorMessage = "Cluster Configuration cannot be null";
            }
            if (AppName == null)
            {
                ValidationErrorMessage = "Application Configuration cannot be null";
            }

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
