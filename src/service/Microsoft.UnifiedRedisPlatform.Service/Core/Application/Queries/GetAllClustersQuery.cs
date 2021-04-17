using System;
using CQRS.Mediatr.Lite;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedPlatform.Service.Application.Queries
{
    public class GetAllClustersQuery : Query<List<ClusterConfigurationDto>>
    {
        public override string DisplayName => "Get Cluster Configuration Query";

        private readonly string _id;
        public override string Id => _id;

        public GetAllClustersQuery()
        {
            _id = Guid.NewGuid().ToString();
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            return true;
        }
    }
}
