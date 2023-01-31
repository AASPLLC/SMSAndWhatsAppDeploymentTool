using Azure.ResourceManager.Automation;
using Azure.ResourceManager.Automation.Models;
using Azure;
using Azure.ResourceManager.Models;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    public class AutomationAccountsHandler
    {
        public static async Task InitialCreation(string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetAutomationAccounts())
            {
                desiredCosmosAccountName = item.Data.Name;
                break;
            }
            //try
            //{

            //}
            //catch
            //{
            await CreateAutomationAccount(desiredCosmosAccountName, internalVaultName, form);
            //}
        }
        static async Task CreateAutomationAccount(string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            string AutomationAccountName = "Cosmos-Auto-Key-Rotation";
            AutomationAccountCreateOrUpdateContent content = new()
            {
                Identity = new(ManagedServiceIdentityType.SystemAssigned),
                Sku = new(AutomationSkuName.Basic),
                Encryption = new() { KeySource = EncryptionKeySourceType.MicrosoftAutomation },
                Location = form.SelectedRegion,
                IsPublicNetworkAccessAllowed = true,
                IsLocalAuthDisabled = false
            };

            AutomationAccountResource response = (await form.SelectedGroup.GetAutomationAccounts().CreateOrUpdateAsync(WaitUntil.Completed, AutomationAccountName, content)).Value;
            await SetupVariable(response, "CosmosAccountName", desiredCosmosAccountName);
            await SetupVariable(response, "CosmosVault", internalVaultName);
            await SetupVariable(response, "ResourceGroupName", "SMSAndWhatsAppResourceGroup");
            await SetupVariable(response, "SecretName", "PKey");

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

            string cosmosDBDailyrotation = "CosmosDBDailyrotation";
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
    }
}
