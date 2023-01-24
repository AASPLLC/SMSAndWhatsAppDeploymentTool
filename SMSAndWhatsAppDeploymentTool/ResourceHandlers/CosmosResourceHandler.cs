using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using System.Text.Json;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.CosmosDB;
using Azure.Core;
using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    //keep ARM plus Management way for security team
    public class CosmosResourceHandler
    {
        class JSONDefaultCosmosLibrary
        {
            public string? smsIDName { get; set; }
            public string? whatsappIDName { get; set; }
            public string? accountsIDName { get; set; }
            public string? countersIDName { get; set; }
            public string? smsContainerName { get; set; }
            public string? whatsappContainerName { get; set; }
            public string? accountsContainerName { get; set; }
            public string? countersContainerName { get; set; }
        }
        public static async Task InitialCreation(ResourceIdentifier subnetID, string desiredCosmosName, string vnetName, CosmosDeploy form, bool useArm = false)
        {
            if (useArm)
                await CreateCosmosARM(form, Environment.CurrentDirectory + @"\JSONS\CosmosDeploy.json", desiredCosmosName, form.SelectedSubscription.Data.SubscriptionId, vnetName, subnetID.Name);
            else
                await CreateCosmosDB(subnetID, desiredCosmosName, form);
        }

        static async Task CreateCosmosDB(ResourceIdentifier subnetID, string desiredCosmosName, CosmosDeploy form)
        {
            JSONDefaultCosmosLibrary? cosmosLibrary;
            try { cosmosLibrary = JsonSerializer.Deserialize<JSONDefaultCosmosLibrary>(await File.ReadAllBytesAsync(Environment.CurrentDirectory + "/JSONS/defaultLibraryCosmos.json")); }
            catch { cosmosLibrary = JsonSerializer.Deserialize<JSONDefaultCosmosLibrary>(await Globals.OpenJSONFileAsync()); }

            List<CosmosDBAccountLocation> Locations = new();
            CosmosDBAccountLocation locationstuff = new()
            {
                LocationName = form.SelectedRegion,
                IsZoneRedundant = true
            };
            Locations.Add(locationstuff);
            CosmosDBAccountCreateOrUpdateContent accountcontent = new(form.SelectedRegion, Locations)
            {
                PublicNetworkAccess = CosmosDBPublicNetworkAccess.Enabled,
                IsVirtualNetworkFilterEnabled = true
            };
            accountcontent.VirtualNetworkRules.Add(new()
            {
                Id = subnetID,
                IgnoreMissingVnetServiceEndpoint = true
            });
            try
            {
                CosmosDBAccountResource dbAccountResponse = (await form.SelectedGroup.GetCosmosDBAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredCosmosName, accountcontent)).Value;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                CosmosDBSqlDatabaseResource dbResponse = (await dbAccountResponse.GetCosmosDBSqlDatabases().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.accountsContainerName[..^1], new(form.SelectedRegion, new(cosmosLibrary.accountsContainerName[..^1])))).Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                              //CosmosDBSqlContainerResource accountResponse = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, customAccountsContainerName, new(SelectedRegion, new(customAccountsContainerName)))).Value;
                              //CosmosDBSqlContainerResource counterResponse = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, customCounterContainerName, new(SelectedRegion, new(customCounterContainerName)))).Value;
                              //CosmosDBSqlContainerResource smsResponse = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, customSMSContainerName, new(SelectedRegion, new(customSMSContainerName)))).Value;
                              //CosmosDBSqlContainerResource whatsAppResponse = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, customWhatsAppContainerName, new(SelectedRegion, new(customWhatsAppContainerName)))).Value;
                CosmosDBContainerPartitionKey partKey = new() { Kind = CosmosDBPartitionKind.Hash };
                partKey.Paths.Add(cosmosLibrary.accountsIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.accountsContainerName, new(form.SelectedRegion, new(cosmosLibrary.accountsContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.countersIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.countersContainerName, new(form.SelectedRegion, new(cosmosLibrary.countersContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.smsIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.smsContainerName, new(form.SelectedRegion, new(cosmosLibrary.smsContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.whatsappIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.whatsappContainerName, new(form.SelectedRegion, new(cosmosLibrary.whatsappContainerName) { PartitionKey = partKey }))).Value;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Sorry, we are currently experiencing high demand in"))
                {
                    Globals.OpenLink("https://aka.ms/cosmosdbquota");
                    form.mb.label1.Text = "Error: Cosmos DB has not finished due to high demand.";
                    form.mb.richTextBox1.Text = "Run this again after fixing quota error link."
                        + Environment.NewLine + Environment.NewLine +
                        "Be sure to mention your requested name: " + desiredCosmosName
                        + Environment.NewLine + Environment.NewLine +
                        "Full error in case Microsoft needs it: " + Environment.NewLine + ex.Message;
                    form.mb.ShowDialog();
                    form.Close();
                }
            }
        }

        static async Task CreateCosmosARM(CosmosDeploy form, string cosmosJSONPath, string desiredCosmosAccountName, string subId, string virtualNetworkName, string subnetName, string customDBName = "SMSAndWhatsApp", string customAccountsContainerName = "SMSAndWhatsAppA", string customCounterContainerName = "SMSAndWhatsAppC", string customSMSContainerName = "SMSM", string customWhatsAppContainerName = "WhatsAppM", string currentschema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#")
        {
            var temp = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602
            JSONCosmos json = JsonSerializer.Deserialize<JSONCosmos>(File.ReadAllBytes(cosmosJSONPath), temp);
            string complexstring = "\"/\\\"_etag\\\"/?\"";
            complexstring = complexstring[1..];
            complexstring = complexstring[0..^1];
            json.resources[4].properties.resource.indexingPolicy.excludedPaths[0].path = complexstring;
            json.resources[5].properties.resource.indexingPolicy.excludedPaths[0].path = complexstring;
            json.resources[6].properties.resource.indexingPolicy.excludedPaths[0].path = complexstring;
            json.resources[7].properties.resource.indexingPolicy.excludedPaths[0].path = complexstring;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602
            var jsonString = JsonSerializer.Serialize(json);

            //fix schema
            jsonString = jsonString.Replace("schema", "$schema");
            jsonString = jsonString.Replace("schema\":null", "schema\":\"" + currentschema + "\"");

            //nulls
            jsonString = jsonString.Replace("\"hiddencosmosmmspecial\":null", "");
            jsonString = jsonString.Replace("\"resource\":null", "");
            jsonString = jsonString.Replace("\"roleName\":null,", "");
            jsonString = jsonString.Replace("\"type\":null,", "");
            jsonString = jsonString.Replace("\"assignableScopes\":null,", "");
            jsonString = jsonString.Replace("\"permissions\":null", "");
            jsonString = jsonString.Replace("\"dependsOn\":null", "");
            jsonString = jsonString.Replace("\"location\":null,", "");
            jsonString = jsonString.Replace("\"tags\":null,", "");
            jsonString = jsonString.Replace("\"kind\":null,", "\"");
            jsonString = jsonString.Replace("\"identity\":null,", "'");
            jsonString = jsonString.Replace("\"publicNetworkAccess\":null", "");
            jsonString = jsonString.Replace("\"enableAutomaticFailover\":null,", "");
            jsonString = jsonString.Replace("\"enableMultipleWriteLocations\":null,", "");
            jsonString = jsonString.Replace("\"isVirtualNetworkFilterEnabled\":null,", "");
            jsonString = jsonString.Replace("\"virtualNetworkRules\":null,", "");
            jsonString = jsonString.Replace("\"analyticalStorageConfiguration\":null,", "");
            jsonString = jsonString.Replace("\"databaseAccountOfferType\":null,", "");
            jsonString = jsonString.Replace("\"defaultIdentity\":null,", "");
            jsonString = jsonString.Replace("\"networkAclBypass\":null,", "");
            jsonString = jsonString.Replace("\"consistencyPolicy\":null,", "");
            jsonString = jsonString.Replace("\"locations\":null,", "");
            jsonString = jsonString.Replace("\"cors\":null,", "");
            jsonString = jsonString.Replace("\"capabilities\":null,", "");
            jsonString = jsonString.Replace("\"ipRules\":null,", "");
            jsonString = jsonString.Replace("\"backupPolicy\":null,", "");
            jsonString = jsonString.Replace("\"networkAclBypassResourceIds\":null,", "");
            jsonString = jsonString.Replace("\"capacity\":null,", "");
            jsonString = jsonString.Replace("\"keysMetadata\":null,", "");
            jsonString = jsonString.Replace("\"indexingPolicy\":null,", "");
            jsonString = jsonString.Replace("\"partitionKey\":null,", "");
            jsonString = jsonString.Replace("\"uniqueKeyPolicy\":null,", "");
            jsonString = jsonString.Replace("\"conflictResolutionPolicy\":null,", "");

            //trails
            jsonString = jsonString.Replace("\"keysMetadata\":{},,", "");
            jsonString = jsonString.Replace("\"'\"properties\":", "\"properties\":");
            jsonString = jsonString.Replace("{,", "{");
            jsonString = jsonString.Replace("\"enablePartitionMerge\":false,,", "\"enablePartitionMerge\":false,");
            jsonString = jsonString.Replace("(SQL)\",}", "(SQL)\"}");
            jsonString = jsonString.Replace("{\"totalThroughputLimit\":4000},},},{", "{\"totalThroughputLimit\":4000}}},{");
            jsonString = jsonString.Replace("ticalStorageTtl\":0},}", "ticalStorageTtl\":0}}");
            jsonString = jsonString.Replace("},}", "}}");

            jsonString = jsonString.Replace("SUBIDGOESHERE", subId);
            jsonString = jsonString.Replace("ACCOUNTNAMEGOESHERE", desiredCosmosAccountName);
            jsonString = jsonString.Replace("DATABASENAMEGOESHERE", customDBName);
            jsonString = jsonString.Replace("CUSTOMACCOUNTSCONTAINER", customAccountsContainerName);
            jsonString = jsonString.Replace("CUSTOMCOUNTERCONTAINER", customCounterContainerName);
            jsonString = jsonString.Replace("CUSTOMSMSCONTAINER", customSMSContainerName);
            jsonString = jsonString.Replace("CUSTOMWHATSAPPCONTAINER", customWhatsAppContainerName);
            jsonString = jsonString.Replace("PRIVATEVIRTUALNETWORKNAME", virtualNetworkName);
            jsonString = jsonString.Replace("PRIVATEVIRTUALSUBNETNAME", subnetName);

            //fixes escape characters
            jsonString = System.Text.RegularExpressions.Regex.Unescape(jsonString);
            //good up to this point
            //Console.WriteLine(jsonString);

            //dynamic d_json = JsonSerializer.Deserialize<dynamic>(jsonString, temp);
            //Console.WriteLine(d_json);

            ArmDeploymentContent armDeploymentContent = new(new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
            {
                Template = BinaryData.FromString(jsonString)
                //Parameters = BinaryData.FromString("")
            });
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "CosmosDeployment", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Cosmos Deployment Complete";
            }
            catch (Exception e)
            {
                form.OutputRT.Text += Environment.NewLine + e.Message;
            }
        }
    }
}
