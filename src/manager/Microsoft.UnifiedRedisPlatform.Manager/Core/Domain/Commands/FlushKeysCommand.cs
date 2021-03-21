using System;
using Microsoft.CQRS;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Results;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands
{
    public class FlushKeysCommand : Command<KeysResult>
    {
        public override string DisplayName => "Flush Application Command";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string SearchText { get; set; }
        public bool DeleteSecondary { get; set; }

        public FlushKeysCommand(string cluster, string application, string searchText, bool deleteSecondary = false)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            SearchText = searchText;
            DeleteSecondary = deleteSecondary;
        }

        public FlushKeysCommand(string cluster, string application)
            : this(cluster, application, null)
        { }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            
            if (string.IsNullOrWhiteSpace(Cluster))
                ValidationErrorMessage += "Cluster name cannot be empty.";
            if (string.IsNullOrWhiteSpace(Application))
                ValidationErrorMessage += "Application name cannot be empty.";
            
            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
