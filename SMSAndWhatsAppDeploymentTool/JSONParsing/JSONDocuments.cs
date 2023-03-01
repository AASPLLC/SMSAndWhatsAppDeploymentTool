using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    public class JSONDocuments
    {
        public string? DeploymentRequirements { get; set; }
        public string? DatabaseTypes { get; set; }
        public string? DeleteDataverseUsers { get; set; }
        public string? KeyVaultSecretsDescriptions { get; set; }
        public string? ManageSMSPhoneNumbers { get; set; }
        public string? WhatsAppConfiguration { get; set; }
        public string? ManualRequirementsAfterDeployment { get; set; }
        public string? PhoneNumberManagementAfterDeployment { get; set; }
        public string? NetworkingDetails { get; set; }
        public string? ManualAPIRegistration { get; set; }

        public static async Task<JSONDocuments> Load()
        {
            return await Globals.LoadJSON<JSONDocuments>(Environment.CurrentDirectory + "/JSONS/Documents.json");
        }

        readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options);
        }

        public void OpenDeploymentRequirements()
        {
            if (DeploymentRequirements != null)
                Globals.LoadPath(DeploymentRequirements);
        }

        public void OpenDatabaseTypes()
        {
            if (DatabaseTypes != null)
                Globals.LoadPath(DatabaseTypes);
        }

        public void OpenDeleteDataverseUsers()
        {
            if (DeleteDataverseUsers != null)
                Globals.LoadPath(DeleteDataverseUsers);
        }

        public void OpenKeyVaultSecretsDescriptions()
        {
            if (KeyVaultSecretsDescriptions != null)
                Globals.LoadPath(KeyVaultSecretsDescriptions);
        }

        public void OpenManageSMSPhoneNumbers()
        {
            if (ManageSMSPhoneNumbers != null)
                Globals.LoadPath(ManageSMSPhoneNumbers);
        }

        public void OpenWhatsAppConfiguration()
        {
            if (WhatsAppConfiguration != null)
                Globals.LoadPath(WhatsAppConfiguration);
        }

        public void OpenManualRequirementsAfterDeployment()
        {
            if (ManualRequirementsAfterDeployment != null)
                Globals.LoadPath(ManualRequirementsAfterDeployment);
        }

        public void OpenPhoneNumberManagementAfterDeployment()
        {
            if (PhoneNumberManagementAfterDeployment != null)
                Globals.LoadPath(PhoneNumberManagementAfterDeployment);
        }

        public void OpenNetworkingDetails()
        {
            if (NetworkingDetails != null)
                Globals.LoadPath(NetworkingDetails);
        }
    }
}