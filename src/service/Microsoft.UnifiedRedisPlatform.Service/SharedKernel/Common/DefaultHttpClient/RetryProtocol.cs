using System;
using System.Collections.Generic;

namespace Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient
{
    public class RetryProtocol
    {
        public int MaxRetryCount { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeSpan MaxBackOffInterval { get; set; }
        public List<string> TransientFailureCodes { get; set; }

        public RetryProtocol()
        {
            //Set Default Retry values
            MaxRetryCount = 3;
            Timeout = TimeSpan.FromSeconds(30);
            MaxBackOffInterval = TimeSpan.FromMilliseconds(250);
            TransientFailureCodes = new List<string>();
        }
    }
}
