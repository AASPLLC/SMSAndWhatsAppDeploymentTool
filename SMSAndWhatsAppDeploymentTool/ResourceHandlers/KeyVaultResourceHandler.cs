using Azure.ResourceManager.KeyVault.Models;
using Azure;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using AASPGlobalLibrary;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.AppService.Models;
using static AASPGlobalLibrary.CreateAzureAPIHandler;
using Azure.ResourceManager.CosmosDB;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    public class KeyVaultResourceHandler
    {
        public static async Task InitialCreation(WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string[] databases, List<string> apipackage, string connString, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredKeyVaultName, DataverseDeploy form)
        {
            //string prefix = "smsapp_";
            //must be lowercase or errors will occur
            //prefix = prefix.ToLower();
            //might change depending on what happens with system account creation, needs to be re-assigned

            string internalVaultName = desiredKeyVaultName + "io";

            if (await CheckKeyVaultName(desiredKeyVaultName, form))
            {
                (VaultResource publicVault, VaultResource internalVault) = await CreateKeyVaultResource(desiredKeyVaultName, form.TenantID, form);

                await UpdateRedirectUrlsAsync(GraphHandler.GetServiceClientWithoutAPI(), apipackage[3], apipackage[1]);

                if (smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
                {
                    await CreateKeyVaultSecretsDataverse(
                        publicVault,
                        internalVault,
                        form.TenantID,
                        whatsappSystemAccessToken,
                        whatsappCallbackToken,
                        smsSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                        whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(),
                        smsEndpoint,
                        storageIdentity.Id.Name,
                        connString,
                        apipackage,
                        form.SelectedOrgId,
                        databases,
                        form);
                }
            }

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(internalVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
        }
        public static async Task InitialCreation(string desiredRestSite, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string key, string desiredCosmosName, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredKeyVaultName, Guid TenantId, CosmosDeploy form)
        {
            //must be lowercase or errors will occur
            string internalVaultName = desiredKeyVaultName + "io";

            if (await CheckKeyVaultName(desiredKeyVaultName, form))
            {
                (VaultResource publicVault, VaultResource internalVault) = await CreateKeyVaultResource(desiredKeyVaultName, TenantId, form);

                if (smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
                    await CreateKeyVaultSecretsCosmos(desiredRestSite, publicVault, internalVault, TenantId, whatsappSystemAccessToken, whatsappCallbackToken, smsSiteResource.Data.Identity.PrincipalId.Value.ToString(), whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(), smsEndpoint, storageIdentity.Id.Name, key, desiredCosmosName, form);
            }
            else
            {
                _ = await SkipKeyVault(form.SelectedGroup, internalVaultName);
            }

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(internalVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
        }

        static void CreateSecret(VaultResource vr, string key, string value)
        {
            vr.GetSecrets().CreateOrUpdate(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new SecretProperties()
            {
                Value = value
            }));
        }

        static async Task<(VaultResource, VaultResource)> CreateKeyVaultResource(string desiredName, Guid TenantID, DataverseDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for KeyVaults Creation";
            //KeyVaultHandler kv = new KeyVaultHandler();

            VaultProperties properties = new(TenantID, new KeyVaultSku(KeyVaultSkuFamily.A, KeyVaultSkuName.Standard))
            {
                EnableRbacAuthorization = true,
                EnabledForDeployment = false,
                EnabledForDiskEncryption = false,
                EnabledForTemplateDeployment = true
            };
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);

            var keyVaultResponse = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, content)).Value;
            var keyVaultResponse2 = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "io", content)).Value;

            form.OutputRT.Text += Environment.NewLine + "Vaults created successfully";

            return (keyVaultResponse, keyVaultResponse2);
        }
        static async Task<(VaultResource, VaultResource)> CreateKeyVaultResource(string desiredName, Guid TenantID, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for KeyVaults Creation";
            //KeyVaultHandler kv = new KeyVaultHandler();

            VaultProperties properties = new(TenantID, new KeyVaultSku(KeyVaultSkuFamily.A, KeyVaultSkuName.Standard))
            {
                EnableRbacAuthorization = true,
                EnabledForDeployment = false,
                EnabledForDiskEncryption = false,
                EnabledForTemplateDeployment = true
            };
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);

            var keyVaultResponse = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, content)).Value;
            var keyVaultResponse2 = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "io", content)).Value;

            form.OutputRT.Text += Environment.NewLine + "Vaults created successfully";

            return (keyVaultResponse, keyVaultResponse2);
        }

        static async Task CreateKeyVaultSecretsDataverse(VaultResource publicVault, VaultResource internalVault, Guid TenantID, string whatsappSystemAccessToken, string verifyHTTPToken, string smsObjectId, string whatsAppObjectId, string smsEndpoint, string storageName, string storageAccountPrimaryKey, List<string> package, string dynamicsOrgId, string[] databases, DataverseDeploy form)
        {
            var name = await TokenHandler.JwtGetUsersInfo.GetUsersEmail(); // JwtGetUsersInfo jwtGetUsersInfo = new JwtGetUsersInfo();
            //var name = await jwtGetUsersInfo.GetUsersEmail(tokenCredential);
            CreateSecret(publicVault, "DynamicsEnvironmentName", form.SelectedEnvironment);
            CreateSecret(publicVault, "AccountsDBEndingPrefix", databases[0].ToLower() + "eses");
            CreateSecret(publicVault, "SMSDBEndingPrefix", databases[1].ToLower() + "eses");
            CreateSecret(publicVault, "WhatsAppDBEndingPrefix", databases[2].ToLower() + "eses");
            CreateSecret(publicVault, "SmsEndpoint", smsEndpoint);
            if (whatsappSystemAccessToken.StartsWith("Bearer "))
                CreateSecret(publicVault, "WhatsAppSystemAccess", whatsappSystemAccessToken);
            else
                CreateSecret(publicVault, "WhatsAppSystemAccess", "Bearer " + whatsappSystemAccessToken);
            CreateSecret(publicVault, "DBType", "0");

            string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
            //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

            CreateSecret(internalVault, "DynamicsEnvironmentName", form.SelectedEnvironment);
            CreateSecret(internalVault, "TenantID", TenantID.ToString());
            CreateSecret(internalVault, "AccountsDBEndingPrefix", databases[0].ToLower() + "eses");
            CreateSecret(internalVault, "SMSDBEndingPrefix", databases[1].ToLower() + "eses");
            CreateSecret(internalVault, "WhatsAppDBEndingPrefix", databases[2].ToLower() + "eses");
            CreateSecret(internalVault, "DynamicsOrganizationID", dynamicsOrgId);
            CreateSecret(internalVault, "DataverseAPIClientID", package[1]);
            CreateSecret(internalVault, "PrimarySystemAccountEmail", name);
            //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
            CreateSecret(internalVault, "AzureWebJobsStorage", urlPrimary);
            CreateSecret(internalVault, "WhatsAppToken", verifyHTTPToken);
            CreateSecret(internalVault, "DBType", "0");

            if (package[2] == "0")
            {
                var gs = GraphHandler.GetServiceClientWithoutAPI();
                CreateSecret(internalVault, "DataverseAPIClientSecret", await AddSecretClientPasswordAsync(gs, package[3], package[1], "ArchiveAccess"));
            }
            else
            {
                /*SecureString secret = new();

                Console.WriteLine("Right click the paste API Secret here:");
                ConsoleKeyInfo nextKey = Console.ReadKey(true);
                while (nextKey.Key != ConsoleKey.Enter)
                {
                    if (nextKey.Key == ConsoleKey.Backspace)
                    {
                        if (secret.Length > 0)
                        {
                            secret.RemoveAt(secret.Length - 1);
                            // erase the last * as well
                            Console.Write(nextKey.KeyChar);
                            Console.Write(" ");
                            Console.Write(nextKey.KeyChar);
                        }
                    }
                    else
                    {
                        secret.AppendChar(nextKey.KeyChar);
                        Console.Write("*");
                    }
                    nextKey = Console.ReadKey(true);
                }
                secret.MakeReadOnly();*/
                SecuredExistingSecret securedExistingSecret = new();
                securedExistingSecret.ShowDialog();
                CreateSecret(internalVault, "DataverseAPIClientSecret", securedExistingSecret.GetSecuredString());
                securedExistingSecret.Dispose();
            }

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

            properties = internalVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            properties.EnableRbacAuthorization = false;
            AccessPermissions permissions = new();
            permissions.Secrets.Add(SecretPermissions.Get);
            properties.AccessPolicies.Add(new(TenantID, smsObjectId, permissions));
            properties.AccessPolicies.Add(new(TenantID, whatsAppObjectId, permissions));
            content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, internalVault.Data.Name, content)).Value;

            form.OutputRT.Text += Environment.NewLine + "Key Vault secrets created and locked by RBAC access.";
        }
        static async Task CreateKeyVaultSecretsCosmos(string desiredRestSite, VaultResource publicVault, VaultResource internalVault, Guid TenantID, string whatsappSystemAccessToken, string verifyHTTPToken, string smsObjectId, string whatsAppObjectId, string smsEndpoint, string storageName, string storageAccountPrimaryKey, string desiredCosmosName, CosmosDeploy form)
        {
            var name = await TokenHandler.JwtGetUsersInfo.GetUsersEmail(); // JwtGetUsersInfo jwtGetUsersInfo = new JwtGetUsersInfo();
            //var name = await jwtGetUsersInfo.GetUsersEmail(tokenCredential);
            CreateSecret(publicVault, "SmsEndpoint", smsEndpoint);
            if (whatsappSystemAccessToken.StartsWith("Bearer "))
                CreateSecret(publicVault, "WhatsAppSystemAccess", whatsappSystemAccessToken);
            else
                CreateSecret(publicVault, "WhatsAppSystemAccess", "Bearer " + whatsappSystemAccessToken);
            CreateSecret(publicVault, "CosmosRestSite", desiredRestSite);
            CreateSecret(publicVault, "DBType", "1");

            string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
            //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

            CreateSecret(internalVault, "TenantID", TenantID.ToString());
            CreateSecret(internalVault, "PrimarySystemAccountEmail", name);
            //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
            CreateSecret(internalVault, "AzureWebJobsStorage", urlPrimary);
            CreateSecret(internalVault, "WhatsAppToken", verifyHTTPToken);
            CreateSecret(internalVault, "DBType", "1");

            CosmosDBAccountResource cosmosDB = (await form.SelectedGroup.GetCosmosDBAccountAsync(desiredCosmosName)).Value;
            CreateSecret(internalVault, "EUri", desiredCosmosName);
            CreateSecret(internalVault, "PKey", (await cosmosDB.GetKeysAsync()).Value.PrimaryReadonlyMasterKey);

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

            properties = internalVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            properties.EnableRbacAuthorization = false;
            AccessPermissions permissions = new();
            permissions.Secrets.Add(SecretPermissions.Get);
            properties.AccessPolicies.Add(new(TenantID, smsObjectId, permissions));
            properties.AccessPolicies.Add(new(TenantID, whatsAppObjectId, permissions));
            content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, internalVault.Data.Name, content)).Value;

            form.OutputRT.Text += Environment.NewLine + "Key Vault secrets created and locked by RBAC access.";
        }

        static async Task<bool> CheckKeyVaultName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetVaultAsync(desiredName);
                _ = await form.SelectedGroup.GetVaultAsync(desiredName + "io");
                form.OutputRT.Text += Environment.NewLine + desiredName + " and " + desiredName + "io already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckKeyVaultNameAvailabilityAsync(new VaultCheckNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        //form.OutputRT.Text += Environment.NewLine + ex.Message;
                        form.OutputRT.Text += Environment.NewLine + "Key Vault name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Key Vault text is empty.";
                    return false;
                }
            }
        }
        static async Task<bool> CheckKeyVaultName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetVaultAsync(desiredName);
                _ = await form.SelectedGroup.GetVaultAsync(desiredName + "io");
                form.OutputRT.Text += Environment.NewLine + desiredName + " and " + desiredName + "io already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckKeyVaultNameAvailabilityAsync(new VaultCheckNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        //form.OutputRT.Text += Environment.NewLine + ex.Message;
                        form.OutputRT.Text += Environment.NewLine + "Key Vault name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Key Vault text is empty.";
                    return false;
                }
            }
        }

        static async Task<VaultResource> SkipKeyVault(ResourceGroupResource SelectedGroup, string desiredName)
        {
            var temp = await SelectedGroup.GetVaultAsync(desiredName);
            return temp.Value;
        }

        static async Task UpdateFunctionConfigs(string internalVaultName, WebSiteResource smsFunctionAppResource, WebSiteResource whatsAppFunctionAppResource)
        {
            AppServiceIPSecurityRestriction appServiceIPSecurityRestriction = new()
            {
                IPAddressOrCidr = "Any",
                Action = "Allow",
                Priority = 2147483647,
                Name = "Allow all",
                Description = "Allow all access"
            };

            SiteConfigData smsApp = new();
            smsApp.AppSettings.Add(new AppServiceNameValuePair
            {
                Name = "vaultname",
                Value = internalVaultName
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobs.QueueTrigger.Disabled",
                Value = "0"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobsStorage",
                Value = "@Microsoft.KeyVault(VaultName=" + internalVaultName + ";SecretName=AzureWebJobsStorage)"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "DiagnosticServices_EXTENSION_VERSION",
                Value = "~3"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_EXTENSION_VERSION",
                Value = "~4"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_WORKER_RUNTIME",
                Value = "dotnet"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "WEBSITE_RUN_FROM_PACKAGE",
                Value = "1"
            });
            smsApp.NumberOfWorkers = 1;
            smsApp.DefaultDocuments.Add("Default.htm");
            smsApp.DefaultDocuments.Add("Default.html");
            smsApp.DefaultDocuments.Add("Default.asp");
            smsApp.DefaultDocuments.Add("index.htm");
            smsApp.DefaultDocuments.Add("index.html");
            smsApp.DefaultDocuments.Add("iisstart.htm");
            smsApp.DefaultDocuments.Add("default.aspx");
            smsApp.DefaultDocuments.Add("index.php");
            smsApp.NetFrameworkVersion = "v6.0";
            smsApp.IsRequestTracingEnabled = false;
            smsApp.IsRemoteDebuggingEnabled = false;
            smsApp.RemoteDebuggingVersion = "VS2019";
            smsApp.IsHttpLoggingEnabled = false;
            smsApp.UseManagedIdentityCreds = false;
            smsApp.LogsDirectorySizeLimit = 35;
            smsApp.IsDetailedErrorLoggingEnabled = false;
            smsApp.PublishingUsername = "$" + smsFunctionAppResource.Data.Name;
            smsApp.ScmType = ScmType.None;
            smsApp.Use32BitWorkerProcess = true;
            smsApp.IsWebSocketsEnabled = false;
            smsApp.IsAlwaysOn = true;
            smsApp.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            smsApp.VirtualApplications.Add(new VirtualApplication()
            {
                VirtualPath = "/",
                PhysicalPath = "site\\wwwroot",
                IsPreloadEnabled = true
            });
            smsApp.LoadBalancing = SiteLoadBalancing.LeastRequests;
            smsApp.IsAutoHealEnabled = false;
            //siteConfigData.ManagedServiceIdentityId = 13243;

            smsApp.IPSecurityRestrictions.Add(appServiceIPSecurityRestriction);
            smsApp.ScmIPSecurityRestrictions.Add(appServiceIPSecurityRestriction);

            smsApp.AllowIPSecurityRestrictionsForScmToUseMain = true;
            smsApp.IsHttp20Enabled = false;
            smsApp.MinTlsVersion = "1.2";
            smsApp.ScmMinTlsVersion = "1.2";
            smsApp.FtpsState = AppServiceFtpsState.FtpsOnly;
            smsApp.PreWarmedInstanceCount = 0;
            smsApp.FunctionAppScaleLimit = 0;
            smsApp.IsFunctionsRuntimeScaleMonitoringEnabled = false;
            smsApp.MinimumElasticInstanceCount = 0;
            smsApp.IsVnetRouteAllEnabled = true;

            SiteConfigData whatsApp = new();
            whatsApp.AppSettings.Add(new AppServiceNameValuePair
            {
                Name = "vaultname",
                Value = internalVaultName
            });
            whatsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobs.HttpTrigger1.Disabled",
                Value = "0"
            });
            whatsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "DiagnosticServices_EXTENSION_VERSION",
                Value = "~3"
            });
            whatsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_EXTENSION_VERSION",
                Value = "~4"
            });
            whatsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_WORKER_RUNTIME",
                Value = "dotnet"
            });
            whatsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "WEBSITE_RUN_FROM_PACKAGE",
                Value = "1"
            });
            whatsApp.NumberOfWorkers = 1;
            whatsApp.DefaultDocuments.Add("Default.htm");
            whatsApp.DefaultDocuments.Add("Default.html");
            whatsApp.DefaultDocuments.Add("Default.asp");
            whatsApp.DefaultDocuments.Add("index.htm");
            whatsApp.DefaultDocuments.Add("index.html");
            whatsApp.DefaultDocuments.Add("iisstart.htm");
            whatsApp.DefaultDocuments.Add("default.aspx");
            whatsApp.DefaultDocuments.Add("index.php");
            whatsApp.NetFrameworkVersion = "v6.0";
            whatsApp.IsRequestTracingEnabled = false;
            whatsApp.IsRemoteDebuggingEnabled = false;
            whatsApp.IsHttpLoggingEnabled = false;
            whatsApp.UseManagedIdentityCreds = false;
            whatsApp.LogsDirectorySizeLimit = 35;
            whatsApp.IsDetailedErrorLoggingEnabled = false;
            whatsApp.PublishingUsername = "$" + whatsAppFunctionAppResource.Data.Name;
            whatsApp.ScmType = ScmType.None;
            whatsApp.Use32BitWorkerProcess = true;
            whatsApp.IsWebSocketsEnabled = false;
            whatsApp.IsAlwaysOn = true;
            whatsApp.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            whatsApp.VirtualApplications.Add(new VirtualApplication()
            {
                VirtualPath = "/",
                PhysicalPath = "site\\wwwroot",
                IsPreloadEnabled = true
            });
            whatsApp.LoadBalancing = SiteLoadBalancing.LeastRequests;
            whatsApp.IsAutoHealEnabled = false;
            //siteConfigData.ManagedServiceIdentityId = 13433;
            whatsApp.IsVnetRouteAllEnabled = false;
            whatsApp.VnetPrivatePortsCount = 0;
            whatsApp.IsLocalMySqlEnabled = false;

            whatsApp.IPSecurityRestrictions.Add(appServiceIPSecurityRestriction);
            whatsApp.ScmIPSecurityRestrictions.Add(appServiceIPSecurityRestriction);

            whatsApp.AllowIPSecurityRestrictionsForScmToUseMain = true;
            whatsApp.IsHttp20Enabled = false;
            whatsApp.MinTlsVersion = "1.2";
            whatsApp.ScmMinTlsVersion = "1.2";
            whatsApp.FtpsState = AppServiceFtpsState.FtpsOnly;
            whatsApp.PreWarmedInstanceCount = 0;
            whatsApp.FunctionAppScaleLimit = 0;
            whatsApp.IsFunctionsRuntimeScaleMonitoringEnabled = false;
            whatsApp.MinimumElasticInstanceCount = 0;
            _ = await smsFunctionAppResource.GetWebSiteConfig().UpdateAsync(smsApp);
            _ = await whatsAppFunctionAppResource.GetWebSiteConfig().UpdateAsync(whatsApp);
        }
    }
}
