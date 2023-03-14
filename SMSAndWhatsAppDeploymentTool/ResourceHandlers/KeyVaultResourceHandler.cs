using Azure.ResourceManager.KeyVault.Models;
using Azure;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using AASPGlobalLibrary;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.AppService.Models;
using Azure.ResourceManager.CosmosDB;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using SMSAndWhatsAppDeploymentTool.StepByStep;
using Azure.ResourceManager.Authorization;
using Azure.ResourceManager.Authorization.Models;
using Azure.ResourceManager.Automation;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using System.Windows.Forms;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class KeyVaultResourceHandler
    {
        internal static async Task<(string, string)> GetDesiredKeyVaultName(string desiredPublicVault, string desiredInternalVault, ResourceGroupResource SelectedGroup)
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
                    try
                    {
                        var test = (await item.GetSecretAsync("SmsEndpoint")).Value;
                        desiredPublicVault = item.Data.Name;
                        //Console.Write(Environment.NewLine + "Public Vault Found: " + desiredPublicVault);
                    }
                    catch
                    {

                    }
                }
            }
            return (desiredPublicVault, desiredInternalVault);
        }
        static async Task SetIAMToVaults(ResourceGroupResource SelectedGroup)
        {
            ResourceIdentifier VaultOfficerID = new("1");
            string VaultOfficerGUID = "";
            var allroledefinitions = SelectedGroup.GetAuthorizationRoleDefinitions();
            foreach (var roledefinition in allroledefinitions)
            {
                if (roledefinition.Data.RoleName == "Key Vault Secrets Officer")
                {
                    VaultOfficerGUID = roledefinition.Data.Name;
                    VaultOfficerID = roledefinition.Data.Id;
                    break;
                }
            }

            RoleAssignmentCreateOrUpdateContent content = new(VaultOfficerID, Guid.Parse(await TokenHandler.JwtGetUsersInfo.GetUsersID()))
            {
                PrincipalType = RoleManagementPrincipalType.User
            };
            await SelectedGroup.GetRoleAssignments().CreateOrUpdateAsync(WaitUntil.Completed, VaultOfficerGUID, content);
        }
        internal static async Task RemoveIAMToVaults(ResourceGroupResource SelectedGroup)
        {
            ResourceIdentifier VaultOfficerID = new("1");
            string VaultOfficerGUID = "";
            var allroledefinitions = SelectedGroup.GetAuthorizationRoleDefinitions();
            foreach (var roledefinition in allroledefinitions)
            {
                if (roledefinition.Data.RoleName == "Key Vault Secrets Officer")
                {
                    VaultOfficerGUID = roledefinition.Data.Name;
                    VaultOfficerID = roledefinition.Data.Id;
                    break;
                }
            }

            RoleAssignmentCreateOrUpdateContent content = new(VaultOfficerID, Guid.Parse(await TokenHandler.JwtGetUsersInfo.GetUsersID()))
            {
                PrincipalType = RoleManagementPrincipalType.User
            };
            await (await SelectedGroup.GetRoleAssignments().CreateOrUpdateAsync(WaitUntil.Completed, VaultOfficerGUID, content)).Value.DeleteAsync(WaitUntil.Completed);
        }

        internal virtual async Task InitialCreation(string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, StepByStepValues sbs)
        {
            foreach (var item in sbs.SelectedGroup.GetVaults())
            {
                try
                {
                    var test = (await item.GetSecretAsync("TenantID")).Value;
                    desiredInternalKeyVaultName = item.Data.Name;
                }
                catch
                {
                    try
                    {
                        var test = (await item.GetSecretAsync("SmsEndpoint")).Value;
                        desiredPublicKeyVaultName = item.Data.Name;
                    }
                    catch { }
                }
            }
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, sbs))
            {
                await CreateKeyVaultResource(desiredPublicKeyVaultName, sbs);
                await CreateSecret(sbs, desiredPublicKeyVaultName, "SmsEndpoint", "0");
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, sbs))
            {
                await CreateKeyVaultResource(desiredInternalKeyVaultName, sbs);
                await CreateSecret(sbs, desiredInternalKeyVaultName, "TenantID", "0");
            }

            sbs.DesiredPublicVault = desiredPublicKeyVaultName;
            sbs.DesiredInternalVault = desiredInternalKeyVaultName;
            await sbs.SetupKeyVaults();

            Console.Write(Environment.NewLine + "Setting temporary Key Vault Officer IAM for deployment.");
            if (sbs.SelectedGroup != null)
            { 
                await SetIAMToVaults(sbs.SelectedGroup);
                Console.Write(Environment.NewLine + "IAM Setup");
            }
            Console.Write(Environment.NewLine + "Vaults are finished");
        }
        internal virtual async Task<VaultResource> InitialCreation(JSONSecretNames secretNames, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, StorageAccountResource storageIdentity, string archiveEmail, string[] databases, List<string> apipackage, string connString, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, string smsTemplate, DataverseDeploy form)
        {
            //string prefix = "smsapp_";
            //must be lowercase or errors will occur
            //prefix = prefix.ToLower();
            //might change depending on what happens with system account creation, needs to be re-assigned
            VaultResource publicVault;
            VaultResource internalVault;
            //bool skip = false;

            if (form.SelectedGroup != null)
            {
                Console.Write(Environment.NewLine + "Setting temporary Key Vault Officer IAM for deployment.");
                await SetIAMToVaults(form.SelectedGroup);
            }

            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, form.TenantID, form);
                await CreateSecret(publicVault, "SmsEndpoint", "0");
            }
            else
            {
#pragma warning disable CS8604
                //skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, form.TenantID, form);
                await CreateSecret(internalVault, "TenantID", "0");
            }
            else
            {
                //skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
#pragma warning restore CS8604
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
                await CreateAzureAPIHandler.UpdateRedirectUrlsAsync(GraphHandler.GetServiceClientWithoutAPI(), apipackage[3], apipackage[1]);
            }

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";

            if (form.SelectedGroup != null)
            {
                await RemoveIAMToVaults(form.SelectedGroup);
                Console.Write(Environment.NewLine + "Temporary Key Vault Officer IAM removed.");
            }

            return internalVault;
        }
        internal virtual async Task<VaultResource> InitialCreation(JSONSecretNames secretNames, string desiredRestSite, WebSiteResource smsSiteResource, WebSiteResource whatsAppSiteResource, WebSiteResource cosmosAppSiteResource, StorageAccountResource storageIdentity, string archiveEmail, string key, string desiredCosmosName, string smsEndpoint, string whatsappSystemAccessToken, string whatsappCallbackToken, string desiredPublicKeyVaultName, string desiredInternalKeyVaultName, Guid TenantId, string smsTemplate, CosmosDeploy form, List<string> package)
        {
            VaultResource publicVault;
            VaultResource internalVault;
            //bool skip = false;
            //must be lowercase or errors will occur
            if (await CheckKeyVaultName(desiredPublicKeyVaultName, form))
            {
                publicVault = await CreateKeyVaultResource(desiredPublicKeyVaultName, TenantId, form);
                await CreateSecret(publicVault, "SmsEndpoint", "0");
            }
            else
            {
                //skip = true;
                publicVault = await SkipKeyVault(form.SelectedGroup, desiredPublicKeyVaultName);
            }
            if (await CheckKeyVaultName(desiredInternalKeyVaultName, form))
            {
                internalVault = await CreateKeyVaultResource(desiredInternalKeyVaultName, TenantId, form);
                await CreateSecret(internalVault, "TenantID", "0");
            }
            else
            {
                //skip = true;
                internalVault = await SkipKeyVault(form.SelectedGroup, desiredInternalKeyVaultName);
            }

            if (form.SelectedGroup != null)
            {
                Console.Write(Environment.NewLine + "Setting temporary Key Vault Officer IAM for deployment.");
                await SetIAMToVaults(form.SelectedGroup);
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
                    form,
                    package);

            form.OutputRT.Text += Environment.NewLine + "Updating function app configs";
            await UpdateFunctionConfigs(desiredInternalKeyVaultName, smsSiteResource, whatsAppSiteResource, cosmosAppSiteResource);
            form.OutputRT.Text += Environment.NewLine + "Finished updating function app configs";

            if (form.SelectedGroup != null)
            {
                await RemoveIAMToVaults(form.SelectedGroup);
                Console.Write(Environment.NewLine + "Temporary Key Vault Officer IAM removed.");
            }

            return internalVault;
        }

        internal virtual async Task UpdateInternalVaultProperties(StepByStepValues sbs, string desiredSMSFunctionApp, string desiredWhatsAppFunctionApp)
        {
            if (sbs.TenantID != null)
            {
                VaultResource internalVault = (await sbs.SelectedGroup.GetVaultAsync(sbs.DesiredInternalVault)).Value;
                VaultProperties properties = internalVault.Data.Properties;
                properties.EnabledForTemplateDeployment = false;
                properties.EnableRbacAuthorization = false;

                AccessPermissions permissions = new();
                permissions.Secrets.Add(SecretPermissions.Get);
                permissions.Secrets.Add(SecretPermissions.List);
                if (desiredSMSFunctionApp != "")
                {
                    Guid? smsPrincipal = (await sbs.SelectedGroup.GetWebSiteAsync(desiredSMSFunctionApp)).Value.Data.Identity.PrincipalId;
                    if (smsPrincipal != null)
                        properties.AccessPolicies.Add(new(sbs.TenantID.Value, smsPrincipal.Value.ToString(), permissions));
                }
                if (desiredWhatsAppFunctionApp != "")
                {
                    Guid? whatsappPrincipal = (await sbs.SelectedGroup.GetWebSiteAsync(desiredWhatsAppFunctionApp)).Value.Data.Identity.PrincipalId;
                    if (whatsappPrincipal != null)
                        properties.AccessPolicies.Add(new(sbs.TenantID.Value, whatsappPrincipal.Value.ToString(), permissions));
                }

                AccessPermissions automationPermissions = new();
                automationPermissions.Secrets.Add(SecretPermissions.Get);
                automationPermissions.Secrets.Add(SecretPermissions.Set);
                automationPermissions.Secrets.Add(SecretPermissions.List);
                AutomationAccountResource aar = (await sbs.SelectedGroup.GetAutomationAccountAsync(sbs.DesiredAutomationAccount)).Value;
                _ = await AutomationAccountsHandler.CreateRunbook(aar, AutomationAccountsHandler.AutoPowerShellDataverseArchiver(), "AutoArchiver ", sbs.SelectedRegion);
                properties.AccessPolicies.Add(new(sbs.TenantID.Value, (await AutomationAccountsHandler.CreateRunbook(aar, AutomationAccountsHandler.AutoPowerShellKeyCode(), "AutoRotation", sbs.SelectedRegion)).ToString(), automationPermissions));

                AccessPermissions addSelf = new();
                addSelf.Secrets.Add(SecretPermissions.Get);
                addSelf.Secrets.Add(SecretPermissions.Set);
                addSelf.Secrets.Add(SecretPermissions.List);
                properties.AccessPolicies.Add(new(sbs.TenantID.Value, await TokenHandler.JwtGetUsersInfo.GetUsersID(), addSelf));

                VaultCreateOrUpdateContent content = new(sbs.SelectedRegion, properties);
                _ = (await sbs.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, internalVault.Data.Name, content)).Value;
            }
        }
        internal virtual async Task UpdateInternalVaultProperties(StepByStepValues sbs, string desiredSMSFunctionApp, string desiredWhatsAppFunctionApp, string desiredRestApp)
        {
            if (sbs.TenantID != null)
            {
                VaultResource internalVault = (await sbs.SelectedGroup.GetVaultAsync(sbs.DesiredInternalVault)).Value;
                VaultProperties properties = internalVault.Data.Properties;
                properties.EnabledForTemplateDeployment = false;
                properties.EnableRbacAuthorization = false;

                if (sbs.secretNames.AutomationId != null)
                {
                    if (desiredRestApp != "")
                    {
                        AccessPermissions restpermissions = new();
                        restpermissions.Secrets.Add(SecretPermissions.Get);
                        restpermissions.Secrets.Add(SecretPermissions.List);
                        Guid? restPrincipal = (await sbs.SelectedGroup.GetWebSiteAsync(desiredRestApp)).Value.Data.Identity.PrincipalId;
                        if (restPrincipal != null)
                        {
                            await CreateSecret(sbs, sbs.DesiredInternalVault, sbs.secretNames.AutomationId, restPrincipal.Value.ToString());
                            properties.AccessPolicies.Add(new(sbs.TenantID.Value, restPrincipal.Value.ToString(), restpermissions));
                        }
                    }
                }

                AccessPermissions permissions = new();
                permissions.Secrets.Add(SecretPermissions.Get);
                permissions.Secrets.Add(SecretPermissions.List);
                if (desiredSMSFunctionApp != "")
                {
                    Guid? smsPrincipal = (await sbs.SelectedGroup.GetWebSiteAsync(desiredSMSFunctionApp)).Value.Data.Identity.PrincipalId;
                    if (smsPrincipal != null)
                        properties.AccessPolicies.Add(new(sbs.TenantID.Value, smsPrincipal.Value.ToString(), permissions));
                }
                if (desiredWhatsAppFunctionApp != "")
                {
                    Guid? whatsappPrincipal = (await sbs.SelectedGroup.GetWebSiteAsync(desiredWhatsAppFunctionApp)).Value.Data.Identity.PrincipalId;
                    if (whatsappPrincipal != null)
                        properties.AccessPolicies.Add(new(sbs.TenantID.Value, whatsappPrincipal.Value.ToString(), permissions));
                }

                AccessPermissions automationPermissions = new();
                automationPermissions.Secrets.Add(SecretPermissions.Get);
                automationPermissions.Secrets.Add(SecretPermissions.Set);
                automationPermissions.Secrets.Add(SecretPermissions.List);
                AutomationAccountResource aar = (await sbs.SelectedGroup.GetAutomationAccountAsync(sbs.DesiredAutomationAccount)).Value;
                _ = await AutomationAccountsHandler.CreateRunbook(aar, AutomationAccountsHandler.AutoPowerShellCosmosArchiver(), "AutoArchiver ", sbs.SelectedRegion);
                properties.AccessPolicies.Add(new(sbs.TenantID.Value, (await AutomationAccountsHandler.CreateRunbook(aar, AutomationAccountsHandler.AutoPowerShellKeyCode(), "AutoRotation", sbs.SelectedRegion)).ToString(), automationPermissions));

                AccessPermissions addSelf = new();
                addSelf.Secrets.Add(SecretPermissions.Get);
                addSelf.Secrets.Add(SecretPermissions.Set);
                addSelf.Secrets.Add(SecretPermissions.List);
                properties.AccessPolicies.Add(new(sbs.TenantID.Value, await TokenHandler.JwtGetUsersInfo.GetUsersID(), addSelf));

                VaultCreateOrUpdateContent content = new(sbs.SelectedRegion, properties);
                _ = (await sbs.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, internalVault.Data.Name, content)).Value;
            }
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
            AccessPermissions addSelf = new();
            addSelf.Secrets.Add(SecretPermissions.Get);
            addSelf.Secrets.Add(SecretPermissions.Set);
            addSelf.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, await TokenHandler.JwtGetUsersInfo.GetUsersID(), addSelf));
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
            AccessPermissions addSelf = new();
            addSelf.Secrets.Add(SecretPermissions.Get);
            addSelf.Secrets.Add(SecretPermissions.Set);
            addSelf.Secrets.Add(SecretPermissions.List);
            properties.AccessPolicies.Add(new(TenantID, await TokenHandler.JwtGetUsersInfo.GetUsersID(), addSelf));
            VaultCreateOrUpdateContent content = new(SelectedRegion, properties);
            _ = (await SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, vaultResource.Data.Name, content)).Value;
        }

