using Microsoft.AspNetCore.Authentication;
using Microsoft.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Claims
{
    public class UserClaimsTransformer : IClaimsTransformation
    {
        private IQueryService _queryService;

        public UserClaimsTransformer(IQueryService queryService)
        {
            _queryService = queryService;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
