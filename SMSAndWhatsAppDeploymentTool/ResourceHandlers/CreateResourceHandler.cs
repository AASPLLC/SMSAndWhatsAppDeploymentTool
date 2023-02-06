using AASPGlobalLibrary;
using Azure.Core;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.Communication;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Microsoft.PowerPlatform.Dataverse.Client;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager.Authorization.Models;
using Microsoft.Extensions.Azure;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
#pragma warning disable CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    internal class CreateResourceHandler
    {
        static string GetDesiredCommsName(string desiredCommsname, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetCommunicationServiceResources())
            {
                desiredCommsname = item.Data.Name;
                break;
            }
            return desiredCommsname;
        }
        static string GetDesiredStorageName(string desiredStorageName, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetStorageAccounts())
            {
                desiredStorageName = item.Data.Name;
                break;
            }
            return desiredStorageName;
        }
        static async Task<(string, string)> GetDesiredKeyVaultName(string desiredPublicVault, string desiredInternalVault, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetVaults())
            {
                try
                {
                    var test = (await item.GetSecretAsync("TenantID")).Value;
                    desiredInternalVault = item.Data.Name;
                    //Console.Write(Environment.NewLine + "Internal Vault Found: " + desiredInternalVault);
                }
                catch
                {
                    var test = (await item.GetSecretAsync("SmsEndpoint")).Value;
                    desiredPublicVault = item.Data.Name;
                    //Console.Write(Environment.NewLine + "Public Vault Found: " + desiredPublicVault);
                }
            }
            return (desiredPublicVault, desiredInternalVault);
        }
        static string GetDesiredCosmosName(string desiredCosmosName, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetCosmosDBAccounts())
            {
                desiredCosmosName = item.Data.Name;
                break;
            }
            return desiredCosmosName;
        }

        static async Task CreateDatabases(DataverseHandler dh, string clientid, string[] databases, DataverseDeploy form)
        {
            bool finished = false;
            string environmenturl = "https://" + form.SelectedEnvironment + ".crm.dynamics.com";
            //might change depending on what happens with system account creation, needs to be re-assigned
            var connectionString = @"AuthType='OAuth'; Username='" +
                await TokenHandler.JwtGetUsersInfo.GetUsersEmail() +
                "'; Password='passcode'; Url='" +
                environmenturl +
                "'; AppId='" +
                clientid +
                "'; RedirectUri='http://localhost:65135'; LoginPrompt='Auto'";
            while (!finished)
            {
                try
                {
                    ServiceClient service = new(connectionString);
                    await dh.CreateDataverseDatabases(
                    service,
                    databases);
                    finished = true;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Failed to connect to Dataverse"))
                    {
                        form.OutputRT.Text += Environment.NewLine + "Connection String Failed, cannot connect to dataverse: " + connectionString;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + e.Message;
                    }
                }
            }
        }
        static async Task<List<string>> SetupDataverseEnvironment(string[] databases, DataverseHandler dh, DataverseDeploy form)
        {
            string dataverseAPIName = "SMSAndWhatsAppAPI";
            List<string> apipackage = new();
            form.OutputRT.Text += Environment.NewLine + "Starting dataverse deployment";
            MessageBox.Show("May need to login a few times. Takes time for API creation to finalize in Azure.");
            if (form.apiClientId == "")
            {
                form.OutputRT.Text += Environment.NewLine + "Waiting for API Creation";

                var gs = GraphHandler.GetServiceClientWithoutAPI();
                var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, dataverseAPIName);

                await CreateAzureAPIHandler.UpdateRedirectUrlsAsync(gs, app.Id, app.AppId);
                //var secretText = await AddSecretClientPasswordAsync(gs, app.Id, app.AppId, "ArchiveAccess");

                apipackage.Add(app.DisplayName);
                apipackage.Add(app.AppId);
                apipackage.Add("0");
                apipackage.Add(app.Id);

                form.OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
                form.OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
            }
            else
            {
                //this would be the display name and is not needed on an existing API
                apipackage.Add("");
                apipackage.Add(form.apiClientId);
                apipackage.Add("1");
                //apipackage.Add(form.apiSecret);
                apipackage.Add(form.apiObjectId);
            }

            if (form.dataverseCreateAccount)
                apipackage[1] = await dh.CreateSystemAccount(new VerifyAppId(), apipackage[1].Trim(), form.SelectedOrgId);
            await CreateDatabases(dh, apipackage[1], databases, form);

            return apipackage;
        }
        internal virtual async Task CreateAllDataverseResources(DataverseHandler dh, Guid? TenantId, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, DataverseDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            (desiredPublicKeyVaultName, desiredInternalKeyVaultName) = await GetDesiredKeyVaultName(
                desiredPublicKeyVaultName,
                desiredInternalKeyVaultName,
                form.SelectedGroup);
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);

            CommunicationResourceHandler crh = new();
            (var smsIdentityId, var smsEndpoint) = await crh.InitialCreation(
                desiredCommunicationsName,
                form);

            VirtualNetworkResourceHandler vnrh = new();
            ResourceIdentifier vnetSubnetIdentity = await vnrh.InitialCreation(
                form);

            StorageAccountResourceHandler sarh = new();
            (StorageAccountResource storageIdentity, string connString) = await sarh.InitialCreation(desiredStorageName, form);

            EventGridResourceHandler egrh = new();
            await egrh.InitialCreation(
                desiredCommunicationsName,
                smsIdentityId,
                storageIdentity.Id,
                form);

            AppServicePlanResourceHandler asprh = new();
            ResourceIdentifier appPlan = await asprh.InitialCreation(
                form);

            FunctionAppResourceHandler farh = new();
            (WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource) = await farh.InitialCreation(
                appPlan,
                vnetSubnetIdentity,
                desiredStorageName,
                desiredSMSFunctionAppName,
                desiredWhatsAppFunctionAppName,
                form);

            JSONSecretNames secretNames = await Globals.LoadJSON<JSONSecretNames>(Environment.CurrentDirectory + "/JSONS/SecretNames.json");
