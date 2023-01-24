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
        public static async Task InitialCreation(WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string[] databases, List<string> apipackage, string connString, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, DataverseDeploy form)
        {
            //string prefix = "smsapp_";
            //must be lowercase or errors will occur
            //prefix = prefix.ToLower();
            //might change depending on what happens with system account creation, needs to be re-assigned
            VaultResource publicVault;
            VaultResource internalVault;
            bool skip = false;
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, form.TenantID, form);
            }
            else
            {
                skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, form.TenantID, form);
            }
            else
            {
                skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
            }

            if (!skip && smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
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
                await UpdateRedirectUrlsAsync(GraphHandler.GetServiceClientWithoutAPI(), apipackage[3], apipackage[1]);
            }

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
        }
        public static async Task InitialCreation(string desiredRestSite, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string key, string desiredCosmosName, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, Guid TenantId, CosmosDeploy form)
        {
            VaultResource publicVault;
            VaultResource internalVault;
            bool skip = false;
            //must be lowercase or errors will occur
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, TenantId, form);
            }
            else
            {
                skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, TenantId, form);
            }
            else
            {
                skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
            }

            if (!skip && smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
                await CreateKeyVaultSecretsCosmos(desiredRestSite, publicVault, internalVault, TenantId, whatsappSystemAccessToken, whatsappCallbackToken, smsSiteResource.Data.Identity.PrincipalId.Value.ToString(), whatsAppSiteResource.Data.Identity.PrincipalId.Value.ToString(), smsEndpoint, storageIdentity.Id.Name, key, desiredCosmosName, form);

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
        }

        static void CreateSecret(VaultResource vr, string key, string value)
        {
            vr.GetSecrets().CreateOrUpdate(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new SecretProperties()
            {
                Value = value
            }));
        }

        static async Task<VaultResource> CreateKeyVaultResource(string desiredName, Guid TenantID, DataverseDeploy form)
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

            form.OutputRT.Text += Environment.NewLine + "Vaults created successfully";

            return keyVaultResponse;
        }
        static async Task<VaultResource> CreateKeyVaultResource(string desiredName, Guid TenantID, CosmosDeploy form)
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

            form.OutputRT.Text += Environment.NewLine + "Vaults created successfully";

            return keyVaultResponse;
        }

        class JSONSecretNames
        {
            public string? PDynamicsEnvironment { get; set; }
            public string? PAccountsDBPrefix { get; set; }
            public string? PSMSDBPrefix { get; set; }
            public string? PWhatsAppDBPrefix { get; set; }
            public string? PCommsEndpoint { get; set; }
            public string? PWhatsAppAccess { get; set; }
            public string? PTenantID { get; set; }
            public string? IoOrgID { get; set; }
            public string? IoClientID { get; set; }
            public string? IoSecret { get; set; }
            public string? IoEmail { get; set; }
            public string? IoJobs { get; set; }
            public string? IoCallback { get; set; }
            public string? Type { get; set; }
            public string? IoCosmos { get; set; }
            public string? IoKey { get; set; }
            public string? RESTSite { get; set; }
        }
        static async Task CreateKeyVaultSecretsDataverse(VaultResource publicVault, VaultResource internalVault, Guid TenantID, string whatsappSystemAccessToken, string verifyHTTPToken, string smsObjectId, string whatsAppObjectId, string smsEndpoint, string storageName, string storageAccountPrimaryKey, List<string> package, string dynamicsOrgId, string[] databases, DataverseDeploy form)
        {
            JSONSecretNames? secretNames = System.Text.Json.JsonSerializer.Deserialize<JSONSecretNames>(await File.ReadAllBytesAsync(Environment.NewLine + "/JSONS/SecretNames.json"));
            var name = await TokenHandler.JwtGetUsersInfo.GetUsersEmail(); // JwtGetUsersInfo jwtGetUsersInfo = new JwtGetUsersInfo();

#pragma warning disable CS8604 // Possible null reference argument.                                                                       //var name = await jwtGetUsersInfo.GetUsersEmail(tokenCredential);
            if (secretNames != null)
            {
                CreateSecret(publicVault, secretNames.PDynamicsEnvironment, form.SelectedEnvironment);
                CreateSecret(publicVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                CreateSecret(publicVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                CreateSecret(publicVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                CreateSecret(internalVault, secretNames.PDynamicsEnvironment, form.SelectedEnvironment);
                CreateSecret(internalVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                CreateSecret(internalVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                CreateSecret(internalVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                CreateSecret(internalVault, secretNames.IoOrgID, dynamicsOrgId);
                CreateSecret(internalVault, secretNames.IoClientID, package[1]);
                if (package[2] == "0")
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    CreateSecret(internalVault, secretNames.IoSecret, await AddSecretClientPasswordAsync(gs, package[3], package[1], "ArchiveAccess"));
                }
                else
                {
                    SecuredExistingSecret securedExistingSecret = new();
                    securedExistingSecret.ShowDialog();
                    CreateSecret(internalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                    securedExistingSecret.Dispose();
                }

                CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken != "")
                {
                    if (whatsappSystemAccessToken.StartsWith("Bearer "))
                        CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                    else
                        CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                }

                string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
                //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

                CreateSecret(internalVault, secretNames.PTenantID, TenantID.ToString());
                CreateSecret(internalVault, secretNames.IoEmail, name);
                //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
                CreateSecret(internalVault, secretNames.IoJobs, urlPrimary);
                if (verifyHTTPToken != "")
                    CreateSecret(internalVault, secretNames.IoCallback, verifyHTTPToken);

                CreateSecret(publicVault, secretNames.Type, "0");
                CreateSecret(internalVault, secretNames.Type, "0");
            }
#pragma warning restore CS8604 // Possible null reference argument.

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
            JSONSecretNames? secretNames = System.Text.Json.JsonSerializer.Deserialize<JSONSecretNames>(await File.ReadAllBytesAsync(Environment.NewLine + "/JSONS/SecretNames.json"));
            var name = await TokenHandler.JwtGetUsersInfo.GetUsersEmail(); // JwtGetUsersInfo jwtGetUsersInfo = new JwtGetUsersInfo();
                                                                           //var name = await jwtGetUsersInfo.GetUsersEmail(tokenCredential);
#pragma warning disable CS8604 // Possible null reference argument.                                                                       //var name = await jwtGetUsersInfo.GetUsersEmail(tokenCredential);
            if (secretNames != null)
            {
                CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken.StartsWith("Bearer "))
                    CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                else
                    CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                CreateSecret(publicVault, secretNames.RESTSite, desiredRestSite);
                CreateSecret(publicVault, secretNames.Type, "1");

                string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
                //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

                CreateSecret(internalVault, secretNames.PTenantID, TenantID.ToString());
                CreateSecret(internalVault, secretNames.IoEmail, name);
                //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
                CreateSecret(internalVault, secretNames.IoJobs, urlPrimary);
                CreateSecret(internalVault, secretNames.IoCallback, verifyHTTPToken);
                CreateSecret(internalVault, secretNames.Type, "1");

                CosmosDBAccountResource cosmosDB = (await form.SelectedGroup.GetCosmosDBAccountAsync(desiredCosmosName)).Value;
                CreateSecret(internalVault, secretNames.IoCosmos, desiredCosmosName);
                CreateSecret(internalVault, secretNames.IoKey, (await cosmosDB.GetKeysAsync()).Value.PrimaryReadonlyMasterKey);
            }
#pragma warning restore CS8604 // Possible null reference argument.

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