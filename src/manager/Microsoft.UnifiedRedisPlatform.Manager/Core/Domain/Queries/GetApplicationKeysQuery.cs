using System;
using Microsoft.CQRS;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries
{
    public class GetApplicationKeysQuery : Query<List<UnifiedRedisKey>>
    {
        public override string DisplayName => "Get Application Keys";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string SearchText { get; set; }

        public GetApplicationKeysQuery(string cluster, string application, string searchText)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            SearchText = searchText;
        }

        public GetApplicationKeysQuery(string cluster, string application)
            :this(cluster, application, null)
        { }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = "";
            if (string.IsNullOrWhiteSpace(Cluster))
                ValidationErrorMessage += "Cluster name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Application))
                ValidationErrorMessage += "Application name cannot be empty.";
            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
