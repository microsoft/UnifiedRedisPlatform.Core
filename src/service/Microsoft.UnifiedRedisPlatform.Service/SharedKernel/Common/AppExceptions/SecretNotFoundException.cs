using System;

namespace Microsoft.UnifiedPlatform.Service.Common.AppExceptions
{
    [Serializable]
    public class SecretNotFoundException: Exception
    {   
        public SecretNotFoundException(string key, Exception innerException)
            :base($"Key with name {key} doesnt exist", innerException) { }
    }
}
