using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    internal class JSONDefaultCosmosLibrary
    {
        public string? smsIDName { get; set; }
        public string? whatsappIDName { get; set; }
        public string? accountsIDName { get; set; }
        public string? countersIDName { get; set; }
        public string? phoneIDName { get; set; }
        public string? whatsappphoneIDName { get; set; }
        public string? smsContainerName { get; set; }
        public string? whatsappContainerName { get; set; }
        public string? accountsContainerName { get; set; }
        public string? countersContainerName { get; set; }
        public string? phoneContainerName { get; set; }
        public string? whatsappphoneContainerName { get; set; }

        public static async Task<JSONDefaultCosmosLibrary> Load()
        {
            return await Globals.LoadJSON<JSONDefaultCosmosLibrary>(Environment.CurrentDirectory + "/JSONS/defaultLibraryCosmos.json");
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
