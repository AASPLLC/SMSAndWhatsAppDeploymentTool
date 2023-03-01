using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    internal class JSONDefaultDataverseLibrary
    {
        public string? StartingPrefix { get; set; }
        public string? api { get; set; }
        public string? metadataFrom { get; set; }
        public string? metadataMessage { get; set; }
        public string? metadataTo { get; set; }
        public string? metadataTimestamp { get; set; }
        public string? metadataPicPath { get; set; }
        public string? metadataEmailNonAccount { get; set; }
        public string? metadataPhoneNumber { get; set; }
        public string? metadataPhoneNumberID { get; set; }
        public string? metadataEmailAccount { get; set; }
        public string? metadataDisplayName { get; set; }

        public static async Task<JSONDefaultDataverseLibrary> Load()
        {
            return await Globals.LoadJSON<JSONDefaultDataverseLibrary>(Environment.CurrentDirectory + "/JSONS/defaultLibraryDataverse.json");
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
