﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