#pragma warning disable IDE0051 // Remove unused private members
        //user will need access to set old versions to false
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
        internal static async Task<bool> CreateSecret(StepByStepValues sbs, string desiredVault, string key, string value)
        {
            try
            {
                SecretClient sc = TokenHandler.GetFunctionAppKeyVaultClient("https://" + desiredVault + ".vault.azure.net/");
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
                    var temp = await (await sbs.SelectedGroup.GetVaultAsync(desiredVault)).Value.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                    {
                        Value = value
                    }));
                }
                return false;
            }
            catch
            {
                await (await sbs.SelectedGroup.GetVaultAsync(desiredVault)).Value.GetSecrets().CreateOrUpdateAsync(WaitUntil.Completed, key, new SecretCreateOrUpdateContent(new()
                {
                    Value = value
                }));
                return true;
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

        static async Task CreateKeyVaultResource(string desiredName, StepByStepValues sbs)
        {
            Console.Write(Environment.NewLine + "Waiting for KeyVault Creation");
            //KeyVaultHandler kv = new KeyVaultHandler();

#pragma warning disable CS8629 // Nullable value type may be null.
            VaultProperties properties = new(sbs.TenantID.Value, new KeyVaultSku(KeyVaultSkuFamily.A, KeyVaultSkuName.Standard))
            {
                EnableRbacAuthorization = true,
                EnabledForDeployment = false,
                EnabledForDiskEncryption = false,
                EnabledForTemplateDeployment = true
            };
#pragma warning restore CS8629 // Nullable value type may be null.
            VaultCreateOrUpdateContent content = new(sbs.SelectedRegion, properties);

            _ = (await sbs.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, content)).Value;

            Console.Write(Environment.NewLine + desiredName + " created successfully");
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

        static async Task<bool> CheckAPISecret(JSONSecretNames secretNames, VaultResource internalVault, List<string> package, string TenantID)
        {
            if (secretNames.IoSecret != null)
            {
                if (package[2] == "0")
                {
                    var gs = GraphHandler.GetServiceClientWithoutAPI();
                    await CreateSecret(internalVault, secretNames.IoSecret, await CreateAzureAPIHandler.AddSecretClientPasswordAsync(gs, package[3], "ArchiveAccess"));
                    return true;
                }
                else
                {
                    try
                    {
                        _ = await TokenHandler.GetConfidentialClientAccessToken(
                            package[1],
                            await VaultHandler.GetSecretInteractive(internalVault.Data.Name, secretNames.IoSecret),
                            new[] { "https://graph.microsoft.com/.default" },
                            TenantID);
                        Console.Write(Environment.NewLine + "Secure login passed.");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.Write(Environment.NewLine + "Unable to locate secret login: " + e.ToString());
                        Console.Write(Environment.NewLine + "Attempting Auto fix...");
                        try
                        {
                            var gs = GraphHandler.GetServiceClientWithoutAPI();
                            await CreateSecret(internalVault, secretNames.IoSecret, await CreateAzureAPIHandler.AddSecretClientPasswordAsync(gs, package[3], "ArchiveAccess"));
                            await Task.Delay(10000);
                            _ = await TokenHandler.GetConfidentialClientAccessToken(
                                package[1],
                                await VaultHandler.GetSecretInteractive(internalVault.Data.Name, secretNames.IoSecret),
                                new[] { "https://graph.microsoft.com/.default" },
                                TenantID.ToString());
                            Console.Write(Environment.NewLine + "Secure login passed.");
                            Console.Write(Environment.NewLine + "Key Vault secrets created or updated.");
                            return true;
                        }
                        catch
                        {
                            Console.Write(Environment.NewLine + "Auto fix failed...");
                            SecuredExistingSecret securedExistingSecret = new();
                            securedExistingSecret.ShowDialog();
                            if (securedExistingSecret.GetSecuredString() != "")
                                await CreateSecret(internalVault, secretNames.IoSecret, securedExistingSecret.GetSecuredString());
                            securedExistingSecret.Dispose();

                            try
                            {
                                _ = await TokenHandler.GetConfidentialClientAccessToken(
                                    package[1],
                                    await VaultHandler.GetSecretInteractive(internalVault.Data.Name, secretNames.IoSecret),
                                    new[] { "https://graph.microsoft.com/.default" },
                                    TenantID.ToString());
                                Console.Write(Environment.NewLine + "Secure login passed.");
                                Console.Write(Environment.NewLine + "Key Vault secrets created or updated.");
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
                }
            }
            else
                return false;
        }

        static async Task<bool> CreateKeyVaultSecretsDataverse(JSONSecretNames secretNames, VaultResource publicVault, VaultResource internalVault, Guid TenantID, string archiveEmail, string whatsappSystemAccessToken, string verifyHTTPToken, string smsEndpoint, string storageName, string storageAccountPrimaryKey, List<string> package, string dynamicsOrgId, string[] databases, string smsTemplate, DataverseDeploy form)
        {
            JSONDefaultDataverseLibrary dataverseLibrary = await JSONDefaultDataverseLibrary.Load();
            bool success;
#pragma warning disable CS8604 // Possible null reference argument.
            if (dataverseLibrary != null)
            {
                await CreateSecret(publicVault, secretNames.StartingPrefix, dataverseLibrary.StartingPrefix);
            }
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

                await CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken != "")
                {
                    if (whatsappSystemAccessToken.StartsWith("Bearer "))
                    {
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                        await CreateSecret(internalVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                    }
                    else
                    {
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                        await CreateSecret(internalVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                    }
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

                success = await CheckAPISecret(secretNames, internalVault, package, TenantID.ToString());
            }
            else
                success = false;

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

            return success;
        }
        static async Task<bool> CreateKeyVaultSecretsCosmos(JSONSecretNames secretNames, string archiveEmail, string desiredRestSite, VaultResource publicVault, VaultResource internalVault, Guid TenantID, string whatsappSystemAccessToken, string verifyHTTPToken, string smsEndpoint, string storageName, string storageAccountPrimaryKey, string desiredCosmosName, string smsTemplate, CosmosDeploy form, List<string> package)
        {
            //so far not possible due to the custom security. you cannot create an account without already having one that has admin access.
            /*if (createAdminAccount)
            {
                var name = await TokenHandler.JwtGetUsersInfo.GetUsersID();
                string fullurl = "https://" + desiredRestSite + ".documents.azure.com/";
                form.OutputRT.Text += Environment.NewLine + "First account creation attempt: " + await CosmosDBHandler.AddOrUpdateAccount(fullurl, name, name, "+1", "1", "1");
            }*/

            bool success;
#pragma warning disable CS8604 // Possible null reference argument.
            if (secretNames != null)
            {
                if (!smsTemplate.Contains("COMPANYNAMEHERE") && smsTemplate != "")
                    await CreateSecret(publicVault, secretNames.SMSTemplate, smsTemplate);
                await CreateSecret(internalVault, secretNames.IoClientID, package[1]);
                await CreateSecret(publicVault, secretNames.PCommsEndpoint, smsEndpoint);
                if (whatsappSystemAccessToken != "")
                {
                    if (whatsappSystemAccessToken.StartsWith("Bearer "))
                    {
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                        await CreateSecret(internalVault, secretNames.PWhatsAppAccess, whatsappSystemAccessToken);
                    }
                    else
                    {
                        await CreateSecret(publicVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                        await CreateSecret(internalVault, secretNames.PWhatsAppAccess, "Bearer " + whatsappSystemAccessToken);
                    }
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

                success = await CheckAPISecret(secretNames, internalVault, package, TenantID.ToString());
            }
            else
                success = false;

            VaultProperties properties = publicVault.Data.Properties;
            properties.EnabledForTemplateDeployment = false;
            VaultCreateOrUpdateContent content = new(form.SelectedRegion, properties);
            _ = (await form.SelectedGroup.GetVaults().CreateOrUpdateAsync(WaitUntil.Completed, publicVault.Data.Name, content)).Value;

            return success;
        }

        static async Task<bool> CheckKeyVaultName(string desiredName, StepByStepValues sbs)
        {
            desiredName = desiredName.Trim();
            try
            {
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Key Vault text is empty and Secret check could not be found.");
                    return false;
                }
                _ = await sbs.SelectedGroup.GetVaultAsync(desiredName);
                Console.Write(Environment.NewLine + desiredName + " and " + desiredName + " already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await sbs.SelectedSubscription.CheckKeyVaultNameAvailabilityAsync(new VaultCheckNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.NameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        //form.OutputRT.Text += Environment.NewLine + ex.Message;
                        Console.Write(Environment.NewLine + "Key Vault name has already been taken, try another.");
                        return false;
                    }
                }
                else
                {
                    Console.Write(Environment.NewLine + "Key Vault text is empty.");
                    return false;
                }
            }
        }
        static async Task<bool> CheckKeyVaultName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Key Vault text is empty and Secret check could not be found.");
                    return false;
                }
                _ = await form.SelectedGroup.GetVaultAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " and " + desiredName + " already exists in your environment, skipping.";
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
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Key Vault text is empty and Secret check could not be found.");
                    return false;
                }
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

        internal static async Task UpdateSMSConfigs(string desiredSMSFunction, StepByStepValues sbs)
        {
            WebSiteResource smsFunctionAppResource = await sbs.SelectedGroup.GetWebSiteAsync(desiredSMSFunction);
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
                Value = sbs.DesiredInternalVault
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobs.QueueTrigger.Disabled",
                Value = "0"
            });
            smsApp.AppSettings.Add(new AppServiceNameValuePair()
            {
                Name = "AzureWebJobsStorage",
                Value = "@Microsoft.KeyVault(VaultName=" + sbs.DesiredInternalVault + ";SecretName=AzureWebJobsStorage)"
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
        internal static async Task UpdateWhatsAppConfigs(string desiredWhatsAppFunction, StepByStepValues sbs)
        {
            WebSiteResource whatsAppFunctionAppResource = await sbs.SelectedGroup.GetWebSiteAsync(desiredWhatsAppFunction);
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
                Value = sbs.DesiredInternalVault
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
        internal static async Task UpdateCosmosConfigs(string desiredCosmosRest, StepByStepValues sbs)
        {
            WebSiteResource cosmosAppFunctionAppResource = await sbs.SelectedGroup.GetWebSiteAsync(desiredCosmosRest);
            SiteConfigData cosmosApp = new();
            cosmosApp.AppSettings.Add(new AppServiceNameValuePair
            {
                Name = "vaultname",
                Value = sbs.DesiredInternalVault
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