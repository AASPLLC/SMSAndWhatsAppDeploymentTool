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
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class KeyVaultResourceHandler
    {
        internal virtual async Task<VaultResource> InitialCreation(JSONSecretNames secretNames, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string archiveEmail, string[] databases, List<string> apipackage, string connString, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string smsTemplate, DataverseDeploy form)
        {
            //string prefix = "smsapp_";
            //must be lowercase or errors will occur
            //prefix = prefix.ToLower();
            //might change depending on what happens with system account creation, needs to be re-assigned
            VaultResource publicVault;
            VaultResource internalVault;
            //bool skip = false;
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, form.TenantID, form);
            }
            else
            {
                //skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, form.TenantID, form);
            }
            else
            {
                //skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
            }

            if (smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
            //if (!skip && smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
            {
                await CreateKeyVaultSecretsDataverse(
                    secretNames,
                    publicVault,
                    internalVault,
                    form.TenantID,
                    archiveEmail,
                    whatsappSystemAccessToken,
                    whatsappCallbackToken,
                    smsEndpoint,
                    storageIdentity.Id.Name,
                    connString,
                    apipackage,
                    form.SelectedOrgId,
                    databases,
                    smsTemplate,
                    form);
                await UpdateRedirectUrlsAsync(GraphHandler.GetServiceClientWithoutAPI(), apipackage[3], apipackage[1]);
            }

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
            return internalVault;
        }
        internal virtual async Task<VaultResource> InitialCreation(JSONSecretNames secretNames, string desiredRestSite, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, WebSiteResource cosmosAppSiteResource, StorageAccountResource storageIdentity, string archiveEmail, string key, string desiredCosmosName, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, Guid TenantId, string smsTemplate, CosmosDeploy form)
        {
            VaultResource publicVault;
            VaultResource internalVault;
            //bool skip = false;
            //must be lowercase or errors will occur
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, TenantId, form);
            }
            else
            {
                //skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, TenantId, form);
            }
            else
            {
                //skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
            }

            if (smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
            //if (!skip && smsSiteResource.Data.Identity.PrincipalId != null && whatsAppSiteResource.Data.Identity.PrincipalId != null)
                await CreateKeyVaultSecretsCosmos(
                    secretNames,
                    archiveEmail,
                    desiredRestSite,
                    publicVault,
                    internalVault,
                    TenantId,
                    whatsappSystemAccessToken,
                    whatsappCallbackToken,
                    smsEndpoint,
                    storageIdentity.Id.Name,
                    key,
                    desiredCosmosName,
                    smsTemplate,
                    form);

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource, cosmosAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";
            return internalVault;
        }

        internal virtual async Task UpdateInternalVaultProperties(string smsObjectId, string whatsAppObjectId, string automationObjectId, VaultResource vaultResource, Guid TenantID, AzureLocation SelectedRegion, ResourceGroupResource SelectedGroup)
        {
            VaultProperties properties = vaultResource.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            properties.EnableRbacAuthorization = false;
            AccessPermissions permissions = new();
            permissions.Secrets.Add(SecretPermissions.Get);
            permissions.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, smsObjectId, permissions));
            properties.AccessPolicies.Add(new(TenantID, whatsAppObjectId, permissions));
            AccessPermissions automationPermissions = new();
            automationPermissions.Secrets.Add(SecretPermissions.Get);
            automationPermissions.Secrets.Add(SecretPermissions.Set);
            automationPermissions.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, automationObjectId, automationPermissions));
            VaultCreateOrUpdateContent content = new(SelectedRegion, properties);
            _ = (await SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, vaultResource.Data.Name, content)).Value;
        }
        internal virtual async Task UpdateInternalVaultProperties(JSONSecretNames secretNames, string smsObjectId, string whatsAppObjectId, string restAppObjectId, string automationObjectId, VaultResource vaultResource, Guid TenantID, AzureLocation SelectedRegion, ResourceGroupResource SelectedGroup)
        {

            if (secretNames != null)
            {
                if (restAppObjectId != "")
#pragma warning disable CS8604 // Possible null reference argument.
                    await CreateSecret(vaultResource, secretNames.AutomationId, restAppObjectId);
#pragma warning restore CS8604 // Possible null reference argument.
            }

            VaultProperties properties = vaultResource.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            properties.EnableRbacAuthorization = false;
            AccessPermissions permissions = new();
            permissions.Secrets.Add(SecretPermissions.Get);
            permissions.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, smsObjectId, permissions));
            properties.AccessPolicies.Add(new(TenantID, whatsAppObjectId, permissions));
            properties.AccessPolicies.Add(new(TenantID, restAppObjectId, permissions));
            AccessPermissions automationPermissions = new();
            automationPermissions.Secrets.Add(SecretPermissions.Get);
            automationPermissions.Secrets.Add(SecretPermissions.Set);
            automationPermissions.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, automationObjectId, automationPermissions));
            VaultCreateOrUpdateContent content = new(SelectedRegion, properties);
            _ = (await SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, vaultResource.Data.Name, content)).Value;
        }

