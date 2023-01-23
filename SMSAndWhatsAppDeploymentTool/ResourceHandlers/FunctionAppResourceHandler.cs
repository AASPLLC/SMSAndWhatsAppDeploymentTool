using Azure.Core;
using Azure.ResourceManager.AppService.Models;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.Models;
using Azure;
using Azure.ResourceManager.WebPubSub;
using System.Text.Json;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Resources;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    public class FunctionAppResourceHandler
    {
        public static async Task<(WebSiteResource, WebSiteResource)> InitialCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, DataverseDeploy form)
        {
            WebSiteResource smsSiteResource = await DeepCheckSMS(appPlan, vnetIdentity, desiredStorageName, desiredSMSFunctionAppName, form);
            WebSiteResource whatsAppSiteResource = await DeepCheckWhatsApp(appPlan, vnetIdentity, desiredWhatsAppFunctionAppName, form);
            return (smsSiteResource, whatsAppSiteResource);
        }
        public static async Task<(WebSiteResource, WebSiteResource)> InitialCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredRestAppName, CosmosDeploy form)
        {
            WebSiteResource smsSiteResource = await DeepCheckSMS(appPlan, vnetIdentity, desiredStorageName, desiredSMSFunctionAppName, form);
            WebSiteResource whatsAppSiteResource = await DeepCheckWhatsApp(appPlan, vnetIdentity, desiredWhatsAppFunctionAppName, form);

            bool haveREST = false;
            foreach (var item in form.SelectedGroup.GetWebSites())
            {
                if (item.Data.Name.EndsWith("CosmosREST"))
                {
                    haveREST = true;
                }
            }
            if (!haveREST)
                _ = await InitialRESTCreation(appPlan, vnetIdentity, desiredRestAppName, form);

            //await DeployZips(smsSiteResource, whatsAppSiteResource, form);

            return (smsSiteResource, whatsAppSiteResource);
        }

        static async Task<WebSiteResource> InitialSMSCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredSMSFunctionAppName, string deisredStorageAccountName, DataverseDeploy form)
        {
            if (await CheckSMSFunctionAppName(desiredSMSFunctionAppName, form))
            {
                return await CreateSMSFunctionApp(deisredStorageAccountName, desiredSMSFunctionAppName, appPlan, vnetIdentity, form);
            }
            else
                return await SkipSMSFunctionApp(desiredSMSFunctionAppName, form);
        }
        static async Task<WebSiteResource> InitialSMSCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredSMSFunctionAppName, string deisredStorageAccountName, CosmosDeploy form)
        {
            if (await CheckSMSFunctionAppName(desiredSMSFunctionAppName, form))
            {
                return await CreateSMSFunctionApp(deisredStorageAccountName, desiredSMSFunctionAppName, appPlan, vnetIdentity, form);
            }
            else
                return await SkipSMSFunctionApp(desiredSMSFunctionAppName, form);
        }

        static async Task<WebSiteResource> InitialWhatsAppCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredWhatsAppFunctionAppName, DataverseDeploy form)
        {
            if (await CheckWhatsAppFunctionAppName(desiredWhatsAppFunctionAppName, form))
            {
                return await CreateWhatsAppFunctionApp(desiredWhatsAppFunctionAppName, appPlan, vnetIdentity, form);
            }
            else
                return await SkipWhatsAppFunctionApp(desiredWhatsAppFunctionAppName, form);
        }
        static async Task<WebSiteResource> InitialWhatsAppCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredWhatsAppFunctionAppName, CosmosDeploy form)
        {
            if (await CheckWhatsAppFunctionAppName(desiredWhatsAppFunctionAppName, form))
            {
                return await CreateWhatsAppFunctionApp(desiredWhatsAppFunctionAppName, appPlan, vnetIdentity, form);
            }
            else
                return await SkipWhatsAppFunctionApp(desiredWhatsAppFunctionAppName, form);
        }

        static async Task<WebSiteResource> InitialRESTCreation(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredWhatsAppFunctionAppName, CosmosDeploy form)
        {
            if (await CheckRESTFunctionAppName(desiredWhatsAppFunctionAppName, form))
            {
                return await CreateRESTFunctionApp(desiredWhatsAppFunctionAppName, appPlan, vnetIdentity, form);
            }
            else
                return await SkipRESTFunctionApp(desiredWhatsAppFunctionAppName, form);
        }

        static async Task<WebSiteResource> CreateSMSFunctionApp(string storagedesiredname, string desiredName, ResourceIdentifier appPlan, ResourceIdentifier vnetId, DataverseDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for SMS Function App Creation";
            SiteConfigProperties siteConfigProperties = new()
            {
                NumberOfWorkers = 1,
                UseManagedIdentityCreds = false,
                IsAlwaysOn = true,
                IsHttp20Enabled = false,
                FunctionAppScaleLimit = 0,
                MinimumElasticInstanceCount = 0
            };

            var functionAppResponse = await form.SelectedGroup.GetWebSites().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "SMSApp", new WebSiteData(form.SelectedRegion)
            {
                Kind = "functionapp",
                AppServicePlanId = appPlan,
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                IsEnabled = true,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                VirtualNetworkSubnetId = vnetId,
                KeyVaultReferenceIdentity = "SystemAssigned",
                SiteConfig = siteConfigProperties,
                IsScmSiteAlsoStopped = false,
                IsClientAffinityEnabled = false,
                IsClientCertEnabled = false,
                ClientCertMode = ClientCertMode.Required,
                IsHostNameDisabled = false,
                ContainerSize = 1536,
                DailyMemoryTimeQuota = 0,
                IsHttpsOnly = true,
                RedundancyMode = RedundancyMode.None,
                IsStorageAccountRequired = false,
            });

            var functionAppResource = functionAppResponse.Value;

            AppServiceVirtualNetworkData appServiceVirtualNetworkData = new()
            {
                IsSwift = true,
                VnetResourceId = vnetId
            };

            _ = await functionAppResource.GetSiteVirtualNetworkConnections().CreateOrUpdateAsync(WaitUntil.Completed, vnetId.Name, appServiceVirtualNetworkData);

            await StorageAccountResourceHandler.CreateStorageAccountNetworkRuleSet(form.SelectedGroup, form.SelectedRegion, vnetId, functionAppResponse.Value.Data.OutboundIPAddresses.Split(","), storagedesiredname);

            form.OutputRT.Text += Environment.NewLine + "Funcation app for SMS created";

            return functionAppResource;
        }
        static async Task<WebSiteResource> CreateSMSFunctionApp(string storagedesiredname, string desiredName, ResourceIdentifier appPlan, ResourceIdentifier vnetId, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for SMS Function App Creation";
            SiteConfigProperties siteConfigProperties = new()
            {
                NumberOfWorkers = 1,
                UseManagedIdentityCreds = false,
                IsAlwaysOn = true,
                IsHttp20Enabled = false,
                FunctionAppScaleLimit = 0,
                MinimumElasticInstanceCount = 0
            };

            var functionAppResponse = await form.SelectedGroup.GetWebSites().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "SMSApp", new WebSiteData(form.SelectedRegion)
            {
                Kind = "functionapp",
                AppServicePlanId = appPlan,
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                IsEnabled = true,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                VirtualNetworkSubnetId = vnetId,
                KeyVaultReferenceIdentity = "SystemAssigned",
                SiteConfig = siteConfigProperties,
                IsScmSiteAlsoStopped = false,
                IsClientAffinityEnabled = false,
                IsClientCertEnabled = false,
                ClientCertMode = ClientCertMode.Required,
                IsHostNameDisabled = false,
                ContainerSize = 1536,
                DailyMemoryTimeQuota = 0,
                IsHttpsOnly = true,
                RedundancyMode = RedundancyMode.None,
                IsStorageAccountRequired = false,
            });

            var functionAppResource = functionAppResponse.Value;

            AppServiceVirtualNetworkData appServiceVirtualNetworkData = new()
            {
                IsSwift = true,
                VnetResourceId = vnetId
            };

            _ = await functionAppResource.GetSiteVirtualNetworkConnections().CreateOrUpdateAsync(WaitUntil.Completed, vnetId.Name, appServiceVirtualNetworkData);

            await StorageAccountResourceHandler.CreateStorageAccountNetworkRuleSet(form.SelectedGroup, form.SelectedRegion, vnetId, functionAppResponse.Value.Data.OutboundIPAddresses.Split(","), storagedesiredname);

            form.OutputRT.Text += Environment.NewLine + "Funcation app for SMS created";

            return functionAppResource;
        }

        static async Task<WebSiteResource> CreateWhatsAppFunctionApp(string desiredName, ResourceIdentifier appPlan, ResourceIdentifier vnetId, DataverseDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for WhatsApp Function App Creation";
            SiteConfigProperties siteConfigProperties = new()
            {
                NumberOfWorkers = 1,
                UseManagedIdentityCreds = false,
                IsAlwaysOn = true,
                IsHttp20Enabled = false,
                FunctionAppScaleLimit = 0,
                MinimumElasticInstanceCount = 0
            };

            var functionAppResponse = await form.SelectedGroup.GetWebSites().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "WhatsApp", new WebSiteData(form.SelectedRegion)
            {
                Kind = "functionapp",
                AppServicePlanId = appPlan,
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                IsEnabled = true,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                VirtualNetworkSubnetId = vnetId,
                KeyVaultReferenceIdentity = "SystemAssigned",
                SiteConfig = siteConfigProperties,
                IsScmSiteAlsoStopped = false,
                IsClientAffinityEnabled = false,
                IsClientCertEnabled = false,
                ClientCertMode = ClientCertMode.Required,
                IsHostNameDisabled = false,
                ContainerSize = 1536,
                DailyMemoryTimeQuota = 0,
                IsHttpsOnly = true,
                RedundancyMode = RedundancyMode.None,
                IsStorageAccountRequired = false,
            });

            var functionAppResource = functionAppResponse.Value;

            form.OutputRT.Text += Environment.NewLine + "Funcation app for WhatsApp created";

            return functionAppResource;
        }
        static async Task<WebSiteResource> CreateWhatsAppFunctionApp(string desiredName, ResourceIdentifier appPlan, ResourceIdentifier vnetId, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for WhatsApp Function App Creation";
            SiteConfigProperties siteConfigProperties = new()
            {
                NumberOfWorkers = 1,
                UseManagedIdentityCreds = false,
                IsAlwaysOn = true,
                IsHttp20Enabled = false,
                FunctionAppScaleLimit = 0,
                MinimumElasticInstanceCount = 0
            };

            var functionAppResponse = await form.SelectedGroup.GetWebSites().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "WhatsApp", new WebSiteData(form.SelectedRegion)
            {
                Kind = "functionapp",
                AppServicePlanId = appPlan,
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                IsEnabled = true,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                VirtualNetworkSubnetId = vnetId,
                KeyVaultReferenceIdentity = "SystemAssigned",
                SiteConfig = siteConfigProperties,
                IsScmSiteAlsoStopped = false,
                IsClientAffinityEnabled = false,
                IsClientCertEnabled = false,
                ClientCertMode = ClientCertMode.Required,
                IsHostNameDisabled = false,
                ContainerSize = 1536,
                DailyMemoryTimeQuota = 0,
                IsHttpsOnly = true,
                RedundancyMode = RedundancyMode.None,
                IsStorageAccountRequired = false,
            });

            var functionAppResource = functionAppResponse.Value;

            form.OutputRT.Text += Environment.NewLine + "Funcation app for WhatsApp created";

            return functionAppResource;
        }

        static async Task<WebSiteResource> CreateRESTFunctionApp(string desiredName, ResourceIdentifier appPlan, ResourceIdentifier vnetId, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Cosmos REST API Function App Creation";
            SiteConfigProperties siteConfigProperties = new()
            {
                NumberOfWorkers = 1,
                UseManagedIdentityCreds = false,
                IsAlwaysOn = true,
                IsHttp20Enabled = false,
                FunctionAppScaleLimit = 0,
                MinimumElasticInstanceCount = 0
            };

            var functionAppResponse = await form.SelectedGroup.GetWebSites().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "CosmosREST", new WebSiteData(form.SelectedRegion)
            {
                Kind = "functionapp",
                AppServicePlanId = appPlan,
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                IsEnabled = true,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                VirtualNetworkSubnetId = vnetId,
                KeyVaultReferenceIdentity = "SystemAssigned",
                SiteConfig = siteConfigProperties,
                IsScmSiteAlsoStopped = false,
                IsClientAffinityEnabled = false,
                IsClientCertEnabled = false,
                ClientCertMode = ClientCertMode.Required,
                IsHostNameDisabled = false,
                ContainerSize = 1536,
                DailyMemoryTimeQuota = 0,
                IsHttpsOnly = true,
                RedundancyMode = RedundancyMode.None,
                IsStorageAccountRequired = false,
            });

            var functionAppResource = functionAppResponse.Value;

            form.OutputRT.Text += Environment.NewLine + "Funcation app for Cosmos REST API created";

            return functionAppResource;
        }

        static async Task<WebSiteResource> SkipSMSFunctionApp(string desiredName, DataverseDeploy form)
        {
            return await form.SelectedGroup.GetWebSiteAsync(desiredName + "SMSApp");
        }
        static async Task<WebSiteResource> SkipSMSFunctionApp(string desiredName, CosmosDeploy form)
        {
            return await form.SelectedGroup.GetWebSiteAsync(desiredName + "SMSApp");
        }

        static async Task<WebSiteResource> SkipWhatsAppFunctionApp(string desiredName, DataverseDeploy form)
        {
            return await form.SelectedGroup.GetWebSiteAsync(desiredName + "WhatsApp");
        }
        static async Task<WebSiteResource> SkipWhatsAppFunctionApp(string desiredName, CosmosDeploy form)
        {
            return await form.SelectedGroup.GetWebSiteAsync(desiredName + "WhatsApp");
        }

        static async Task<WebSiteResource> SkipRESTFunctionApp(string desiredName, CosmosDeploy form)
        {
            return await form.SelectedGroup.GetWebSiteAsync(desiredName + "CosmosREST");
        }

        static async Task<bool> CheckSMSFunctionAppName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetWebSiteAsync(desiredName + "SMSApp");
                form.OutputRT.Text += Environment.NewLine + desiredName + "SMSApp" + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex)
            {
                if (desiredName != "")
                {
                    if (ex.Status == 404)
                    {
                        var nameResponse = await form.SelectedSubscription.CheckWebPubSubNameAvailabilityAsync(form.SelectedRegion, new Azure.ResourceManager.WebPubSub.Models.WebPubSubNameAvailabilityContent("Microsoft.SignalRService/webPubSub", desiredName));

                        if (nameResponse.Value.NameAvailable == true)
                        {
                            return true;
                        }
                        else
                        {
                            form.OutputRT.Text += Environment.NewLine + "Funcation name for SMS App has already been taken, try another.";
                            return false;
                        }
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Unknown Error:" + Environment.NewLine + ex.Message;
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Funcation name for SMS App is empty.";
                    return false;
                }
            }
        }
        static async Task<bool> CheckSMSFunctionAppName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetWebSiteAsync(desiredName + "SMSApp");
                form.OutputRT.Text += Environment.NewLine + desiredName + "SMSApp" + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex)
            {
                if (desiredName != "")
                {
                    if (ex.Status == 404)
                    {
                        var nameResponse = await form.SelectedSubscription.CheckWebPubSubNameAvailabilityAsync(form.SelectedRegion, new Azure.ResourceManager.WebPubSub.Models.WebPubSubNameAvailabilityContent("Microsoft.SignalRService/webPubSub", desiredName));

                        if (nameResponse.Value.NameAvailable == true)
                        {
                            return true;
                        }
                        else
                        {
                            form.OutputRT.Text += Environment.NewLine + "Funcation name for SMS App has already been taken, try another.";
                            return false;
                        }
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Unknown Error:" + Environment.NewLine + ex.Message;
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Funcation name for SMS App is empty.";
                    return false;
                }
            }
        }

        static async Task<bool> CheckWhatsAppFunctionAppName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetWebSiteAsync(desiredName + "WhatsApp");
                form.OutputRT.Text += Environment.NewLine + desiredName + "WhatsApp" + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckWebPubSubNameAvailabilityAsync(form.SelectedRegion, new Azure.ResourceManager.WebPubSub.Models.WebPubSubNameAvailabilityContent("Microsoft.SignalRService/webPubSub", desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Funcation name for WhatsApp has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Funcation name for WhatsApp App is empty.";
                    return false;
                }
            }
        }
        static async Task<bool> CheckWhatsAppFunctionAppName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetWebSiteAsync(desiredName + "WhatsApp");
                form.OutputRT.Text += Environment.NewLine + desiredName + "WhatsApp" + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckWebPubSubNameAvailabilityAsync(form.SelectedRegion, new Azure.ResourceManager.WebPubSub.Models.WebPubSubNameAvailabilityContent("Microsoft.SignalRService/webPubSub", desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Funcation name for WhatsApp has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Funcation name for WhatsApp App is empty.";
                    return false;
                }
            }
        }

        static async Task<bool> CheckRESTFunctionAppName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetWebSiteAsync(desiredName + "CosmosREST");
                form.OutputRT.Text += Environment.NewLine + desiredName + "CosmosREST" + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckWebPubSubNameAvailabilityAsync(form.SelectedRegion, new Azure.ResourceManager.WebPubSub.Models.WebPubSubNameAvailabilityContent("Microsoft.SignalRService/webPubSub", desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Funcation name for Cosmos REST API has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Funcation name for Cosmos REST API App is empty.";
                    return false;
                }
            }
        }

        static async Task<WebSiteResource> DeepCheckSMS(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredStorageName, string desiredSMSFunctionAppName, DataverseDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetWebSites())
            {
                if (item.Data.Name.EndsWith("SMSApp"))
                {
                    return item;
                }
            }
            WebSiteResource smsSiteResource = await InitialSMSCreation(appPlan, vnetIdentity, desiredSMSFunctionAppName, desiredStorageName, form);

            //make sure IAM is setup for storage account
            if (smsSiteResource.Data.Identity.PrincipalId != null)
                await CreateStorageRoleAccessARM(desiredStorageName, smsSiteResource.Data.Identity.PrincipalId.Value.ToString(), form);

            return smsSiteResource;
        }
        static async Task<WebSiteResource> DeepCheckSMS(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredStorageName, string desiredSMSFunctionAppName, CosmosDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetWebSites())
            {
                if (item.Data.Name.EndsWith("SMSApp"))
                {
                    return item;
                }
            }
            WebSiteResource smsSiteResource = await InitialSMSCreation(appPlan, vnetIdentity, desiredSMSFunctionAppName, desiredStorageName, form);

            //make sure IAM is setup for storage account
            if (smsSiteResource.Data.Identity.PrincipalId != null)
                await CreateStorageRoleAccessARM(desiredStorageName, smsSiteResource.Data.Identity.PrincipalId.Value.ToString(), form);

            return smsSiteResource;
        }

        static async Task<WebSiteResource> DeepCheckWhatsApp(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredWhatsAppFunctionAppName, DataverseDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetWebSites())
            {
                if (item.Data.Name.EndsWith("WhatsApp"))
                {
                    return item;
                }
            }
            return await InitialWhatsAppCreation(appPlan, vnetIdentity, desiredWhatsAppFunctionAppName, form);
        }
        static async Task<WebSiteResource> DeepCheckWhatsApp(ResourceIdentifier appPlan, ResourceIdentifier vnetIdentity, string desiredWhatsAppFunctionAppName, CosmosDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetWebSites())
            {
                if (item.Data.Name.EndsWith("WhatsApp"))
                {
                    return item;
                }
            }
            return await InitialWhatsAppCreation(appPlan, vnetIdentity, desiredWhatsAppFunctionAppName, form);
        }

        static async Task CreateStorageRoleAccessARM(string storageName, string smsId, DataverseDeploy form)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            form.OutputRT.Text += Environment.NewLine + "Creating IAM Access to storage acccount for SMS Function App.";
            var temp = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            var JSONData = JsonSerializer.Deserialize<JSONRoleAssignments>(File.ReadAllBytes(Environment.CurrentDirectory + "/JSONS/StorageRoleAccess.json"), temp);

            JSONData.parameters.role.defaultValue = "c12c1c16-33a1-487b-954d-41c89c60f349";
            JSONData.parameters.storageAccounts.defaultValue = storageName;
            JSONData.parameters.prin.defaultValue = smsId;

            JSONData.resources[0].location = form.SelectedRegion;
            JSONData.resources[1].scope = "Microsoft.Storage/storageAccounts/" + storageName;

            var jsonString = JsonSerializer.Serialize(JSONData);

            //fixes schema
            jsonString = jsonString.Replace("schema", "$schema");
            jsonString = jsonString.Replace("schema\":null", "schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\"");

            //since these are arrays, sometimes values are needed and sometimes they are not.
            jsonString = jsonString.Replace("\"roleDefinitionId\":null,", "");
            jsonString = jsonString.Replace("\"principalId\":null", "");
            jsonString = jsonString.Replace("\"parameters\":null,", "");
            jsonString = jsonString.Replace("\"variables\":null,", "");
            jsonString = jsonString.Replace("\"scope\":null,", "");
            jsonString = jsonString.Replace("\"dependsOn\":null,", "");
            jsonString = jsonString.Replace("\"location\":null,", "");
            jsonString = jsonString.Replace("\"sku\":null,", "");
            jsonString = jsonString.Replace("\"kind\":null,", "");
            jsonString = jsonString.Replace("\",\"dependsOn\":null", "\"");
            jsonString = jsonString.Replace("\u0027", "'");
            jsonString = jsonString.Replace(",\"dependsOn\":null", "");
            jsonString = jsonString.Replace(",\"dependsOn\":[\"\"]", "");

            //fixes escape characters
            jsonString = System.Text.RegularExpressions.Regex.Unescape(jsonString);

            ArmDeploymentContent armDeploymentContent = new(new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
            {
                Template = BinaryData.FromString(jsonString)
                //Parameters = BinaryData.FromString("")
            });
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Reader and Data Access.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Reader and Data Access access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }

            jsonString = jsonString.Replace("c12c1c16-33a1-487b-954d-41c89c60f349", "17d1049b-9a84-46fb-8f53-869881c3d3ab");
            armDeploymentContent.Properties.Template = BinaryData.FromString(jsonString);
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Storage Account Contributor.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Storage Account Contributor access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }

            jsonString = jsonString.Replace("17d1049b-9a84-46fb-8f53-869881c3d3ab", "974c5e8b-45b9-4653-ba55-5f855dd0fb88");
            armDeploymentContent.Properties.Template = BinaryData.FromString(jsonString);
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Storage Queue Data Contributor.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Storage Queue Data Contributor access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        static async Task CreateStorageRoleAccessARM(string storageName, string smsId, CosmosDeploy form)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            form.OutputRT.Text += Environment.NewLine + "Creating IAM Access to storage acccount for SMS Function App.";
            var temp = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            var JSONData = JsonSerializer.Deserialize<JSONRoleAssignments>(File.ReadAllBytes(Environment.CurrentDirectory + "/JSONS/StorageRoleAccess.json"), temp);

            JSONData.parameters.role.defaultValue = "c12c1c16-33a1-487b-954d-41c89c60f349";
            JSONData.parameters.storageAccounts.defaultValue = storageName;
            JSONData.parameters.prin.defaultValue = smsId;

            JSONData.resources[0].location = form.SelectedRegion;
            JSONData.resources[1].scope = "Microsoft.Storage/storageAccounts/" + storageName;

            var jsonString = JsonSerializer.Serialize(JSONData);

            //fixes schema
            jsonString = jsonString.Replace("schema", "$schema");
            jsonString = jsonString.Replace("schema\":null", "schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\"");

            //since these are arrays, sometimes values are needed and sometimes they are not.
            jsonString = jsonString.Replace("\"roleDefinitionId\":null,", "");
            jsonString = jsonString.Replace("\"principalId\":null", "");
            jsonString = jsonString.Replace("\"parameters\":null,", "");
            jsonString = jsonString.Replace("\"variables\":null,", "");
            jsonString = jsonString.Replace("\"scope\":null,", "");
            jsonString = jsonString.Replace("\"dependsOn\":null,", "");
            jsonString = jsonString.Replace("\"location\":null,", "");
            jsonString = jsonString.Replace("\"sku\":null,", "");
            jsonString = jsonString.Replace("\"kind\":null,", "");
            jsonString = jsonString.Replace("\",\"dependsOn\":null", "\"");
            jsonString = jsonString.Replace("\u0027", "'");
            jsonString = jsonString.Replace(",\"dependsOn\":null", "");
            jsonString = jsonString.Replace(",\"dependsOn\":[\"\"]", "");

            //fixes escape characters
            jsonString = System.Text.RegularExpressions.Regex.Unescape(jsonString);

            ArmDeploymentContent armDeploymentContent = new(new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
            {
                Template = BinaryData.FromString(jsonString)
                //Parameters = BinaryData.FromString("")
            });
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Reader and Data Access.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Reader and Data Access access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }

            jsonString = jsonString.Replace("c12c1c16-33a1-487b-954d-41c89c60f349", "17d1049b-9a84-46fb-8f53-869881c3d3ab");
            armDeploymentContent.Properties.Template = BinaryData.FromString(jsonString);
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Storage Account Contributor.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Storage Account Contributor access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }

            jsonString = jsonString.Replace("17d1049b-9a84-46fb-8f53-869881c3d3ab", "974c5e8b-45b9-4653-ba55-5f855dd0fb88");
            armDeploymentContent.Properties.Template = BinaryData.FromString(jsonString);
            try
            {
                await form.SelectedGroup.GetArmDeployments().CreateOrUpdateAsync(WaitUntil.Completed, "StorageAccountAccess", armDeploymentContent);
                form.OutputRT.Text += Environment.NewLine + "Added Storage Queue Data Contributor.";
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The role assignment already exists"))
                    form.OutputRT.Text += Environment.NewLine + "Storage Queue Data Contributor access already exists in IAM, skipping";
                else
                    form.OutputRT.Text += Environment.NewLine + e.Message;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
