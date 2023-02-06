using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using System.Text.Json;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.CosmosDB;
using Azure.Core;
using AASPGlobalLibrary;
using Azure.ResourceManager.AppService;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    //keep ARM plus Management way for security team
    internal class CosmosResourceHandler
    {
        internal static bool found = false;
        internal virtual async Task InitialCreation(JSONDefaultCosmosLibrary cosmosLibrary, ResourceIdentifier subnetID, string DBName, string desiredCosmosName, string vnetName, CosmosDeploy form, bool useArm = false)
        {
            //if (useArm)
                //await CreateCosmosARM(form, DBName, Environment.CurrentDirectory + @"\JSONS\CosmosDeploy.json", desiredCosmosName, form.SelectedSubscription.Data.SubscriptionId, vnetName, subnetID.Name);
            //else
                await CreateCosmosDB(cosmosLibrary, subnetID, DBName, desiredCosmosName, form);
        }

        static async Task CreateCosmosDB(JSONDefaultCosmosLibrary cosmosLibrary, ResourceIdentifier subnetID, string DBName, string desiredCosmosName, CosmosDeploy form)
        {
            List<CosmosDBAccountLocation> Locations = new();
            CosmosDBAccountLocation locationstuff = new()
            {
                LocationName = form.SelectedRegion,
                IsZoneRedundant = false
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
                var item = form.SelectedGroup.GetCosmosDBAccount(desiredCosmosName).Value;
                form.OutputRT.Text += Environment.NewLine + item.Data.Name + "already exists in your environment, skipping.";
            }
            //can be improved?
            /*foreach (var item in form.SelectedGroup.GetCosmosDBAccounts())
            {
                CosmosDBSqlDatabaseResource dbResponse = (await item.GetCosmosDBSqlDatabases().CreateOrUpdateAsync(WaitUntil.Completed, DBName, new(form.SelectedRegion, new(DBName)))).Value;
                CosmosDBContainerPartitionKey partKey = new() { Kind = CosmosDBPartitionKind.Hash };
                partKey.Paths.Add(cosmosLibrary.accountsIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.accountsContainerName, new(form.SelectedRegion, new(cosmosLibrary.accountsContainerName) { PartitionKey = partKey }))).Value;
                //CosmosDBSqlContainerResource sql = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.accountsContainerName, new(form.SelectedRegion, new(cosmosLibrary.accountsContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.countersIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.countersContainerName, new(form.SelectedRegion, new(cosmosLibrary.countersContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.smsIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.smsContainerName, new(form.SelectedRegion, new(cosmosLibrary.smsContainerName) { PartitionKey = partKey }))).Value;
                partKey.Paths.Clear();
                partKey.Paths.Add(cosmosLibrary.whatsappIDName);
                _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(WaitUntil.Completed, cosmosLibrary.whatsappContainerName, new(form.SelectedRegion, new(cosmosLibrary.whatsappContainerName) { PartitionKey = partKey }))).Value;
                if (item.Data.ProvisioningState.Contains("Failed"))
                {
                    MessageBox2 mb = new();
                    mb.label1.Text = "Error: Unknown";
                    mb.richTextBox1.Text = "Provisioning state is in failed status, but there is no description for the error."
                        + Environment.NewLine +
                        "The most likely cause is due to quota limits. Please see https://aka.ms/cosmosdbquota"
                        + Environment.NewLine + Environment.NewLine +
                        "Desired Cosmos Name: " + desiredCosmosName;
                    mb.ShowDialog();
                    mb.Close();
                }
                else
                    form.OutputRT.Text += Environment.NewLine + item.Data.Name + "already exists in your environment, skipping.";
                found = true;
                break;
            }
            if (!found)*/
            catch
            {
                try
                {
                    CosmosDBAccountResource dbAccountResponse = (await form.SelectedGroup.GetCosmosDBAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredCosmosName, accountcontent)).Value;
                    CosmosDBSqlDatabaseResource dbResponse = (await dbAccountResponse.GetCosmosDBSqlDatabases().CreateOrUpdateAsync(WaitUntil.Completed, DBName, new(form.SelectedRegion, new(DBName)))).Value;
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
                        MessageBox2 mb = new();
                        mb.label1.Text = "Error: Cosmos DB has not finished due to high demand.";
                        mb.richTextBox1.Text = "Run this again after fixing quota error link."
                            + Environment.NewLine + Environment.NewLine +
                            "Be sure to mention your requested name: " + desiredCosmosName
                            + Environment.NewLine + Environment.NewLine +
                            "Full error in case Microsoft needs it: " + Environment.NewLine + ex.Message;
                        mb.ShowDialog();
                        mb.Close();
                    }
                    else
                    {
                        MessageBox2 mb = new();
                        mb.label1.Text = "Error: Cosmos DB has an unknown error.";
                        mb.richTextBox1.Text = ex.Message;
                        mb.ShowDialog();
                        mb.Close();
                    }
                }
            }
        }

        /*static async Task CreateCosmosARM(CosmosDeploy form, string DBName, string cosmosJSONPath, string desiredCosmosAccountName, string subId, string virtualNetworkName, string subnetName, string customAccountsContainerName = "SMSAndWhatsAppA", string customCounterContainerName = "SMSAndWhatsAppC", string customSMSContainerName = "SMSM", string customWhatsAppContainerName = "WhatsAppM", string currentschema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#")
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
            jsonString = jsonString.Replace("DATABASENAMEGOESHERE", DBName);
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
        }*/
    }
}