#pragma warning disable IDE0051 // Remove unused private members
        static async Task CreateSecretManaged(VaultResource vr, string key, string value)
#pragma warning restore IDE0051 // Remove unused private members
        {
            try
            {
                SecretClient sc = TokenHandler.GetFunctionAppKeyVaultClient();
                if ((await sc.GetSecretAsync(key)).Value.Value != value)
                {
                    await sc.UpdateSecretPropertiesAsync(new(key) { Enabled = false });
                    await vr.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                    {
                        Value = value
                    }));
                }
            }
            catch
            {
                await vr.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                {
                    Value = value
                }));
            }
        }
        static async Task CreateSecret(VaultResource vr, string key, string value)
        {
            try
            {
                SecretClient sc = TokenHandler.GetFunctionAppKeyVaultClient("https://" + vr.Data.Name + ".vault.azure.net/");
                if ((await sc.GetSecretAsync(key)).Value.Value != value)
                {
                    await foreach (var version in (sc.GetPropertiesOfSecretVersionsAsync(key)))
                    {
                        if (version.Enabled == true)
                        {
                            version.Enabled = false;
                            await sc.UpdateSecretPropertiesAsync(version);
                        }
                    }
                    await vr.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                    {
                        Value = value
                    }));
                }
            }
            catch {
                await vr.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                {
                    Value = value
                }));
            }
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

        static async Task CreateKeyVaultSecretsDataverse(JSONSecretNames secretNames, VaultResource publicVault, VaultResource internalVault, Guid TenantID, string archiveEmail, string whatsappSystemAccessToken, string verifyHTTPToken, string smsEndpoint, string storageName, string storageAccountPrimaryKey, List<string> package, string dynamicsOrgId, string[] databases, string smsTemplate, DataverseDeploy form)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            if (secretNames != null)
            {
                if (!smsTemplate.Contains("COMPANYNAMEHERE") && smsTemplate != "")
                    await CreateSecret(publicVault, secretNames.SMSTemplate, smsTemplate);
                await CreateSecret(publicVault, secretNames.PDynamicsEnvironment, form.SelectedEnvironment);
                await CreateSecret(publicVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                await CreateSecret(publicVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                await CreateSecret(publicVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                await CreateSecret(publicVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                await CreateSecret(internalVault, secretNames.PDynamicsEnvironment, form.SelectedEnvironment);
                await CreateSecret(internalVault, secretNames.PAccountsDBPrefix, databases[0].ToLower() + "eses");
                await CreateSecret(internalVault, secretNames.PSMSDBPrefix, databases[1].ToLower() + "eses");
                await CreateSecret(internalVault, secretNames.PWhatsAppDBPrefix, databases[2].ToLower() + "eses");
                await CreateSecret(internalVault, secretNames.PPhoneNumberDBPrefix, databases[3].ToLower() + "eses");
                await CreateSecret(internalVault, secretNames.IoOrgID, dynamicsOrgId);
                await CreateSecret(internalVault, secretNames.IoClientID, package[1]);
                if (package[2] == "0")
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    await CreateSecret(internalVault, secretNames.IoSecret, await AddSecretClientPasswordAsync(gs, package[3], package[1], "ArchiveAccess"));
                }
                else
                {
                    SecuredExistingSecret securedExistingSecret = new();
                    securedExistingSecret.ShowDialog();
                    await CreateSecret(internalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                    securedExistingSecret.Dispose();
                }

                await CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken != "")
                {
                    if (whatsappSystemAccessToken.StartsWith("Bearer "))
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                    else
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                }

                string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
                await CreateSecret(internalVault, secretNames.IoJobs, urlPrimary);
                //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

                await CreateSecret(internalVault, secretNames.PTenantID, TenantID.ToString());

                if (archiveEmail != "")
                    await CreateSecret(internalVault, secretNames.IoEmail, archiveEmail);
                //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
                if (verifyHTTPToken != "")
                    await CreateSecret(internalVault, secretNames.IoCallback, verifyHTTPToken);

                await CreateSecret(publicVault, secretNames.Type, "0");
                await CreateSecret(internalVault, secretNames.Type, "0");
            }
#pragma warning restore CS8604 // Possible null reference argument.

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

            form.OutputRT.Text += Environment.NewLine + "Key Vault secrets created and locked by RBAC access.";
        }
        static async Task CreateKeyVaultSecretsCosmos(JSONSecretNames secretNames, string archiveEmail, string desiredRestSite, VaultResource publicVault, VaultResource internalVault, Guid TenantID, string whatsappSystemAccessToken, string verifyHTTPToken, string smsEndpoint, string storageName, string storageAccountPrimaryKey, string desiredCosmosName, string smsTemplate, CosmosDeploy form)
        {
            //so far not possible due to the custom security. you cannot create an account without already having one that has admin access.
            /*if (createAdminAccount)
            {
                var name = await TokenHandler.JwtGetUsersInfo.GetUsersID();
                string fullurl = "https://" + desiredRestSite + ".documents.azure.com/";
                form.OutputRT.Text += Environment.NewLine + "First account creation attempt: " + await CosmosDBHandler.AddOrUpdateAccount(fullurl, name, name, "+1", "1", "1");
            }*/


#pragma warning disable CS8604 // Possible null reference argument.
            if (secretNames != null)
            {
                if (!smsTemplate.Contains("COMPANYNAMEHERE") && smsTemplate != "")
                    await CreateSecret(publicVault, secretNames.SMSTemplate, smsTemplate);
                await CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken != "")
                {
                    if (whatsappSystemAccessToken.StartsWith("Bearer "))
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                    else
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                }
                if (desiredRestSite != "")
                {
                    await CreateSecret(publicVault, secretNames.RESTSite, desiredRestSite);
                    await CreateSecret(internalVault, secretNames.RESTSite, desiredRestSite);
                }
                await CreateSecret(publicVault, secretNames.Type, "1");

                string urlPrimary = "DefaultEndpointsProtocol=https;AccountName=" + storageName + ";AccountKey=" + storageAccountPrimaryKey + ";EndpointSuffix=core.windows.net";
                await CreateSecret(internalVault, secretNames.IoJobs, urlPrimary);
                //string urlQueuePrimary = "https://" + storageName + ".queue.core.windows.net/";

                await CreateSecret(internalVault, secretNames.PTenantID, TenantID.ToString());
                if (archiveEmail != "")
                    await CreateSecret(internalVault, secretNames.IoEmail, archiveEmail);
                //CreateSecret(internalVault, "StorageKey", storageAccountPrimaryKey);
                if (verifyHTTPToken != "")
                    await CreateSecret(internalVault, secretNames.IoCallback, verifyHTTPToken);
                await CreateSecret(internalVault, secretNames.Type, "1");

                if (desiredCosmosName != "")
                {
                    CosmosDBAccountResource cosmosDB = (await form.SelectedGroup.GetCosmosDBAccountAsync(desiredCosmosName)).Value;
                    await CreateSecret(internalVault, secretNames.IoCosmos, desiredCosmosName);
                    await CreateSecret(internalVault, secretNames.IoKey, (await cosmosDB.GetKeysAsync()).Value.PrimaryReadonlyMasterKey);
                }
            }
#pragma warning restore CS8604 // Possible null reference argument.

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

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
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
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

        static async Task UpdateSMSConfigs(string internalVaultName, WebSiteResource smsFunctionAppResource)
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
            _ = await smsFunctionAppResource.GetWebSiteConfig().UpdateAsync(smsApp);
        }
        static async Task UpdateWhatsAppConfigs(string internalVaultName, WebSiteResource whatsAppFunctionAppResource)
        {
            AppServiceIPSecurityRestriction appServiceIPSecurityRestriction = new()
            {
                IPAddressOrCidr = "Any",
                Action = "Allow",
                Priority = 2147483647,
                Name = "Allow all",
                Description = "Allow all access"
            };

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
            _ = await whatsAppFunctionAppResource.GetWebSiteConfig().UpdateAsync(whatsApp);
        }
        static async Task UpdateCosmosConfigs(string internalVaultName, WebSiteResource cosmosAppFunctionAppResource)
        {
            SiteConfigData cosmosApp = new();
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair
            {
                Name = "vaultname",
                Value = internalVaultName
            });
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobs.Function1.Disabled",
                Value = "0"
            });
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "DiagnosticServices_EXTENSION_VERSION",
                Value = "~3"
            });
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_EXTENSION_VERSION",
                Value = "~4"
            });
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "FUNCTIONS_WORKER_RUNTIME",
                Value = "dotnet"
            });
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "WEBSITE_RUN_FROM_PACKAGE",
                Value = "1"
            });
            cosmosApp.NumberOfWorkers = 1;
            cosmosApp.DefaultDocuments.Add("Default.htm");
            cosmosApp.DefaultDocuments.Add("Default.html");
            cosmosApp.DefaultDocuments.Add("Default.asp");
            cosmosApp.DefaultDocuments.Add("index.htm");
            cosmosApp.DefaultDocuments.Add("index.html");
            cosmosApp.DefaultDocuments.Add("iisstart.htm");
            cosmosApp.DefaultDocuments.Add("default.aspx");
            cosmosApp.DefaultDocuments.Add("index.php");
            cosmosApp.NetFrameworkVersion = "v6.0";
            cosmosApp.IsRequestTracingEnabled = false;
            cosmosApp.IsRemoteDebuggingEnabled = false;
            cosmosApp.IsHttpLoggingEnabled = false;
            cosmosApp.UseManagedIdentityCreds = false;
            cosmosApp.LogsDirectorySizeLimit = 35;
            cosmosApp.IsDetailedErrorLoggingEnabled = false;
            cosmosApp.PublishingUsername = "$" + cosmosAppFunctionAppResource.Data.Name;
            cosmosApp.ScmType = ScmType.None;
            cosmosApp.Use32BitWorkerProcess = true;
            cosmosApp.IsWebSocketsEnabled = false;
            cosmosApp.IsAlwaysOn = true;
            cosmosApp.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            cosmosApp.VirtualApplications.Add(new VirtualApplication()
            {
                VirtualPath = "/",
                PhysicalPath = "site\\wwwroot",
                IsPreloadEnabled = true
            });
            cosmosApp.LoadBalancing = SiteLoadBalancing.LeastRequests;
            cosmosApp.IsAutoHealEnabled = false;
            //siteConfigData.ManagedServiceIdentityId = 13433;
            cosmosApp.IsVnetRouteAllEnabled = false;
            cosmosApp.VnetPrivatePortsCount = 0;
            cosmosApp.IsLocalMySqlEnabled = false;

            cosmosApp.AllowIPSecurityRestrictionsForScmToUseMain = true;
            cosmosApp.IsHttp20Enabled = false;
            cosmosApp.MinTlsVersion = "1.2";
            cosmosApp.ScmMinTlsVersion = "1.2";
            cosmosApp.FtpsState = AppServiceFtpsState.FtpsOnly;
            cosmosApp.PreWarmedInstanceCount = 0;
            cosmosApp.FunctionAppScaleLimit = 0;
            cosmosApp.IsFunctionsRuntimeScaleMonitoringEnabled = false;
            cosmosApp.MinimumElasticInstanceCount = 0;
            _ = await cosmosAppFunctionAppResource.GetWebSiteConfig().UpdateAsync(cosmosApp);
        }
        static async Task UpdateFunctionConfigs(string internalVaultName, WebSiteResource smsFunctionAppResource, WebSiteResource whatsAppFunctionAppResource)
        {
            await UpdateSMSConfigs(internalVaultName, smsFunctionAppResource);
            await UpdateWhatsAppConfigs(internalVaultName, whatsAppFunctionAppResource);
        }
        static async Task UpdateFunctionConfigs(string internalVaultName, WebSiteResource smsFunctionAppResource, WebSiteResource whatsAppFunctionAppResource, WebSiteResource cosmosAppFunctionAppResource)
        {
            await UpdateSMSConfigs(internalVaultName, smsFunctionAppResource);
            await UpdateWhatsAppConfigs(internalVaultName, whatsAppFunctionAppResource);
            await UpdateCosmosConfigs(internalVaultName, cosmosAppFunctionAppResource);
        }
    }
}