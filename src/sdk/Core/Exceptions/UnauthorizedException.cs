using System;
using Microsoft.UnifiedRedisPlatform.Core.Constants;

namespace Microsoft.UnifiedRedisPlatform.Core.Exceptions
{
    [Serializable]
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string correlationId)
            :base(string.Format(Constant.ExceptionMessages.Unauthorized, correlationId)) { }
    }
}
