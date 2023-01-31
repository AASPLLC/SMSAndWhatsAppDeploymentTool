using Azure.ResourceManager.Automation;
using Azure.ResourceManager.Automation.Models;
using Azure;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Azure.Core;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Microsoft.Azure.Cosmos;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    public class AutomationAccountsHandler
    {
        readonly static string AutomationAccountName = "Automation-SMS-And-WhatsApp";

        public static async Task InitialCreation(JSONDefaultCosmosLibrary cosmosLibrary, string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            await CreateAutomationAccount(cosmosLibrary, desiredCosmosAccountName, internalVaultName, form);
        }
        public static async Task InitialCreation(JSONDefaultDataverseLibrary dataverseLibrary, JSONSecretNames SecretNames, string internalVaultName, DataverseDeploy form)
        {
            await CreateAutomationAccount(dataverseLibrary, SecretNames, internalVaultName, form);
        }

        static async Task<AutomationAccountResource> CreateVariables(JSONDefaultCosmosLibrary cosmosLibrary, ResourceGroupResource SelectedGroup, AzureLocation SelectedRegion, string desiredCosmosAccountName, string internalVaultName)
        {
            AutomationAccountCreateOrUpdateContent content = new()
            {
                Identity = new(ManagedServiceIdentityType.SystemAssigned),
                Sku = new(AutomationSkuName.Basic),
                Encryption = new() { KeySource = EncryptionKeySourceType.MicrosoftAutomation },
                Location = SelectedRegion,
                IsPublicNetworkAccessAllowed = true,
                IsLocalAuthDisabled = false
            };
            AutomationAccountResource response = (await SelectedGroup.GetAutomationAccounts().CreateOrUpdateAsync(WaitUntil.Completed, AutomationAccountName, content)).Value;
            await SetupVariable(response, "CosmosAccountName", desiredCosmosAccountName);
            await SetupVariable(response, "InternalVault", internalVaultName);
            await SetupVariable(response, "ResourceGroupName", "SMSAndWhatsAppResourceGroup");
            await SetupVariable(response, "SecretName", "PKey");

            return response;
        }
        static async Task<AutomationAccountResource> CreateVariables(JSONDefaultDataverseLibrary dataverseLibrary, ResourceGroupResource SelectedGroup, AzureLocation SelectedRegion, JSONSecretNames secretNames, string internalVaultName)
        {
            AutomationAccountCreateOrUpdateContent content = new()
            {
                Identity = new(ManagedServiceIdentityType.SystemAssigned),
                Sku = new(AutomationSkuName.Basic),
                Encryption = new() { KeySource = EncryptionKeySourceType.MicrosoftAutomation },
                Location = SelectedRegion,
                IsPublicNetworkAccessAllowed = true,
                IsLocalAuthDisabled = false
            };
            AutomationAccountResource response = (await SelectedGroup.GetAutomationAccounts().CreateOrUpdateAsync(WaitUntil.Completed, AutomationAccountName, content)).Value;
            
            await CreateOrSkipModule(
                response,
                "Microsoft.Graph.Authentication",
                "https://psg-prod-eastus.azureedge.net/packages/microsoft.graph.authentication.1.21.0.nupkg",
                "1.21.0"
                );

            await CreateOrSkipModule(
                response,
                "Microsoft.Graph.Users.Actions",
                "https://psg-prod-eastus.azureedge.net/packages/microsoft.graph.users.actions.1.21.0.nupkg",
                "1.21.0"
                );

            await SetupVariable(response, "InternalVault", internalVaultName);
            await SetupVariable(response, "ResourceGroupName", "SMSAndWhatsAppResourceGroup");

            //starting prefix, environment, clientid, client_secret, tenantid, & archiveemail is in keyvault.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            //prefix
            if (dataverseLibrary.StartingPrefix != null)
                await SetupVariable(response, "StartingPrefix", dataverseLibrary.StartingPrefix);

            //primary db names
            await SetupVariable(response, "AccountsDBName", secretNames.DbName1.ToLower() + "eses");
            await SetupVariable(response, "SMSMessagesDBName", secretNames.DbName2.ToLower() + "eses");
            await SetupVariable(response, "WhatsAppMessagesDBName", secretNames.DbName3.ToLower() + "eses");

            //column data
            await SetupVariable(response, "PhoneNumberDBColumn", dataverseLibrary.metadataPhoneNumberID[..^2].ToLower() + "es");
            await SetupVariable(response, "PhoneNumberIDDBColumn", dataverseLibrary.metadataPhoneNumberID.ToLower() + "es");
            await SetupVariable(response, "AssignedToDBColumn", dataverseLibrary.metadataEmailAccount.ToLower() + "es");
            await SetupVariable(response, "AssiedUserDBColumn", dataverseLibrary.metadataEmailNonAccount.ToLower() + "es");
            await SetupVariable(response, "ToDBColumn", dataverseLibrary.metadataTo.ToLower() + "es");
            await SetupVariable(response, "FromDBColumn", dataverseLibrary.metadataFrom.ToLower() + "es");
            await SetupVariable(response, "TimestampDBColumn", dataverseLibrary.metadataTimestamp.ToLower() + "es");
            await SetupVariable(response, "MessagesDBColumn", dataverseLibrary.metadataMessage.ToLower() + "es");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return response;
        }

        static string AutoPowerShellKeyCode()
        {
            return "try\r\n{\r\n" +
                "    \"Logging in to Azure...\"\r\n" +
                "    Connect-AzAccount -Identity\r\n}\r\ncatch {\r\n" +
                "    Write-Error -Message $_.Exception\r\n" +
                "    throw $_.Exception\r\n}\r\n\r\n" +
                "$resourceGroupName = Get-AutomationVariable -Name 'ResourceGroupName' # Resource Group must already exist" +
                "\r\n$accountName = Get-AutomationVariable -Name 'CosmosAccountName' # Must be all lower case" +
                "\r\n$keyKind = \"primary\" # Other key kinds: secondary, primaryReadonly, secondaryReadonly" +
                "\r\n$vault = Get-AutomationVariable -Name 'InternalVault'" +
                "\r\n$secretname = Get-AutomationVariable -Name 'SecretName'" +
                "\r\n\r\n$newkey = New-AzCosmosDBAccountKey `" +
                "\r\n    -ResourceGroupName $resourceGroupName `" +
                "\r\n    -Name $accountName `" +
                "\r\n    -KeyKind $keyKind" +
                "\r\n$Secret = ConvertTo-SecureString -String $newkey -AsPlainText -Force" +
                "\r\n$newkey = \"\"\r\n\r\nGet-AzKeyVaultSecret $vault | Where-Object {$_.Name -like $secretname} | Update-AzKeyVaultSecret -Enable $False" +
                "\r\n\r\nSet-AzKeyVaultSecret -VaultName $vault -Name $secretname -SecretValue $Secret";
        }
        static async Task CreateAutomationAccount(JSONDefaultCosmosLibrary cosmosLibrary, string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            AutomationAccountResource response = await CreateVariables(cosmosLibrary, form.SelectedGroup, form.SelectedRegion, desiredCosmosAccountName, internalVaultName);

            AutomationRunbookResource runbook = (await response.GetAutomationRunbooks().CreateOrUpdateAsync(WaitUntil.Completed, "AutoRotation", new(AutomationRunbookType.PowerShell) { Location = form.SelectedRegion })).Value;

            try
            {
                using (var stream = new MemoryStream())
                {
                    //stream.Write(Encoding.UTF8.GetBytes(runbookstring));
                    using var writer = new StreamWriter(stream);
                    writer.Write(AutoPowerShellKeyCode());
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    _ = await runbook.ReplaceContentRunbookDraftAsync(WaitUntil.Completed, stream);
                }
            }
            catch { }
            try { _ = await runbook.PublishAsync(WaitUntil.Completed); } catch { }

            string cosmosDBDailyrotation = "Dailyrotation";
            AutomationScheduleCreateOrUpdateContent schedulecontent = new(cosmosDBDailyrotation, new(DateTime.Now.AddMinutes(10)), AutomationScheduleFrequency.Day) { Interval = BinaryData.FromString("1") };
            AutomationScheduleResource schedule = (await response.GetAutomationSchedules().CreateOrUpdateAsync(WaitUntil.Completed, cosmosDBDailyrotation, schedulecontent)).Value;
            try
            {
                AutomationJobResource job = (await response.GetAutomationJobs().CreateOrUpdateAsync(WaitUntil.Completed, "JobName", new() { RunbookName = "AutoRotation" })).Value;

                if (job.Data.JobId != null)
                {
                    ScheduleAssociationProperty associationSchedule = new() { Name = schedule.Data.Name };
                    RunbookAssociationProperty associationRunbook = new() { Name = runbook.Data.Name };
                    _ = await response.GetAutomationJobSchedules().CreateOrUpdateAsync(WaitUntil.Completed, job.Data.JobId.Value, new(associationSchedule, associationRunbook));
                }
            }
            catch { }
        }
        static async Task CreateAutomationAccount(JSONDefaultDataverseLibrary dataverseLibrary, JSONSecretNames secretNames, string internalVaultName, DataverseDeploy form)
        {
            AutomationAccountResource response = await CreateVariables(dataverseLibrary, form.SelectedGroup, form.SelectedRegion, secretNames, internalVaultName);

            string runbookstring = "try\r\n{\r\n    \"Logging in to Azure...\"\r\n    Connect-AzAccount -Identity\r\n}\r\ncatch {\r\n    Write-Error -Message $_.Exception\r\n    throw $_.Exception\r\n}\r\n\r\n$resourceGroupName = Get-AutomationVariable -Name 'ResourceGroupName' # Resource Group must already exist\r\n$accountName = Get-AutomationVariable -Name 'CosmosAccountName' # Must be all lower case\r\n$keyKind = \"primary\" # Other key kinds: secondary, primaryReadonly, secondaryReadonly\r\n$vault = Get-AutomationVariable -Name 'CosmosVault'\r\n$secretname = Get-AutomationVariable -Name 'SecretName'\r\n\r\n$newkey = New-AzCosmosDBAccountKey `\r\n    -ResourceGroupName $resourceGroupName `\r\n    -Name $accountName `\r\n    -KeyKind $keyKind\r\n$Secret = ConvertTo-SecureString -String $newkey -AsPlainText -Force\r\n$newkey = \"\"\r\n\r\nGet-AzKeyVaultSecret $vault | Where-Object {$_.Name -like $secretname} | Update-AzKeyVaultSecret -Enable $False\r\n\r\nSet-AzKeyVaultSecret -VaultName $vault -Name $secretname -SecretValue $Secret";
            AutomationRunbookResource runbook = (await response.GetAutomationRunbooks().CreateOrUpdateAsync(WaitUntil.Completed, "AutoRotation", new(AutomationRunbookType.PowerShell) { Location = form.SelectedRegion })).Value;

            try
            {
                using (var stream = new MemoryStream())
                {
                    //stream.Write(Encoding.UTF8.GetBytes(runbookstring));
                    using var writer = new StreamWriter(stream);
                    writer.Write(runbookstring);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    _ = await runbook.ReplaceContentRunbookDraftAsync(WaitUntil.Completed, stream);
                }
            }
            catch { }
            try { _ = await runbook.PublishAsync(WaitUntil.Completed); } catch { }

            string cosmosDBDailyrotation = "Dailyrotation";
            AutomationScheduleCreateOrUpdateContent schedulecontent = new(cosmosDBDailyrotation, new(DateTime.Now.AddMinutes(10)), AutomationScheduleFrequency.Day) { Interval = BinaryData.FromString("1") };
            AutomationScheduleResource schedule = (await response.GetAutomationSchedules().CreateOrUpdateAsync(WaitUntil.Completed, cosmosDBDailyrotation, schedulecontent)).Value;
            try
            {
                AutomationJobResource job = (await response.GetAutomationJobs().CreateOrUpdateAsync(WaitUntil.Completed, "JobName", new() { RunbookName = "AutoRotation" })).Value;

                if (job.Data.JobId != null)
                {
                    ScheduleAssociationProperty associationSchedule = new() { Name = schedule.Data.Name };
                    RunbookAssociationProperty associationRunbook = new() { Name = runbook.Data.Name };
                    _ = await response.GetAutomationJobSchedules().CreateOrUpdateAsync(WaitUntil.Completed, job.Data.JobId.Value, new(associationSchedule, associationRunbook));
                }
            }
            catch { }
        }

        static async Task SetupVariable(AutomationAccountResource response, string name, string value)
        {
            AutomationVariableCreateOrUpdateContent cosmosvarContent = new(name)
            {
                IsEncrypted = false,
                Value = "\"" + value + "\""
            };
            try { _ = await response.GetAutomationVariables().CreateOrUpdateAsync(WaitUntil.Completed, name, cosmosvarContent); } catch { }
        }
        static async Task CreateOrSkipModule(AutomationAccountResource response, string name, string url, string version)
        {
            try
            {
                var module = response.GetAutomationAccountModule(name).Value;
                if (module.Data.Version != version)
                {
                    Console.Write(Environment.NewLine + "Version mismatch, updating: " + name);
                    _ = (await response.GetAutomationAccountModules().CreateOrUpdateAsync(WaitUntil.Completed, name, new(new()
                    {
                        Uri = new Uri(url),
                        Version = version
                    }))).Value;
                }
                else
                    Console.Write(Environment.NewLine + name + " already exists, skipping.");
            }
            catch
            {
                //while (response.GetAutomationAccountModule(module1.Data.Name).Value.Data.ProvisioningState != ModuleProvisioningState.Succeeded) { }
                Console.Write(Environment.NewLine + "Adding " + name);
                _ = (await response.GetAutomationAccountModules().CreateOrUpdateAsync(WaitUntil.Completed, name, new(new()
                {
                    Uri = new Uri(url),
                    Version = version
                }))).Value;
                Console.Write(Environment.NewLine + name + " Created.");
            }
        }
    }
}
