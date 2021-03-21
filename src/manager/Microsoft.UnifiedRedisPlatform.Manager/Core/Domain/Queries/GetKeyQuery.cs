using Microsoft.CQRS;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries
{
    public class GetKeyQuery : Query<UnifiedRedisKey>
    {
        public override string DisplayName => "Get Key Query";

        private string _id;
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
                ValidationErrorMessage += "Cluster name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Application))
                ValidationErrorMessage += "Application name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Key))
                ValidationErrorMessage += "Key cannot be empty";

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
