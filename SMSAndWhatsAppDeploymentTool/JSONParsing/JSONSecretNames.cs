using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    internal class JSONSecretNames
    {
        public string? PDynamicsEnvironment { get; set; }
        public string? PAccountsDBPrefix { get; set; }
        public string? PSMSDBPrefix { get; set; }
        public string? PWhatsAppDBPrefix { get; set; }
        public string? PCommsEndpoint { get; set; }
        public string? PWhatsAppAccess { get; set; }
        public string? PPhoneNumberDBPrefix { get; set; }
        public string? PTenantID { get; set; }
        public string? IoOrgID { get; set; }
        public string? IoClientID { get; set; }
        public string? IoSecret { get; set; }
        public string? IoEmail { get; set; }
        public string? IoJobs { get; set; }
        public string? IoCallback { get; set; }
        public string? Type { get; set; }
        public string? IoCosmos { get; set; }
        public string? IoKey { get; set; }
        public string? RESTSite { get; set; }
        public string? DbName { get; set; }
        public string? DbName1 { get; set; }
        public string? DbName2 { get; set; }
        public string? DbName3 { get; set; }
        public string? DbName4 { get; set; }
        public string? DbName5 { get; set; }
        public string? AutomationId { get; set; }
        public string? SMSTemplate { get; set; }

        public static async Task<JSONSecretNames> Load()
        {
            return await Globals.LoadJSON<JSONSecretNames>(Environment.CurrentDirectory + "/JSONS/SecretNames.json");
        }

        readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options);
        }
    }
}
