using System;
using Microsoft.UnifiedRedisPlatform.Core.Constants;

namespace Microsoft.UnifiedRedisPlatform.Core.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException: Exception
    {   
        public InvalidConfigurationException(string invalidArgument, Exception innerException)
            :base(string.Format(Constant.ExceptionMessages.InvalidConfiguration, invalidArgument), innerException)
        { }
    }
}
