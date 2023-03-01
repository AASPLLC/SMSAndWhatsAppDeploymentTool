using AASPGlobalLibrary;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Communication;
using Azure.ResourceManager.Resources;
using Microsoft.PowerPlatform.Dataverse.Client;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public class StepByStepValues
    {
        internal int DBType = 0;
        internal AzureLocation SelectedRegion;
        internal SubscriptionResource? SelectedSubscription;
        internal ArmClientHandler? Arm;
        internal ResourceGroupResource? SelectedGroup;
        internal Guid? TenantID;
        internal JSONDocuments infoWebsites = new();
        internal JSONSecretNames secretNames = new();

        internal string SelectedEnvironment = "";
        internal string SelectedOrgId = "";
        internal bool AutoAPI = false;

        internal string DesiredCommsName = "";
        internal string DesiredStorageName = "";
        internal string DesiredPublicVault = "";
        internal string DesiredInternalVault = "";

        internal async Task SetupKeyVaults(string desiredPublicKeyVaultName, string desiredInternalKeyVaultName)
        {
            DesiredPublicVault = desiredPublicKeyVaultName;
            DesiredInternalVault = desiredInternalKeyVaultName;
            await CreateDBTypeAndTenantSecrets();
        }
        async Task CreateDBTypeAndTenantSecrets()
        {
            if (secretNames.Type != null)
            {
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredPublicVault,
                    secretNames.Type,
                    DBType.ToString()
                    );
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredInternalVault,
                    secretNames.Type,
                    DBType.ToString()
                    );
            }
            if (secretNames.PTenantID != null && TenantID != null)
            {
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredInternalVault,
                    secretNames.PTenantID,
                    TenantID.Value.ToString());
            }
        }

        internal async Task CreateAllDataverseResources(string apiClientId, string apiObjectId, bool createSystemAccount)
        {
            DataverseHandler dh = new();
            string DataverseLibraryPath = Environment.CurrentDirectory + "/JSONS/defaultLibraryDataverse.json";
            try { await dh.InitAsync(SelectedEnvironment, DataverseLibraryPath); }
            catch { await dh.InitAsync(SelectedEnvironment); }
            if (!AutoAPI)
            {
                await SetupDataverseEnvironment(dh, apiClientId, apiObjectId, createSystemAccount);
            }
            else
            {
                await SetupDataverseEnvironment(dh, createSystemAccount);
            }
            if (secretNames.PDynamicsEnvironment != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PDynamicsEnvironment, SelectedEnvironment);
            if (secretNames.IoOrgID != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoOrgID, SelectedOrgId);
        }
        async Task CreateDatabases(DataverseHandler dh, string clientid, string[] databases)
        {
            bool finished = false;
            string environmenturl = "https://" + SelectedEnvironment + ".crm.dynamics.com";
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
                        Console.Write(Environment.NewLine + "Connection String Failed, cannot connect to dataverse: " + connectionString);
                    }
                    else
                    {
                        Console.Write(Environment.NewLine + e.Message);
                    }
                }
            }
        }
        async Task SetupDataverseEnvironment(DataverseHandler dh, string apiClientId, string apiObjectId, bool createSystemAccount)
        {
            if (TenantID != null && secretNames.IoSecret != null && secretNames.IoClientID != null && secretNames.DbName1 != null && secretNames.DbName2 != null && secretNames.DbName3 != null && secretNames.DbName4 != null && secretNames.DbName5 != null)
            {
                string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3, secretNames.DbName4, secretNames.DbName5 };

                List<string> apipackage = new();
                Console.Write(Environment.NewLine + "Starting dataverse deployment");
                MessageBox.Show("May need to login a few times. Takes time for API creation to finalize in Azure.");
                apipackage.Add(apiClientId);
                apipackage.Add(apiObjectId);

                if (createSystemAccount)
                    apiClientId = await dh.CreateSystemAccount(new VerifyAppId(), apiClientId.Trim(), SelectedOrgId);
                await CreateDatabases(dh, apiClientId, databases);
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoClientID, apiClientId);

                if (secretNames.PAccountsDBPrefix != null)
                {
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                }
                if (secretNames.PSMSDBPrefix != null)
                {
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                }
                if (secretNames.PWhatsAppDBPrefix != null)
                {
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                }
                if (secretNames.PPhoneNumberDBPrefix != null)
                {
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                }

                try
                {
                    _ = await TokenHandler.GetConfidentialClientAccessToken(
                        apiClientId,
                        await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoSecret),
                        new[] { "https://graph.microsoft.com/.default" },
                        TenantID.Value.ToString());
                }
                catch (Exception e)
                {
                    Console.Write(Environment.NewLine + e.ToString());
                    SecuredExistingSecret securedExistingSecret = new();
                    securedExistingSecret.ShowDialog();
                    if (securedExistingSecret.GetSecuredString() != "")
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                    securedExistingSecret.Dispose();
                }
            }
        }
        async Task SetupDataverseEnvironment(DataverseHandler dh, bool createSystemAccount)
        {
            if (secretNames.IoSecret != null && secretNames.IoClientID != null && secretNames.DbName1 != null && secretNames.DbName2 != null && secretNames.DbName3 != null && secretNames.DbName4 != null && secretNames.DbName5 != null)
            {
                string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3, secretNames.DbName4, secretNames.DbName5 };

                string dataverseAPIName = "SMSAndWhatsAppAPI";
                Console.Write(Environment.NewLine + "Starting dataverse deployment");
                MessageBox.Show("May need to login a few times. Takes time for API creation to finalize in Azure.");
                Console.Write(Environment.NewLine + "Waiting for API Creation");

                try
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, dataverseAPIName);

                    Console.Write(Environment.NewLine + "Created: " + app.DisplayName);

                    if (createSystemAccount)
                        app.AppId = await dh.CreateSystemAccount(new VerifyAppId(), app.AppId.Trim(), SelectedOrgId);
                    await CreateDatabases(dh, app.AppId, databases);
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoClientID, await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoClientID));
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, await CreateAzureAPIHandler.AddSecretClientPasswordAsync(gs, app.Id, app.DisplayName, "ArchiveAccess"));

                    if (secretNames.PAccountsDBPrefix != null)
                    {
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                    }
                    if (secretNames.PSMSDBPrefix != null)
                    {
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                    }
                    if (secretNames.PWhatsAppDBPrefix != null)
                    {
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                    }
                    if (secretNames.PPhoneNumberDBPrefix != null)
                    {
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                    }
                }
                catch (Exception e)
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
        }

        internal async Task SetupCosmosEnvironment()
        {
            if (secretNames.IoSecret != null && secretNames.IoClientID != null && secretNames.DbName1 != null && secretNames.DbName2 != null && secretNames.DbName3 != null && secretNames.DbName4 != null && secretNames.DbName5 != null)
            {
                string dataverseAPIName = "SMSAndWhatsAppAPI";
                Console.Write(Environment.NewLine + "Starting dataverse deployment");
                MessageBox.Show("May need to login a few times. Takes time for API creation to finalize in Azure.");
                Console.Write(Environment.NewLine + "Waiting for API Creation");

                try
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, dataverseAPIName, true);

                    Console.Write(Environment.NewLine + "Created: " + app.DisplayName);
                }
                catch (Exception e)
                {
                    MessageBox2 mb = new();
                    mb.label1.Text = "Error: Failed Creating API Automatically.";
                    mb.richTextBox1.Text = "Most common reason is due to missing Global Admin Access."
                        + Environment.NewLine + Environment.NewLine +
                        "Full Error: " + e.ToString();
                    mb.ShowDialog();
                    mb.Close();
                }
            }
        }

        internal async Task UpdateFunctionAppConfigs(string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredRestAppName)
        {
            Console.Write(Environment.NewLine + "Updating function app configs");
            await KeyVaultResourceHandler.UpdateSMSConfigs(desiredSMSFunctionAppName, this);
            await KeyVaultResourceHandler.UpdateWhatsAppConfigs(desiredWhatsAppFunctionAppName, this);
            if (DBType == 1)
                await KeyVaultResourceHandler.UpdateCosmosConfigs(desiredRestAppName, this);
            Console.Write(Environment.NewLine + "Finished updating function app configs");
        }

        internal async Task CreateSMSConnectionStringSecret()
        {
            if (secretNames.PCommsEndpoint != null)
            {
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredPublicVault,
                    secretNames.PCommsEndpoint,
                    (await (await SelectedGroup.GetCommunicationServiceResourceAsync(DesiredCommsName)).Value.GetKeysAsync()).Value.PrimaryConnectionString
                    );
            }
        }
        internal async Task CreateSMSTemplateSecret(string smsTemplate)
        {
            if (secretNames.SMSTemplate != null && !smsTemplate.Contains("COMPANYNAMEHERE") && smsTemplate != "")
            {
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredPublicVault,
                    secretNames.SMSTemplate,
                    smsTemplate
                    );
            }
        }
        internal async Task CreateWhatsAppSecrets(string whatsappSystemAccessToken, string verifyHTTPToken)
        {
            if (secretNames.IoCallback != null && verifyHTTPToken != "")
            {
                await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredInternalVault,
                    secretNames.IoCallback,
                    verifyHTTPToken);
            }
            if (secretNames.PWhatsAppAccess != null && whatsappSystemAccessToken != "")
            {
                if (whatsappSystemAccessToken.StartsWith("Bearer "))
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                else
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
            }
        }
        internal async Task CreateCosmosSecret(string desiredRestSite)
        {
            if (secretNames.RESTSite != null && desiredRestSite != "")
            {
                await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.RESTSite, desiredRestSite);
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.RESTSite, desiredRestSite);
            }
        }

        internal async Task SetupSubscriptionInfo()
        {
            try
            {
                ArmClient? client = Arm?.GetArmClient();
                TenantID = SelectedSubscription?.Data.TenantId;

                SelectedGroup = await ResourceGroupResourceHandler.FullResourceGroupCheck(this);

                Console.Write(Environment.NewLine + "Group Name: " + SelectedGroup.Data.Name);
            }
            catch (Exception ex)
            {
                Console.Write(Environment.NewLine + ex.Message);
            }
        }
    }
}
