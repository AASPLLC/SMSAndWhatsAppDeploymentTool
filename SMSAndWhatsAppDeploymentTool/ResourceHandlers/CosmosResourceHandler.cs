using Azure;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.CosmosDB;
using Azure.Core;
using AASPGlobalLibrary;
using Azure.ResourceManager.Resources;
using SMSAndWhatsAppDeploymentTool.StepByStep;
using Azure.ResourceManager.Network;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    //keep ARM plus Management way for security team
    internal class CosmosResourceHandler
    {
        static string GetDesiredCosmosName(string desiredCosmosName, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetCosmosDBAccounts())
            {
                desiredCosmosName = item.Data.Name;
                break;
            }
            return desiredCosmosName;
        }

        internal static bool found = false;
        internal virtual async Task<bool> InitialCreation(string desiredCosmosName, StepByStepValues sbs)
        {
#pragma warning disable CS8604
            desiredCosmosName = GetDesiredCosmosName(desiredCosmosName, sbs.SelectedGroup);
#pragma warning restore CS8604
            //if (useArm)
            //await CreateCosmosARM(form, DBName, Environment.CurrentDirectory + @"\JSONS\CosmosDeploy.json", desiredCosmosName, form.SelectedSubscription.Data.SubscriptionId, vnetName, subnetID.Name);
            //else
            if (sbs.secretNames.DbName != null)
                await CreateCosmosDB(sbs.secretNames.DbName, desiredCosmosName, sbs);
            sbs.DesiredCosmosAccount = desiredCosmosName;
            if (desiredCosmosName == "")
                return false;
            else
            {
                await sbs.CreateCosmosSecret(desiredCosmosName);
                return true;
            }
        }
        internal virtual async Task InitialCreation(JSONDefaultCosmosLibrary cosmosLibrary, ResourceIdentifier subnetID, string DBName, string desiredCosmosName, string vnetName, CosmosDeploy form, bool useArm = false)
        {
            desiredCosmosName = GetDesiredCosmosName(desiredCosmosName, form.SelectedGroup);
            //if (useArm)
            //await CreateCosmosARM(form, DBName, Environment.CurrentDirectory + @"\JSONS\CosmosDeploy.json", desiredCosmosName, form.SelectedSubscription.Data.SubscriptionId, vnetName, subnetID.Name);
            //else
            await CreateCosmosDB(cosmosLibrary, subnetID, DBName, desiredCosmosName, form);
        }

        static async Task AddContainer(CosmosDBSqlDatabaseResource dbResponse, string idName, string containerName, AzureLocation SelectedRegion)
        {
            CosmosDBContainerPartitionKey partKey = new() { Kind = CosmosDBPartitionKind.Hash };
            partKey.Paths.Add(idName);
            _ = (await dbResponse.GetCosmosDBSqlContainers().CreateOrUpdateAsync(
                WaitUntil.Completed,
                containerName,
                new(SelectedRegion,
                new(containerName)
                { PartitionKey = partKey }))).Value;
        }
        static async Task CreateCosmosDB(string DBName, string desiredCosmosName, StepByStepValues sbs)
        {
            JSONDefaultCosmosLibrary cosmosLibrary = await JSONDefaultCosmosLibrary.Load();
            List<CosmosDBAccountLocation> Locations = new();
            CosmosDBAccountLocation locationstuff = new()
            {
                LocationName = sbs.SelectedRegion,
                IsZoneRedundant = false
            };
            Locations.Add(locationstuff);
            CosmosDBAccountCreateOrUpdateContent accountcontent = new(sbs.SelectedRegion, Locations)
            {
                PublicNetworkAccess = CosmosDBPublicNetworkAccess.Enabled,
                IsVirtualNetworkFilterEnabled = true
            };
            ResourceIdentifier subnetID = (await (await sbs.SelectedGroup.GetVirtualNetworkAsync("StorageConnection")).Value.GetSubnetAsync("RestAPIToCosmos")).Value.Id;
            accountcontent.VirtualNetworkRules.Add(new()
            {
                Id = subnetID,
                IgnoreMissingVnetServiceEndpoint = true
            });
            Console.Write(Environment.NewLine + "Virtual network prepped for cosmos");
            try
            {
                var item = sbs.SelectedGroup.GetCosmosDBAccount(desiredCosmosName).Value;
                Console.Write(Environment.NewLine + item.Data.Name + "already exists in your environment, skipping.");
            }
            catch
            {
                Console.Write(Environment.NewLine + "Creating or Updating Cosmos");
                try
                {
                    CosmosDBAccountResource dbAccountResponse = (await sbs.SelectedGroup.GetCosmosDBAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredCosmosName, accountcontent)).Value;
                    Console.Write(Environment.NewLine + "Account created");
                    CosmosDBSqlDatabaseResource dbResponse = (await dbAccountResponse.GetCosmosDBSqlDatabases().CreateOrUpdateAsync(WaitUntil.Completed, DBName, new(sbs.SelectedRegion, new(DBName)))).Value;
                    Console.Write(Environment.NewLine + "Main DB created");
#pragma warning disable CS8604
                    await AddContainer(dbResponse, cosmosLibrary?.accountsIDName, cosmosLibrary?.accountsContainerName, sbs.SelectedRegion);
                    Console.Write(Environment.NewLine + "Accounts Container created");
                    await AddContainer(dbResponse, cosmosLibrary?.countersIDName, cosmosLibrary?.countersContainerName, sbs.SelectedRegion);
                    Console.Write(Environment.NewLine + "Counters Container created");
                    await AddContainer(dbResponse, cosmosLibrary?.smsIDName, cosmosLibrary?.smsContainerName, sbs.SelectedRegion);
                    Console.Write(Environment.NewLine + "SMS Container created");
                    await AddContainer(dbResponse, cosmosLibrary?.whatsappIDName, cosmosLibrary?.whatsappContainerName, sbs.SelectedRegion);
                    Console.Write(Environment.NewLine + "WhatsApp Container created");
                    await AddContainer(dbResponse, cosmosLibrary?.phoneIDName, cosmosLibrary?.phoneContainerName, sbs.SelectedRegion);
                    Console.Write(Environment.NewLine + "Phone Container created");
                    //keeping in case whatsapp api changes, this might be required some day
                    //await AddContainer(dbResponse, cosmosLibrary?.whatsappphoneIDName, cosmosLibrary?.whatsappphoneContainerName, sbs.SelectedRegion);
                    //Console.Write("WhatsApp Phone Container created");
                    Console.Write(Environment.NewLine + "Finished");
#pragma warning restore CS8604
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
                    else if(ex.Message.Contains("Value cannot be an empty string. (Parameter 'accountName')"))
                    {
                        Console.Write(Environment.NewLine + "Desired account name is empty and existing service not found.");
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
            form.OutputRT.Text += Environment.NewLine + "Virtual network prepped for cosmos";
            try
            {
                var item = form.SelectedGroup.GetCosmosDBAccount(desiredCosmosName).Value;
                form.OutputRT.Text += Environment.NewLine + item.Data.Name + "already exists in your environment, skipping.";
            }
            catch
            {
                form.OutputRT.Text += Environment.NewLine + "Creating or Updating Cosmos";
                try
                {
                    CosmosDBAccountResource dbAccountResponse = (await form.SelectedGroup.GetCosmosDBAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredCosmosName, accountcontent)).Value;
                    form.OutputRT.Text += Environment.NewLine + "Cosmos Account created";
                    CosmosDBSqlDatabaseResource dbResponse = (await dbAccountResponse.GetCosmosDBSqlDatabases().CreateOrUpdateAsync(WaitUntil.Completed, DBName, new(form.SelectedRegion, new(DBName)))).Value;
                    form.OutputRT.Text += Environment.NewLine + "Cosmos Main DB created";
#pragma warning disable CS8604
                    await AddContainer(dbResponse, cosmosLibrary?.accountsIDName, cosmosLibrary?.accountsContainerName, form.SelectedRegion);
                    form.OutputRT.Text += Environment.NewLine + "Cosmos Accounts Container created";
                    await AddContainer(dbResponse, cosmosLibrary?.countersIDName, cosmosLibrary?.countersContainerName, form.SelectedRegion);
                    form.OutputRT.Text += Environment.NewLine + "Cosmos Counters Container created";
                    await AddContainer(dbResponse, cosmosLibrary?.smsIDName, cosmosLibrary?.smsContainerName, form.SelectedRegion);
                    form.OutputRT.Text += Environment.NewLine + "Cosmos SMS Container created";
                    await AddContainer(dbResponse, cosmosLibrary?.whatsappIDName, cosmosLibrary?.whatsappContainerName, form.SelectedRegion);
                    form.OutputRT.Text += Environment.NewLine + "Cosmos WhatsApp Container created";
                    await AddContainer(dbResponse, cosmosLibrary?.phoneIDName, cosmosLibrary?.phoneContainerName, form.SelectedRegion);
                    form.OutputRT.Text += Environment.NewLine + "Cosmos Phone Container created";
                    //keeping in case whatsapp api changes, this might be required some day
                    //await AddContainer(dbResponse, cosmosLibrary?.whatsappphoneIDName, cosmosLibrary?.whatsappphoneContainerName, form.SelectedRegion);
                    //form.OutputRT.Text += "Cosmos WhatsApp Phone Container created";
#pragma warning restore CS8604
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
