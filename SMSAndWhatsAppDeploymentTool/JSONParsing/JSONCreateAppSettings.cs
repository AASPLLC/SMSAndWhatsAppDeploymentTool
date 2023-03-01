using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    public class JSONCreateAppSettings
    {
        public Settings? settings { get; set; }

        public class Settings
        {
            public string? clientId { get; set; }
            public string? tenantId { get; set; }
            public string? clientSecret { get; set; }
            public string? authTenant { get; set; }

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
}
