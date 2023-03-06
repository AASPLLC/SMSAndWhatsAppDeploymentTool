using AASPGlobalLibrary;
using Azure.Core;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Storage;
using Microsoft.PowerPlatform.Dataverse.Client;
using SMSAndWhatsAppDeploymentTool.JSONParsing;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
#pragma warning disable CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    internal class CreateResourceHandler
    {
        /*static async Task CreateDatabases(DataverseHandler dh, string clientid, string[] databases, DataverseDeploy form)
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
            MessageBox.Show("You may need to login a few times. It will take time for API creation to finalize in Azure." + Environment.NewLine + "Dataverse setups require creating the databases at this point.");
            if (form.apiClientId == "")
            {
                form.OutputRT.Text += Environment.NewLine + "Waiting for API Creation";

                try
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, dataverseAPIName);

                    //var secretText = await AddSecretClientPasswordAsync(gs, app.Id, app.AppId, "ArchiveAccess");

                    apipackage.Add(app.DisplayName);
                    apipackage.Add(app.AppId);
                    apipackage.Add("0");
                    apipackage.Add(app.Id);

                    form.OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
                    form.OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
                }
                catch(Exception e)
                {
                    MessageBox2 mb = new();
                    mb.label1.Text = "Error: Failed Creating API Automatically.";
                    mb.richTextBox1.Text = "Most common reason is due to missing Global Admin Access or Dynamics 365 Admin access."
                        + Environment.NewLine + Environment.NewLine +
                        "Full Error: " + e.ToString();
                    mb.ShowDialog();
                    mb.Close();
                }
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
        }*/

        internal virtual async Task CreateAllDataverseResources(string defaultSubnet, string appsSubnet, DataverseHandler dh, Guid? TenantId, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string smsTemplate, DataverseDeploy form, List<string> apipackage, string[] databases)
        {
            desiredCommunicationsName = CommunicationResourceHandler.GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            if (desiredCommunicationsName != "")
            {
                CommunicationResourceHandler crh = new();
                (var smsIdentityId, var smsEndpoint) = await crh.InitialCreation(
                    desiredCommunicationsName,
                    form);

                VirtualNetworkResourceHandler vnrh = new();
                ResourceIdentifier vnetSubnetIdentity = await vnrh.InitialCreation(
                    defaultSubnet,
                    appsSubnet,
                    form);

                desiredStorageName = StorageAccountResourceHandler.GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
                if (desiredStorageName != "")
                {
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

                    foreach (var item in form.SelectedGroup.GetWebSites())
                    {
                        if (item.Data.Name.EndsWith("SMSApp"))
                        {
                            desiredSMSFunctionAppName = item.Data.Name[..^6];
                            break;
                        }
                    }
                    foreach (var item in form.SelectedGroup.GetWebSites())
                    {
                        if (item.Data.Name.EndsWith("WhatsApp"))
                        {
                            desiredWhatsAppFunctionAppName = item.Data.Name[..^8];
                            break;
                        }
                    }

                    if (desiredSMSFunctionAppName == "")
                        form.OutputRT.Text = "SMS Function field is empty and no existing resource an be found.";
                    else if (desiredWhatsAppFunctionAppName == "")
                        form.OutputRT.Text = "WhatsApp Function field is empty and no existing resource an be found.";
                    else
                    {
                        FunctionAppResourceHandler farh = new();
                        (WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource) = await farh.InitialCreation(
                            appPlan,
                            vnetSubnetIdentity,
                            desiredStorageName,
                            desiredSMSFunctionAppName,
                            desiredWhatsAppFunctionAppName,
                            form);

                        JSONSecretNames secretNames = await JSONSecretNames.Load();

                        (desiredPublicKeyVaultName, desiredInternalKeyVaultName) = await KeyVaultResourceHandler.GetDesiredKeyVaultName(
                                desiredPublicKeyVaultName,
                                desiredInternalKeyVaultName,
                                form.SelectedGroup);

                        if (desiredPublicKeyVaultName == "")
                            form.OutputRT.Text = "Public Key Vault field is empty and no existing resource an be found.";
                        else if (desiredPublicKeyVaultName == "")
                            form.OutputRT.Text = "Internal Key Vault field is empty and no existing resource an be found.";
                        else
                        {
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
                                smsTemplate,
                                form);

                            AutomationAccountsHandler aah = new();
                            Guid automationaccountid = await aah.InitialCreation(
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
                    }
                }
                else
                    form.OutputRT.Text = Environment.NewLine + "Storage Account field is empty and no existing resource found.";
            }
            else
                form.OutputRT.Text = Environment.NewLine + "Communications field is empty and no existing resource found.";
        }
        internal virtual async Task CreateAllCosmosResources(string defaultSubnet, string appsSubnet, Guid? TenantId, string archiveEmail, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredCommunicationsName, string desiredStorageName, string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string desiredRestSite, string desiredCosmosName, string smsTemplate, CosmosDeploy form, List<string> apipackage)
        {
            desiredCommunicationsName = CommunicationResourceHandler.GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            if (desiredCommunicationsName != "")
            {
                CommunicationResourceHandler crh = new();
                (var smsIdentityId, var smsEndpoint) = await crh.InitialCreation(
                    desiredCommunicationsName,
                    form);

                VirtualNetworkResourceHandler vnrh = new();
                (ResourceIdentifier vnetSubnetIdentity, string vnetName) = await vnrh.InitialCreation(
                    defaultSubnet,
                    appsSubnet,
                    form);

                desiredStorageName = StorageAccountResourceHandler.GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
                if (desiredStorageName != "")
                {
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

                    foreach (var item in form.SelectedGroup.GetWebSites())
                    {
                        if (item.Data.Name.EndsWith("SMSApp"))
                        {
                            desiredSMSFunctionAppName = item.Data.Name[..^6];
                            break;
                        }
                    }
                    foreach (var item in form.SelectedGroup.GetWebSites())
                    {
                        if (item.Data.Name.EndsWith("WhatsApp"))
                        {
                            desiredWhatsAppFunctionAppName = item.Data.Name[..^8];
                            break;
                        }
                    }
                    foreach (var item in form.SelectedGroup.GetWebSites())
                    {
                        if (item.Data.Name.EndsWith("CosmosREST"))
                        {
                            desiredRestSite = item.Data.Name[..^10];
                            break;
                        }
                    }

                    if (desiredSMSFunctionAppName == "")
                        form.OutputRT.Text = "SMS Function field is empty and no existing resource an be found.";
                    else if (desiredWhatsAppFunctionAppName == "")
                        form.OutputRT.Text = "WhatsApp Function field is empty and no existing resource an be found.";
                    else if (desiredRestSite == "")
                        form.OutputRT.Text = "Cosmos REST API Function field is empty and no existing resource an be found.";
                    else
                    {
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

                        desiredCosmosName = CosmosResourceHandler.GetDesiredCosmosName(desiredCosmosName, form.SelectedGroup);
                        if (desiredCosmosName != "")
                        {
                            JSONSecretNames secretNames = await JSONSecretNames.Load();
#pragma warning disable CS8604
                            CosmosResourceHandler cosmosrh = new();
                            await cosmosrh.InitialCreation(
                                vnetSubnetIdentity,
                                secretNames.DbName,
                                desiredCosmosName,
                                vnetName,
                                form);
#pragma warning restore CS8604

                            (desiredPublicKeyVaultName, desiredInternalKeyVaultName) = await KeyVaultResourceHandler.GetDesiredKeyVaultName(
                                desiredPublicKeyVaultName,
                                desiredInternalKeyVaultName,
                                form.SelectedGroup);

                            if (desiredPublicKeyVaultName == "")
                                form.OutputRT.Text = "Public Key Vault field is empty and no existing resource an be found.";
                            else if (desiredInternalKeyVaultName == "")
                                form.OutputRT.Text = "Internal Key Vault field is empty and no existing resource an be found.";
                            else
                            {
                                //dataverse creation and config updates happens during this phase as well, might try to split up at some point, complicated for security reasons
                                KeyVaultResourceHandler kvrh = new();
                                VaultResource internalVault = await kvrh.InitialCreation(
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
                                    smsTemplate,
                                    form,
                                    apipackage);

                                AutomationAccountsHandler aah = new();
                                Guid automationaccountid = await aah.InitialCreation(
                                    desiredCosmosName,
                                    desiredInternalKeyVaultName,
                                    form);

                                KeyVaultResourceHandler kvrh2 = new();
                                await kvrh2.UpdateInternalVaultProperties(
                                    secretNames,
                                    smsSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                                    whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                                    cosmosAppSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                                    automationaccountid.ToString(),
                                    internalVault,
                                    TenantId.Value,
                                    form.SelectedRegion,
                                    form.SelectedGroup);
                            }
                        }
                        else
                            form.OutputRT.Text = "Cosmos Account field is empty and no existing resource found.";
                    }
                }
                else
                    form.OutputRT.Text = Environment.NewLine + "Storage Account field is empty and no existing resource found.";
            }
            else
                form.OutputRT.Text = Environment.NewLine + "Communications field is empty and no existing resource found.";
        }
    }
#pragma warning restore CS8629 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
}