#pragma warning disable CS8601
            string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3 };
#pragma warning restore CS8601
            List<string> apipackage = await SetupDataverseEnvironment(
                databases,
                dh,
                form);

            //dataverse creation and config updates happens during this phase as well, might try to split up at some point, complicated for security reasons
            KeyVaultResourceHandler kvrh = new();
            VaultResource internalVault = await kvrh.InitialCreation(
                secretNames,
                smsSiteResource,
                whatsAppSiteResource,
                storageIdentity,
                archiveEmail,
                databases,
                apipackage,
                connString,
                smsEndpoint,
                whatsappSystemAccessToken,
                whatsappCallbackToken,
                desiredPublicKeyVaultName,
                desiredInternalKeyVaultName,
                form);

            JSONDefaultDataverseLibrary dataverseLibrary = await Globals.LoadJSON<JSONDefaultDataverseLibrary>(form.DataverseLibraryPath);
            AutomationAccountsHandler aah = new();
            Guid automationaccountid = await aah.InitialCreation(
                dataverseLibrary,
                secretNames,
                desiredInternalKeyVaultName,
                form);

            KeyVaultResourceHandler kvrh2 = new();
            await kvrh2.UpdateInternalVaultProperties(
                smsSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                automationaccountid.ToString(),
                internalVault,
                TenantId.Value,
                form.SelectedRegion,
                form.SelectedGroup);
        }

        internal virtual async Task CreateAllCosmosResources(bool createAdminAccount, Guid? TenantId, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string desiredRestSite, string desiredCosmosName, CosmosDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            (desiredPublicKeyVaultName, desiredInternalKeyVaultName) = await GetDesiredKeyVaultName(
                desiredPublicKeyVaultName,
                desiredInternalKeyVaultName,
                form.SelectedGroup);
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            desiredCosmosName = GetDesiredCosmosName(desiredCosmosName, form.SelectedGroup);

            CommunicationResourceHandler crh = new();
            (var smsIdentityId, var smsEndpoint) = await crh.InitialCreation(
                desiredCommunicationsName,
                form);

            VirtualNetworkResourceHandler vnrh = new();
            (ResourceIdentifier vnetSubnetIdentity, string vnetName) = await vnrh.InitialCreation(
                form);

            StorageAccountResourceHandler sarh = new();
            (StorageAccountResource storageIdentity, string key) = await sarh.InitialCreation(
                desiredStorageName,
                form);

            EventGridResourceHandler egrh = new();
            await egrh.InitialCreation(
                desiredCommunicationsName,
                smsIdentityId,
                storageIdentity.Id,
                form);

            AppServicePlanResourceHandler asprh = new();
            ResourceIdentifier appPlan = await asprh.InitialCreation(form);

            FunctionAppResourceHandler farh = new();
            (WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, WebSiteResource cosmosAppSiteResource) = await farh.InitialCreation(
                appPlan,
                vnetSubnetIdentity,
                desiredStorageName,
                desiredSMSFunctionAppName,
                desiredWhatsAppFunctionAppName,
                desiredRestSite,
                form);
            //required to create keyvault secret properly.
            desiredRestSite = cosmosAppSiteResource.Data.Name;

            JSONSecretNames secretNames = await Globals.LoadJSON<JSONSecretNames>(Environment.CurrentDirectory + "/JSONS/SecretNames.json");
            JSONDefaultCosmosLibrary cosmosLibrary = await Globals.LoadJSON<JSONDefaultCosmosLibrary>(Environment.CurrentDirectory + "/JSONS/defaultLibraryCosmos.json");
#pragma warning disable CS8604
            CosmosResourceHandler cosmosrh = new();
            await cosmosrh.InitialCreation(
                cosmosLibrary,
                vnetSubnetIdentity,
                secretNames.DbName,
                desiredCosmosName,
                vnetName,
                form);
#pragma warning restore CS8604
            //dataverse creation and config updates happens during this phase as well, might try to split up at some point, complicated for security reasons
            KeyVaultResourceHandler kvrh = new();
            VaultResource internalVault = await kvrh.InitialCreation(
                createAdminAccount,
                secretNames,
                desiredRestSite,
                smsSiteResource,
                whatsAppSiteResource,
                cosmosAppSiteResource,
                storageIdentity,
                archiveEmail,
                key,
                desiredCosmosName,
                smsEndpoint,
                whatsappSystemAccessToken,
                whatsappCallbackToken,
                desiredPublicKeyVaultName,
                desiredInternalKeyVaultName,
                TenantId.Value,
                form);

            AutomationAccountsHandler aah = new();
            Guid automationaccountid = await aah.InitialCreation(
                cosmosLibrary,
                desiredCosmosName,
                desiredInternalKeyVaultName,
                form);

            KeyVaultResourceHandler kvrh2 = new();
            await kvrh2.UpdateInternalVaultProperties(
                smsSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                automationaccountid.ToString(),
                internalVault,
                TenantId.Value,
                form.SelectedRegion,
                form.SelectedGroup);
        }
    }
#pragma warning restore CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
}
