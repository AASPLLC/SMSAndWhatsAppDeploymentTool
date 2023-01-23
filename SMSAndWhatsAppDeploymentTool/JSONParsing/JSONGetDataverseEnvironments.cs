using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    public class JSONGetDataverseEnvironments
    {
        public string? odatacontext { get; set; }
        public Value[]? value { get; set; }

        public class Value
        {
            public string? Id { get; set; }
            public string? UniqueName { get; set; }
            public string? UrlName { get; set; }
            public string? FriendlyName { get; set; }
            public int? State { get; set; }
            public string? Version { get; set; }
            public string? Url { get; set; }
            public string? ApiUrl { get; set; }
            public DateTime? LastUpdated { get; set; }
            public string? SchemaType { get; set; }
        }

    }
}
