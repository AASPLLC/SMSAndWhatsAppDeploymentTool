using AASPGlobalLibrary;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Communication;
using Azure.ResourceManager.CosmosDB;
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
        //only used for async fixes in certain situations
        internal Control InvokableText = new();

        internal string SelectedEnvironment = "";
        internal string SelectedOrgId = "";
        internal bool AutoAPI = false;

        internal string DesiredCommsName = "";
        internal string DesiredStorageName = "";
        internal string DesiredPublicVault = "";
        internal string DesiredInternalVault = "";
        internal string DesiredAutomationAccount = "";
        internal string DesiredCosmosAccount = "";

        internal async Task SetupKeyVaults()
        {
            if (DesiredPublicVault != "" && DesiredInternalVault != "")
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
        }

        internal async Task<bool> CreateAllDataverseResources(string apiClientId, string apiObjectId, bool createSystemAccount)
        {
            DataverseHandler dh = new();
            string DataverseLibraryPath = Environment.CurrentDirectory + "/JSONS/defaultLibraryDataverse.json";
            try { await dh.InitAsync(SelectedEnvironment, DataverseLibraryPath); }
            catch { await dh.InitAsync(SelectedEnvironment); }
            bool successful = false;
            if (!AutoAPI)
            {
                if (await SetupDataverseEnvironment(dh, apiClientId, apiObjectId, createSystemAccount))
                    successful = true;
            }
            else
            {
                if (await SetupDataverseEnvironment(dh, createSystemAccount))
                    successful = true;
            }
            if (secretNames.PDynamicsEnvironment != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PDynamicsEnvironment, SelectedEnvironment);
            if (secretNames.IoOrgID != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoOrgID, SelectedOrgId);
            return successful;
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
        async Task<bool> SetupDataverseEnvironment(DataverseHandler dh, string apiClientId, string apiObjectId, bool createSystemAccount)
        {
            if (TenantID != null && secretNames.IoSecret != null && secretNames.IoClientID != null && secretNames.DbName1 != null && secretNames.DbName2 != null && secretNames.DbName3 != null && secretNames.DbName4 != null && secretNames.DbName5 != null)
            {
                string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3, secretNames.DbName4, secretNames.DbName5 };

                List<string> apipackage = new();
                Console.Write(Environment.NewLine + "Starting dataverse deployment");
                MessageBox.Show("You may need to login a few times. It will take time for API creation to finalize in Azure." + Environment.NewLine + "Dataverse setups require creating the databases at this point.");
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
                    Console.Write(Environment.NewLine + "Secure login passed.");
                    return true;
                }
                catch (Exception e)
                {
                    Console.Write(Environment.NewLine + "Unable to locate secret login: " + e.ToString());
                    SecuredExistingSecret securedExistingSecret = new();
                    securedExistingSecret.ShowDialog();
                    if (securedExistingSecret.GetSecuredString() != "")
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                    securedExistingSecret.Dispose();

                    try
                    {
                        _ = await TokenHandler.GetConfidentialClientAccessToken(
                            apiClientId,
                            await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoSecret),
                            new[] { "https://graph.microsoft.com/.default" },
                            TenantID.Value.ToString());
                        Console.Write(Environment.NewLine + "Secure login passed.");
                        return true;
                    }
                    catch
                    {
                        Console.Write(Environment.NewLine + "Secure login failed.");
                        Console.Write(Environment.NewLine + "Manually create a secret named ArchiveAccess in the existing API.");
                        return false;
                    }
                }
            }
            else
            {
                Console.Write(Environment.NewLine + "Secret Names JSON is missing IoSecret, IoClientID, DBNames, and/or the TenantID is invalid");
                Console.Write(Environment.NewLine + "Secure login failed.");
                return false;
            }
        }
        async Task<bool> SetupDataverseEnvironment(DataverseHandler dh, bool createSystemAccount)
        {
            if (secretNames.IoSecret != null && secretNames.IoClientID != null && secretNames.DbName1 != null && secretNames.DbName2 != null && secretNames.DbName3 != null && secretNames.DbName4 != null && secretNames.DbName5 != null)
            {
                string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3, secretNames.DbName4, secretNames.DbName5 };

                string APIName = "SMSAndWhatsAppAPI";
                Console.Write(Environment.NewLine + "Starting dataverse deployment");
                MessageBox.Show("You may need to login a few times. It will take time for API creation to finalize in Azure." + Environment.NewLine + "Dataverse setups require creating the databases at this point.");
                Console.Write(Environment.NewLine + "Waiting for API Creation");

                try
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, APIName);

                    Console.Write(Environment.NewLine + "Created: " + app.DisplayName);

                    if (createSystemAccount)
                        app.AppId = await dh.CreateSystemAccount(new VerifyAppId(), app.AppId.Trim(), SelectedOrgId);

                    await CreateDatabases(dh, app.AppId, databases);

                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoClientID, app.AppId);
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, await CreateAzureAPIHandler.AddSecretClientPasswordAsync(gs, app.Id, "ArchiveAccess"));

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
                    return true;
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
                    return false;
                }
            }
            else
            {
                Console.Write(Environment.NewLine + "Secret Names JSON is missing IoSecret, IoClientID, and/or DBNames");
                return false;
            }
        }

        internal async Task<bool> SetupCosmosEnvironment()
        {
            if (secretNames.IoSecret != null && secretNames.IoClientID != null)
            {
                string APIName = "SMSAndWhatsAppAPI";
                Console.Write(Environment.NewLine + "Waiting for API Creation");

                try
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, APIName, true);

                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoClientID, await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoClientID));
                    await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, await CreateAzureAPIHandler.AddSecretClientPasswordAsync(gs, app.Id, "ArchiveAccess"));

                    Console.Write(Environment.NewLine + "Created: " + app.DisplayName);
                    return true;
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
                    return false;
                }
            }
            else
            {
                Console.Write(Environment.NewLine + "Secret Names JSON is missing IoSecret and/or IoClientID");
                return false;
            }
        }
        internal async Task<bool> SetupCosmosEnvironment(string apiClientId)
        {
            if (TenantID != null && secretNames.IoSecret != null && secretNames.IoClientID != null)
            {
                Console.Write(Environment.NewLine + "Checking existing API.");

                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoClientID, apiClientId);

                try
                {
                    _ = await TokenHandler.GetConfidentialClientAccessToken(
                        apiClientId,
                        await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoSecret),
                        new[] { "https://graph.microsoft.com/.default" },
                        TenantID.Value.ToString());
                    Console.Write(Environment.NewLine + "Secure login passed.");
                    return true;
                }
                catch (Exception e)
                {
                    Console.Write(Environment.NewLine + "Unable to locate secret login: " + e.ToString());
                    SecuredExistingSecret securedExistingSecret = new();
                    securedExistingSecret.ShowDialog();
                    if (securedExistingSecret.GetSecuredString() != "")
                        await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                    securedExistingSecret.Dispose();

                    try
                    {
                        _ = await TokenHandler.GetConfidentialClientAccessToken(
                            apiClientId,
                            await VaultHandler.GetSecretInteractive(DesiredInternalVault, secretNames.IoSecret),
                            new[] { "https://graph.microsoft.com/.default" },
                            TenantID.Value.ToString());
                        Console.Write(Environment.NewLine + "Secure login passed.");
                        return true;
                    }
                    catch
                    {
                        Console.Write(Environment.NewLine + "Secure login failed.");
                        Console.Write(Environment.NewLine + "Manually create a secret named ArchiveAccess in the existing API.");
                        return false;
                    }
                }
            }
            else
            {
                Console.Write(Environment.NewLine + "Secret Names JSON is missing IoSecret, IoClientID, and/or the TenantID is invalid");
                Console.Write(Environment.NewLine + "Secure login failed.");
                return false;
            }
        }

        internal async Task UpdateFunctionAppConfigs(string desiredSMSFunctionAppName, string desiredWhatsAppFunctionAppName, string desiredRestAppName)
        {
            Console.Write(Environment.NewLine + "Updating function app configs");
            if (desiredSMSFunctionAppName != "")
                await KeyVaultResourceHandler.UpdateSMSConfigs(desiredSMSFunctionAppName, this);
            else
                Console.Write(Environment.NewLine + "WARNING: SMS Configuration skipped, no function app found.");
            if (desiredWhatsAppFunctionAppName != "")
                await KeyVaultResourceHandler.UpdateWhatsAppConfigs(desiredWhatsAppFunctionAppName, this);
            else
                Console.Write(Environment.NewLine + "WARNING: WhatsApp Configuration skipped, no function app found.");
            if (DBType == 1 && desiredRestAppName != "")
                await KeyVaultResourceHandler.UpdateCosmosConfigs(desiredRestAppName, this);
            else
                Console.Write(Environment.NewLine + "WARNING: Cosmos REST API Configuration skipped, no function app found.");
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
                if (await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredPublicVault,
                    secretNames.SMSTemplate,
                    smsTemplate
                    ))
                    Console.Write(Environment.NewLine + "SMS Template has been created.");
                else
                    Console.Write(Environment.NewLine + "SMS Template has been changed if different.");
            }
            else
                Console.Write(Environment.NewLine + "SMS Template skipped, no change to sms template field found or secretNames json is incorrect.");
        }
        internal async Task CreateWhatsAppSecrets(string whatsappSystemAccessToken, string verifyHTTPToken)
        {
            if (secretNames.IoCallback != null && verifyHTTPToken != "")
            {
                if (await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredInternalVault,
                    secretNames.IoCallback,
                    verifyHTTPToken))
                    Console.Write(Environment.NewLine + "Callback verify has been created.");
                else
                    Console.Write(Environment.NewLine + "Callback verify has been changed if different.");
            }
            else
                Console.Write(Environment.NewLine + "Callback skipped, text is empty.");
            if (secretNames.PWhatsAppAccess != null && whatsappSystemAccessToken != "")
            {
                bool IsNew;
                if (whatsappSystemAccessToken.StartsWith("Bearer "))
                    IsNew = await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                else
                    IsNew = await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                if (IsNew)
                    Console.Write(Environment.NewLine + "System Access token has been created.");
                else
                    Console.Write(Environment.NewLine + "System Access token has been changed if different.");
            }
            else
                Console.Write(Environment.NewLine + "System Access skipped, text is empty.");
        }
        internal async Task CreateCosmosSecret(string desiredRestSite)
        {
            if (secretNames.RESTSite != null)
            {
                await KeyVaultResourceHandler.CreateSecret(this, DesiredPublicVault, secretNames.RESTSite, desiredRestSite);
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.RESTSite, desiredRestSite);
            }
            if (secretNames.IoCosmos != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoCosmos, DesiredCosmosAccount);
            if (secretNames.IoKey != null)
                await KeyVaultResourceHandler.CreateSecret(this, DesiredInternalVault, secretNames.IoKey, (await (await SelectedGroup.GetCosmosDBAccountAsync(DesiredCosmosAccount)).Value.GetKeysAsync()).Value.PrimaryReadonlyMasterKey);
        }
        internal async Task CreateAutoArchiverSecret(string autoArchiverEmail)
        {
            if (secretNames.IoEmail != null)
            {
                if (await KeyVaultResourceHandler.CreateSecret(
                    this,
                    DesiredInternalVault,
                    secretNames.IoEmail,
                    autoArchiverEmail
                    ))
                    Console.Write(Environment.NewLine + "Auto Archiver has been created.");
                else
                    Console.Write(Environment.NewLine + "Auto Archiver has been changed if different.");
            }
            else
                Console.Write(Environment.NewLine + "Auto Archiver skipped, secretNames json is incorrect.");
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
