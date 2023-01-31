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

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
#pragma warning disable CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    public class CreateResourceHandler
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
        static string GetDesiredKeyVaultName(int vault, string desiredKeyVaultName, ResourceGroupResource SelectedGroup)
        {
            List<string> vaultnames = new();
            foreach (var item in SelectedGroup.GetVaults())
            {
                vaultnames.Add(item.Data.Name);
            }
            if (vaultnames.Count == 2)
            {
                desiredKeyVaultName = vaultnames[vault];
            }
            return desiredKeyVaultName;
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
        public static async Task CreateAllDataverseResources(DataverseHandler dh, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, DataverseDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            desiredPublicKeyVaultName = GetDesiredKeyVaultName(0, desiredPublicKeyVaultName, form.SelectedGroup);
            desiredInternalKeyVaultName = GetDesiredKeyVaultName(1, desiredInternalKeyVaultName, form.SelectedGroup);
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);

            (var smsIdentityId, var smsEndpoint) = await CommunicationResourceHandler.InitialCreation(desiredCommunicationsName, form);
            ResourceIdentifier vnetSubnetIdentity = await VirtualNetworkResourceHandler.InitialCreation(form);
            (StorageAccountResource storageIdentity, string connString) = await StorageAccountResourceHandler.InitialCreation(desiredStorageName, form);
            await EventGridResourceHandler.InitialCreation(desiredCommunicationsName, smsIdentityId, storageIdentity.Id, form);
            ResourceIdentifier appPlan = await AppServicePlanResourceHandler.InitialCreation(form);
            (WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource) = await FunctionAppResourceHandler.InitialCreation(appPlan, vnetSubnetIdentity, desiredStorageName, desiredSMSFunctionAppName, desiredWhatsAppFunctionAppName, form);

            JSONSecretNames secretNames = await Globals.LoadJSON<JSONSecretNames>(Environment.CurrentDirectory + "/JSONS/SecretNames.json");
#pragma warning disable CS8601
            string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3 };
#pragma warning restore CS8601
            List<string> apipackage = await SetupDataverseEnvironment(databases, dh, form);
            //dataverse creation and config updates happens during this phase as well, might try to split up at some point, complicated for security reasons
            await KeyVaultResourceHandler.InitialCreation(secretNames, smsSiteResource, whatsAppSiteResource, storageIdentity, archiveEmail, databases, apipackage, connString, smsEndpoint, whatsappSystemAccessToken, whatsappCallbackToken, desiredPublicKeyVaultName, desiredInternalKeyVaultName, form);

            JSONDefaultDataverseLibrary dataverseLibrary = await Globals.LoadJSON<JSONDefaultDataverseLibrary>(form.DataverseLibraryPath);
            await AutomationAccountsHandler.InitialCreation(dataverseLibrary, secretNames, desiredInternalKeyVaultName, form);
        }

        public static async Task CreateAllCosmosResources(bool createAdminAccount, Guid? TenantId, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string desiredRestSite, string desiredCosmosName, CosmosDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            desiredPublicKeyVaultName = GetDesiredKeyVaultName(0, desiredPublicKeyVaultName, form.SelectedGroup);
            desiredInternalKeyVaultName = GetDesiredKeyVaultName(1, desiredInternalKeyVaultName, form.SelectedGroup);
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            desiredCosmosName = GetDesiredCosmosName(desiredCosmosName, form.SelectedGroup);

            (var smsIdentityId, var smsEndpoint) = await CommunicationResourceHandler.InitialCreation(desiredCommunicationsName, form);
            (ResourceIdentifier vnetSubnetIdentity, string vnetName) = await VirtualNetworkResourceHandler.InitialCreation(form);
            (StorageAccountResource storageIdentity, string key) = await StorageAccountResourceHandler.InitialCreation(desiredStorageName, form);
            await EventGridResourceHandler.InitialCreation(desiredCommunicationsName, smsIdentityId, storageIdentity.Id, form);
            ResourceIdentifier appPlan = await AppServicePlanResourceHandler.InitialCreation(form);
            (WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource) = await FunctionAppResourceHandler.InitialCreation(appPlan, vnetSubnetIdentity, desiredStorageName, desiredSMSFunctionAppName, desiredWhatsAppFunctionAppName, desiredRestSite, form);

            JSONSecretNames secretNames = await Globals.LoadJSON<JSONSecretNames>(Environment.CurrentDirectory + "/JSONS/SecretNames.json");
            JSONDefaultCosmosLibrary cosmosLibrary = await Globals.LoadJSON<JSONDefaultCosmosLibrary>(Environment.CurrentDirectory + "/JSONS/defaultLibraryCosmos.json");
#pragma warning disable CS8604
            await CosmosResourceHandler.InitialCreation(cosmosLibrary, vnetSubnetIdentity, secretNames.DbName, desiredCosmosName, vnetName, form);
#pragma warning restore CS8604
            //dataverse creation and config updates happens during this phase as well, might try to split up at some point, complicated for security reasons
            await KeyVaultResourceHandler.InitialCreation(createAdminAccount, secretNames, desiredRestSite, smsSiteResource, whatsAppSiteResource, storageIdentity, archiveEmail, key, desiredCosmosName, smsEndpoint, whatsappSystemAccessToken, whatsappCallbackToken, desiredPublicKeyVaultName, desiredInternalKeyVaultName, TenantId.Value, form);

            await AutomationAccountsHandler.InitialCreation(cosmosLibrary, desiredCosmosName, desiredInternalKeyVaultName, form);
        }
    }
#pragma warning restore CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
}
