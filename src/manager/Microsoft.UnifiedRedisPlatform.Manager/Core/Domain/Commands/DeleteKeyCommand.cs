using System;
using Microsoft.CQRS;

namespace Microsoft.UnifiedRedisPlatform.Manager.Domain.Commands
{
    public class DeleteKeyCommand : Command<IdCommandResult>
    {
        public override string DisplayName => "Delete Key Command";

        private readonly string _id;
        public override string Id => _id;

        public string Cluster { get; set; }
        public string Application { get; set; }
        public string KeyName { get; set; }
        public bool DeleteSecondary { get; set; }

        public DeleteKeyCommand(string cluster, string application, string keyName, bool deleteSecondary)
        {
            _id = Guid.NewGuid().ToString();
            Cluster = cluster;
            Application = application;
            KeyName = keyName;
            DeleteSecondary = deleteSecondary;
        }

        public override bool Validate(out string ValidationErrorMessage)
        {
            ValidationErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Cluster))
                ValidationErrorMessage = "Cluster cannot be null. ";
            if (string.IsNullOrWhiteSpace(Application))
                ValidationErrorMessage = "Application cannot be null. ";
            if (string.IsNullOrWhiteSpace(KeyName))
                ValidationErrorMessage = "Key cannot be null.";

            return string.IsNullOrWhiteSpace(ValidationErrorMessage);
        }
    }
}
