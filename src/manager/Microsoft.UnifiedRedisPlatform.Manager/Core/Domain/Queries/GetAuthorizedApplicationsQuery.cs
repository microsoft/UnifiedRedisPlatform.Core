using System;
using Microsoft.CQRS;
using System.Collections.Generic;
using Microsoft.UnifiedPlatform.Service.Common.Models;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Queries
{
    public class GetAuthorizedApplicationsQuery : Query<List<ClusterConfigurationDto>>
    {
        public override string DisplayName => "Get Authorized Applications Query";

        private readonly string _id;
        public override string Id => _id;

        public GetAuthorizedApplicationsQuery()
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
