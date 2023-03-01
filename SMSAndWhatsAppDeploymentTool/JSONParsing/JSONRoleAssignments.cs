using AASPGlobalLibrary;
using System.Text.Json;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    internal class JSONRoleAssignments
    {
        public string? schema { get; set; }
        public string? contentVersion { get; set; }
        public Parameters? parameters { get; set; }
        public Resource[]? resources { get; set; }

        public class Parameters
        {
            public Storageaccounts? storageAccounts { get; set; }
            public Prin? prin { get; set; }
            public Role? role { get; set; }
            public Name? name { get; set; }
        }

        public class Storageaccounts
        {
            public string? type { get; set; }
            public string? defaultValue { get; set; }
            public Metadata? metadata { get; set; }
        }

        public class Metadata
        {
            public string? description { get; set; }
        }

        public class Prin
        {
            public string? type { get; set; }
            public string? defaultValue { get; set; }
            public Metadata1? metadata { get; set; }
        }

        public class Metadata1
        {
            public string? description { get; set; }
        }

        public class Role
        {
            public string? type { get; set; }
            public string? defaultValue { get; set; }
            public Metadata2? metadata { get; set; }
        }

        public class Metadata2
        {
            public string? description { get; set; }
        }

        public class Name
        {
            public string? type { get; set; }
            public string? defaultValue { get; set; }
            public Metadata3? metadata { get; set; }
        }

        public class Metadata3
        {
            public string? description { get; set; }
        }

        public class Resource
        {
            public string? apiVersion { get; set; }
            public string? type { get; set; }
            public string? name { get; set; }
            public string? location { get; set; }
            public Sku? sku { get; set; }
            public string? kind { get; set; }
            public Properties? properties { get; set; }
            public string? scope { get; set; }
            public string[]? dependsOn { get; set; }
        }

        public class Sku
        {
            public string? name { get; set; }
        }

        public class Properties
        {
            public string? roleDefinitionId { get; set; }
            public string? principalId { get; set; }
        }

        public static async Task<JSONRoleAssignments> Load()
        {
            return await Globals.LoadJSON<JSONRoleAssignments>(Environment.CurrentDirectory + "/JSONS/StorageRoleAccess.json");
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
