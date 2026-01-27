using System;
using Microsoft.CQRS;
using Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands.Results;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands
{
    public class FlushKeysCommand : Command<KeysResult>
    {
        public override string DisplayName => "Flush Keys Command";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string SearchText { get; set; }
        public bool DeleteSecondary { get; set; }

        public FlushKeysCommand(string cluster, string application, string searchText, bool deleteSecondary)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            SearchText = searchText;
            DeleteSecondary = deleteSecondary;
        }

        public FlushKeysCommand(string cluster, string application, string searchText)
            : this(cluster, application, searchText, false)
        { }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Cluster))
            {
                ValidationErrorMessage = "Cluster is not provided";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Application))
            {
                ValidationErrorMessage = "Application is not provided";
                return false;
            }
            return true;
        }
    }
}
