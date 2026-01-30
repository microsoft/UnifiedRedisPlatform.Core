using System;
using Microsoft.CQRS;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries
{
    public class GetKeyQuery : Query<UnifiedRedisKey>
    {
        public override string DisplayName => "Get Key Query";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string Key { get; set; }

        public GetKeyQuery(string cluster, string application, string key)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            Key = key;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Cluster))
            {
                ValidationErrorMessage = "Cluster is not provided";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Application))
            {
                ValidationErrorMessage = "Application is not provided";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Key))
            {
                ValidationErrorMessage = "Key is not provided";
                return false;
            }
            return true;
        }
    }
}
