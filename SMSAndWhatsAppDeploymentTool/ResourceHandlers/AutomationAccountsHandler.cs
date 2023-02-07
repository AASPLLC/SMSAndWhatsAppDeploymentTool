using Azure.ResourceManager.Automation;
using Azure.ResourceManager.Automation.Models;
using Azure;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Azure.Core;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using Azure.ResourceManager.Authorization.Models;
using Azure.ResourceManager.Authorization;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class AutomationAccountsHandler
    {
        readonly public string AutomationAccountName = "Automation-SMS-And-WhatsApp";

        internal virtual async Task<Guid> InitialCreation(string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            return await CreateAutomationAccount(desiredCosmosAccountName, internalVaultName, form);
        }
        internal virtual async Task<Guid> InitialCreation(JSONDefaultDataverseLibrary dataverseLibrary, JSONSecretNames SecretNames, string internalVaultName, DataverseDeploy form)
        {
            return await CreateAutomationAccount(dataverseLibrary, SecretNames, internalVaultName, form);
        }

        async Task<AutomationAccountResource> CreateVariables(ResourceGroupResource SelectedGroup, AzureLocation SelectedRegion, string desiredCosmosAccountName, string internalVaultName)
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
        async Task<AutomationAccountResource> CreateVariables(JSONDefaultDataverseLibrary dataverseLibrary, ResourceGroupResource SelectedGroup, AzureLocation SelectedRegion, JSONSecretNames secretNames, string internalVaultName)
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
            await SetupVariable(response, "PhoneNumberDBColumn", dataverseLibrary.metadataPhoneNumber.ToLower() + "es");
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
            return "try" +
                "\r\n{" +
                "\r\n    \"Logging in to Azure...\"" +
                "\r\n    Connect-AzAccount -Identity" +
                "\r\n}" +
                "\r\ncatch {" +
                "\r\n    Write-Error -Message $_.Exception" +
                "\r\n    throw $_.Exception" +
                "\r\n}" +
                "\r\n" +
                "\r\n$resourceGroupName = Get-AutomationVariable -Name 'ResourceGroupName' # Resource Group must already exist" +
                "\r\n$accountName = Get-AutomationVariable -Name 'CosmosAccountName' # Must be all lower case" +
                "\r\n$keyKind = \"primary\" # Other key kinds: secondary, primaryReadonly, secondaryReadonly" +
                "\r\n$vault = Get-AutomationVariable -Name 'InternalVault'" +
                "\r\n$secretname = Get-AutomationVariable -Name 'SecretName'" +
                "\r\n" +
                "\r\n$newkey = New-AzCosmosDBAccountKey `" +
                "\r\n    -ResourceGroupName $resourceGroupName `" +
                "\r\n    -Name $accountName `" +
                "\r\n    -KeyKind $keyKind" +
                "\r\n$Secret = ConvertTo-SecureString -String $newkey -AsPlainText -Force" +
                "\r\n$newkey = \"\"" +
                "\r\n" +
                "\r\nGet-AzKeyVaultSecret $vault | Where-Object {$_.Name -like $secretname} | Update-AzKeyVaultSecret -Enable $False" +
                "\r\n" +
                "\r\nSet-AzKeyVaultSecret -VaultName $vault -Name $secretname -SecretValue $Secret";
        }
        static string AutoPowerShellDataverseArchiver()
        {
            return "#0 = time 1 = count" +
                "\r\n$global:basedon = 1" +
                "\r\n$global:messagecount = 100" +
                "\r\n$global:TimeOffset = (Get-Date).AddMonths(-1)" +
                "\r\n" +
                "\r\ntry" +
                "\r\n{" +
                "\r\n    Connect-AzAccount -Identity | Out-Null" +
                "\r\n}" +
                "\r\ncatch {" +
                "\r\n    Write-Error -Message $_.Exception" +
                "\r\n    throw $_.Exception" +
                "\r\n}" +
                "\r\n" +
                "\r\n$vault = Get-AutomationVariable -Name 'InternalVault'" +
                "\r\n" +
                "\r\n$AppId = (Get-AzKeyVaultSecret -VaultName $vault -Name \"DataverseAPIClientID\").SecretValue" +
                "\r\n$AppId = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($AppId)" +
                "\r\n$AppId = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($AppId)" +
                "\r\n" +
                "\r\n$client_secret = (Get-AzKeyVaultSecret -VaultName $vault -Name \"DataverseAPIClientSecret\").SecretValue" +
                "\r\n$client_secret = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($client_secret)" +
                "\r\n$client_secret = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($client_secret)" +
                "\r\n" +
                "\r\n$TenantId = (Get-AzKeyVaultSecret -VaultName $vault -Name \"TenantID\").SecretValue" +
                "\r\n$TenantId = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($TenantId)" +
                "\r\n$TenantId = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($TenantId)" +
                "\r\n" +
                "\r\n$archiveEmail = (Get-AzKeyVaultSecret -VaultName $vault -Name \"PrimarySystemAccountEmail\").SecretValue" +
                "\r\n$archiveEmail = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($archiveEmail)" +
                "\r\n$archiveEmail = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($archiveEmail)" +
                "\r\n" +
                "\r\n$global:environmentName = (Get-AzKeyVaultSecret -VaultName $vault -Name \"DynamicsEnvironmentName\").SecretValue" +
                "\r\n$global:environmentName = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($global:environmentName)" +
                "\r\n$global:environmentName = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($global:environmentName)" +
                "\r\n" +
                "\r\n$AccountsDBName = Get-AutomationVariable -Name 'AccountsDBName'" +
                "\r\n$SMSMessagesDBName = Get-AutomationVariable -Name 'SMSMessagesDBName'" +
                "\r\n$WhatsAppMessagesDBName = Get-AutomationVariable -Name 'WhatsAppMessagesDBName'" +
                "\r\n$startingprefix = Get-AutomationVariable -Name 'StartingPrefix'" +
                "\r\n$assignedtoDBColumn = Get-AutomationVariable -Name 'AssignedToDBColumn'" +
                "\r\n$phonenumberDBColumn = Get-AutomationVariable -Name 'PhoneNumberDBColumn'" +
                "\r\n$phonenumberidDBColumn = Get-AutomationVariable -Name 'PhoneNumberIDDBColumn'" +
                "\r\n$assigneduserDBColumn = Get-AutomationVariable -Name 'AssiedUserDBColumn'" +
                "\r\n$toDBColumn = Get-AutomationVariable -Name 'ToDBColumn'" +
                "\r\n$fromDBColumn = Get-AutomationVariable -Name 'FromDBColumn'" +
                "\r\n$timestampDBColumn = Get-AutomationVariable -Name 'TimestampDBColumn'" +
                "\r\n$messageDBColumn = Get-AutomationVariable -Name 'MessagesDBColumn'" +
                "\r\n" +
                "\r\n$AllSMSMessages = \"\";" +
                "\r\n$AllWhatsAppMessages = \"\";" +
                "\r\n" +
                "\r\n#get graph token" +
                "\r\n$tokenbody = @{" +
                "\r\n    client_id     = $AppId" +
                "\r\n    scope         = \"https://graph.microsoft.com/.default\"" +
                "\r\n    client_secret = $client_secret" +
                "\r\n    grant_type    = \"client_credentials\"" +
                "\r\n}" +
                "\r\n" +
                "\r\ntry { $tokenRequest = Invoke-WebRequest -Method Post -Uri \"https://login.microsoftonline.com/$TenantId/oauth2/v2.0/token\" -ContentType \"application/x-www-form-urlencoded\" -Body $tokenbody -UseBasicParsing -ErrorAction Stop }" +
                "\r\ncatch { Write-Host \"Unable to obtain access token, aborting...\"; return }" +
                "\r\n$token = ($tokenRequest.Content | ConvertFrom-Json).access_token" +
                "\r\nConnect-MgGraph -AccessToken $token" +
                "\r\n" +
                "\r\n<#$AuthHeader1 = @{" +
                "\r\n    'Authorization'=\"Bearer $token\"" +
                "\r\n }" +
                "\r\n $URL = \"https://graph.microsoft.com/v1.0/users('$archiveEmail')\"" +
                "\r\n $Users = Invoke-WebRequest -ContentType \"application/json\" -Headers $AuthHeader1 -Uri $URL" +
                "\r\n $result = ($Users.Content | ConvertFrom-Json).Value" +
                "\r\n $result#>" +
                "\r\n" +
                "\r\nfunction CreateSMSMessagesList" +
                "\r\n{" +
                "\r\n    Param" +
                "\r\n    (" +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $token," +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $email," +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $phonenumber" +
                "\r\n    )" +
                "\r\n    $_assigneduser = $startingprefix + $assigneduserDBColumn" +
                "\r\n    $_to = $startingprefix + $toDBColumn" +
                "\r\n    $_from = $startingprefix + $fromDBColumn" +
                "\r\n    $_timestamp = $startingprefix + $timestampDBColumn" +
                "\r\n    $_message = $startingprefix + $messageDBColumn" +
                "\r\n    $_id = $startingprefix + $SMSMessagesDBName.Substring(0, $SMSMessagesDBName.Length-2) + \"id\"" +
                "\r\n" +
                "\r\n    $select = \"?`$select=\" + $_assigneduser + \",\" + $_timestamp + \",\" + $_to + \",\" + $_from + \",\" + $_message" +
                "\r\n    $filter = \"&`$filter=\" + $_assigneduser + \"%20eq%20%27\" + $email + \"%27\"" +
                "\r\n    $query = $startingprefix + $SMSMessagesDBName + $select + $filter" +
                "\r\n    $serviceUrl = \"https://\" + $global:environmentName + \".crm.dynamics.com/\"" +
                "\r\n    $URL = $serviceUrl + \"api/data/v9.2/\" + $query" +
                "\r\n" +
                "\r\n    $headers = @{}" +
                "\r\n    $headers.Add(\"Authorization\", \"Bearer \" + $token)" +
                "\r\n    $headers.Add(\"OData-MaxVersion\", \"4.0\")" +
                "\r\n    $headers.Add(\"OData-Version\", \"4.0\")" +
                "\r\n" +
                "\r\n    $response = Invoke-WebRequest -ContentType \"application/json\" -Headers $headers -Uri $URL -UseBasicParsing" +
                "\r\n    $results = ($response.Content | ConvertFrom-Json).Value" +
                "\r\n    $messages = @()" +
                "\r\n    if ($global:basedon -eq 0)" +
                "\r\n    {" +
                "\r\n        #$DebugPreference = 'Continue'" +
                "\r\n        $withintimecounter = 0" +
                "\r\n        For ($i = 0; $i -lt $results.Length; $i++) {" +
                "\r\n            $enduser = $false" +
                "\r\n            if ($phonenumber -eq $results[$i].$_from) { $enduser = $false; }" +
                "\r\n            else { $enduser = $true; }" +
                "\r\n            if ([DateTime]$results[$i].$_timestamp -lt $global:TimeOffset)" +
                "\r\n            {" +
                "\r\n                $withintimecounter++" +
                "\r\n                $messages += @{" +
                "\r\n                    fromenduser = $enduser" +
                "\r\n                    message = $results[$i].$_message" +
                "\r\n                    timestamp = $results[$i].$_timestamp" +
                "\r\n                    from = $results[$i].$_from" +
                "\r\n                    to = $results[$i].$_to" +
                "\r\n                    id = $results[$i].$_id" +
                "\r\n                }" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        if ($withintimecounter -gt 0) { return $messages }" +
                "\r\n        else { return \"\" }" +
                "\r\n    }" +
                "\r\n    else" +
                "\r\n    {" +
                "\r\n        $withincountcounter = 0" +
                "\r\n        For ($i = 0; $i -lt $results.Length; $i++) {" +
                "\r\n            $enduser = $false" +
                "\r\n            if ($phonenumber -eq $results[$i].$_from) { $enduser = $false; }" +
                "\r\n            else { $enduser = $true; }" +
                "\r\n            if ($i -ge $global:messagecount)" +
                "\r\n            {" +
                "\r\n                $withincountcounter++" +
                "\r\n                $messages += @{" +
                "\r\n                    fromenduser = $enduser" +
                "\r\n                    message = $results[$i].$_message" +
                "\r\n                    timestamp = $results[$i].$_timestamp" +
                "\r\n                    from = $results[$i].$_from" +
                "\r\n                    to = $results[$i].$_to" +
                "\r\n                    id = $results[$i].$_id" +
                "\r\n                }" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        if ($withincountcounter -gt 0) { return $messages }" +
                "\r\n        else { return \"\" }" +
                "\r\n    }" +
                "\r\n}" +
                "\r\nfunction CreateWhatsAppMessagesList" +
                "\r\n{" +
                "\r\n    Param" +
                "\r\n    (" +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $token," +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $email," +
                "\r\n         [Parameter(Mandatory=$true)]" +
                "\r\n         [string] $phonenumber" +
                "\r\n    )" +
                "\r\n    $_assigneduser = $startingprefix + $assigneduserDBColumn" +
                "\r\n    $_to = $startingprefix + $toDBColumn" +
                "\r\n    $_from = $startingprefix + $fromDBColumn" +
                "\r\n    $_timestamp = $startingprefix + $timestampDBColumn" +
                "\r\n    $_message = $startingprefix + $messageDBColumn" +
                "\r\n    $_id = $startingprefix + $WhatsAppMessagesDBName.Substring(0, $WhatsAppMessagesDBName.Length-2) + \"id\"" +
                "\r\n" +
                "\r\n    $select = \"?`$select=\" + $_assigneduser + \",\" + $_timestamp + \",\" + $_to + \",\" + $_from + \",\" + $_message" +
                "\r\n    $filter = \"&`$filter=\" + $_assigneduser + \"%20eq%20%27\" + $email + \"%27\"" +
                "\r\n    $query = $startingprefix + $WhatsAppMessagesDBName + $select + $filter" +
                "\r\n    $serviceUrl = \"https://\" + $global:environmentName + \".crm.dynamics.com/\"" +
                "\r\n    $URL = $serviceUrl + \"api/data/v9.2/\" + $query" +
                "\r\n" +
                "\r\n    $headers = @{}" +
                "\r\n    $headers.Add(\"Authorization\", \"Bearer \" + $token)" +
                "\r\n    $headers.Add(\"OData-MaxVersion\", \"4.0\")" +
                "\r\n    $headers.Add(\"OData-Version\", \"4.0\")" +
                "\r\n" +
                "\r\n    $response = Invoke-WebRequest -ContentType \"application/json\" -Headers $headers -Uri $URL -UseBasicParsing" +
                "\r\n    $results = ($response.Content | ConvertFrom-Json).Value" +
                "\r\n" +
                "\r\n    $messages = @()" +
                "\r\n    if ($global:basedon -eq 0)" +
                "\r\n    {" +
                "\r\n        $withintimecounter = 0" +
                "\r\n        #$DebugPreference = 'Continue'" +
                "\r\n        For ($i = 0; $i -lt $results.Length; $i++) {" +
                "\r\n            $enduser = $false" +
                "\r\n            if ($phonenumber -eq $results[$i].$_from) { $enduser = $false; }" +
                "\r\n            else { $enduser = $true; }" +
                "\r\n            try{" +
                "\r\n                if ([DateTime]$results[$i].$_timestamp -lt $global:TimeOffset)" +
                "\r\n                {" +
                "\r\n                    $withintimecounter++" +
                "\r\n                    $messages += @{" +
                "\r\n                        fromenduser = $enduser" +
                "\r\n                        message = $results[$i].$_message" +
                "\r\n                        timestamp = $results[$i].$_timestamp" +
                "\r\n                        from = $results[$i].$_from" +
                "\r\n                        to = $results[$i].$_to" +
                "\r\n                        id = $results[$i].$_id" +
                "\r\n                    }" +
                "\r\n                }" +
                "\r\n            }" +
                "\r\n            catch{" +
                "\r\n                $timee = (Get-Date 01.01.1970) + ([System.TimeSpan]::fromseconds($results[$i].$_timestamp))" +
                "\r\n                if ($timee -lt $global:TimeOffset)" +
                "\r\n                {" +
                "\r\n                    $withintimecounter++" +
                "\r\n                    $messages += @{" +
                "\r\n                        fromenduser = $enduser" +
                "\r\n                        message = $results[$i].$_message" +
                "\r\n                        timestamp = $results[$i].$_timestamp" +
                "\r\n                        from = $results[$i].$_from" +
                "\r\n                        to = $results[$i].$_to" +
                "\r\n                        id = $results[$i].$_id" +
                "\r\n                    }" +
                "\r\n                }" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        if ($withintimecounter -gt 0) { return $messages }" +
                "\r\n        else { return \"\" }" +
                "\r\n    }" +
                "\r\n    else" +
                "\r\n    {" +
                "\r\n        $withincountcounter = 0" +
                "\r\n        $DebugPreference = 'Continue'" +
                "\r\n        For ($i = 0; $i -lt $results.Length; $i++) {" +
                "\r\n            $enduser = $false" +
                "\r\n            if ($phonenumber -eq $results[$i].$_from) { $enduser = $false; }" +
                "\r\n            else { $enduser = $true; }" +
                "\r\n            if ($i -ge $global:messagecount)" +
                "\r\n            {" +
                "\r\n                $withincountcounter++" +
                "\r\n                $messages += @{" +
                "\r\n                    fromenduser = $enduser" +
                "\r\n                    message = $results[$i].$_message" +
                "\r\n                    timestamp = $results[$i].$_timestamp" +
                "\r\n                    from = $results[$i].$_from" +
                "\r\n                    to = $results[$i].$_to" +
                "\r\n                    id = $results[$i].$_id" +
                "\r\n                }" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        if ($withincountcounter -gt 0) { return $messages }" +
                "\r\n        else { return \"\" }" +
                "\r\n    }" +
                "\r\n}" +
                "\r\nFunction CreateAccountsList($token) {" +
                "\r\n    #$DebugPreference = 'Continue'" +
                "\r\n    $_assignedto = $startingprefix + $assignedtoDBColumn" +
                "\r\n    $_phonenumber = $startingprefix + $phonenumberDBColumn" +
                "\r\n    $_phonenumberid = $startingprefix + $phonenumberidDBColumn" +
                "\r\n" +
                "\r\n    $select = \"?`$select=\" + $_assignedto + \",\" + $_phonenumber + \",\" + $_phonenumberid" +
                "\r\n    $query = $startingprefix + $AccountsDBName + $select" +
                "\r\n    $serviceUrl = \"https://\" + $global:environmentName + \".crm.dynamics.com/\"" +
                "\r\n" +
                "\r\n    $URL = $serviceUrl + \"api/data/v9.2/\" + $query" +
                "\r\n    $headers = @{}" +
                "\r\n    $headers.Add(\"Authorization\", \"Bearer \" + $token)" +
                "\r\n    $headers.Add(\"OData-MaxVersion\", \"4.0\")" +
                "\r\n    $headers.Add(\"OData-Version\", \"4.0\")" +
                "\r\n" +
                "\r\n    $response = Invoke-WebRequest -ContentType \"application/json\" -Headers $headers -Uri $URL -UseBasicParsing" +
                "\r\n    $results = ($response.Content | ConvertFrom-Json).Value" +
                "\r\n" +
                "\r\n    $accounts = @()" +
                "\r\n    For ($i = 0; $i -lt $results.Length; $i++) {" +
                "\r\n        $accounts += @{" +
                "\r\n            AssignedTo = $results[$i].$_assignedto" +
                "\r\n            PhoneNumber = $results[$i].$_phonenumber.Trim()" +
                "\r\n            PhoneNumberID = $results[$i].$_phonenumberid.Trim()" +
                "\r\n        }" +
                "\r\n    }" +
                "\r\n    return $accounts" +
                "\r\n}" +
                "\r\nFunction CreateHTMLEmlString {" +
                "\r\n    Param(" +
                "\r\n    [Parameter(Mandatory=$true)]" +
                "\r\n    [array]$messages," +
                "\r\n    [Parameter(Mandatory=$true)]" +
                "\r\n    [string]$assignedUser," +
                "\r\n    [Parameter(Mandatory=$true)]" +
                "\r\n    [string]$phoneNumber," +
                "\r\n    [Parameter(Mandatory=$true)]" +
                "\r\n    [string]$phoneNumberID," +
                "\r\n    [Parameter(Mandatory=$true)]" +
                "\r\n    [string]$email" +
                "\r\n    )" +
                "\r\n" +
                "\r\n    $messagesBuilder = \"\"" +
                "\r\n    $messagesBuilder += \"Date: $(Get-Date -Format \"M/d/yyyy\")`n\"" +
                "\r\n    $messagesBuilder += \"From: $email`n\"" +
                "\r\n    $messagesBuilder += \"To: $email`n\"" +
                "\r\n    $messagesBuilder += \"Content-Type: text/html; charset=utf-8`n`n\"" +
                "\r\n    $messagesBuilder += \"<html><body>\"" +
                "\r\n    $messagesBuilder += \"<br>Assigned User: $assignedUser\"" +
                "\r\n    $messagesBuilder += \"<br>Phone Number: $phoneNumber\"" +
                "\r\n    $messagesBuilder += \"<br>Phone Number ID: $phoneNumberID\"" +
                "\r\n    $messagesBuilder += \"<br>Archived Messages:<br>\"" +
                "\r\n" +
                "\r\n    foreach ($message in $messages) {" +
                "\r\n        $message.fromenduser" +
                "\r\n        try{$time = [datetime]::Parse($message.timestamp)}" +
                "\r\n        catch{" +
                "\r\n            $origin = New-Object -Type DateTime -ArgumentList 1970, 1, 1, 0, 0, 0, 0" +
                "\r\n            $time = $origin.AddSeconds($message.timestamp)" +
                "\r\n        }" +
                "\r\n        if (-not $message.fromenduser) {" +
                "\r\n            $messagesBuilder += \"<div align=right>\"" +
                "\r\n            $messagesBuilder += \"<div style='font-size: 13px; background-color: #eeeefa; border-width: 3px; border-color :#F5F5F5; display: inline-block; border-radius: 15px; padding: 10px'>\"" +
                "\r\n" +
                "\r\n            $messagesBuilder += \"To:\" + $message.to" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"From: \" + $message.from" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"Timestamp: \" + $time.ToShortDateString() + \" \" + $time.ToShortTimeString()" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"Message: \" + $message.message" +
                "\r\n        }" +
                "\r\n        else {" +
                "\r\n            $messagesBuilder += \"<div align=left>\"" +
                "\r\n            $messagesBuilder += \"<div style='font-size: 13px; background-color: #ffffff; border-width: 3px; border-color: #F5F5F5; display: inline-block; border-radius: 15px; padding: 10px'>\"" +
                "\r\n" +
                "\r\n            $messagesBuilder += \"To:\" + $message.to" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"From: \" + $message.from" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"Timestamp: \" + $time.ToShortDateString() + \" \" + $time.ToShortTimeString()" +
                "\r\n            $messagesBuilder += \"<br>\"" +
                "\r\n            $messagesBuilder += \"Message: \" + $message.message" +
                "\r\n        }" +
                "\r\n        $messagesBuilder += \"</div>\"" +
                "\r\n        $messagesBuilder += \"</div>\"" +
                "\r\n    }" +
                "\r\n" +
                "\r\n    $messagesBuilder += \"</html></body>\"" +
                "\r\n    return $messagesBuilder" +
                "\r\n}" +
                "\r\n" +
                "\r\n$tokenbody = @{" +
                "\r\n    client_id     = $AppId" +
                "\r\n    scope         = \"https://\" + $global:environmentName + \".crm.dynamics.com/.default\"" +
                "\r\n    client_secret = $client_secret" +
                "\r\n    grant_type    = \"client_credentials\"" +
                "\r\n}" +
                "\r\n$tokenRequest = Invoke-WebRequest -Method Post -Uri \"https://login.microsoftonline.com/$TenantId/oauth2/v2.0/token\" -ContentType \"application/x-www-form-urlencoded\" -Body $tokenbody -UseBasicParsing -ErrorAction Stop" +
                "\r\n$responsedata = $tokenRequest.Content" +
                "\r\n$responsedata = ConvertFrom-Json -InputObject $responsedata" +
                "\r\n$token = $responsedata.access_token" +
                "\r\n" +
                "\r\n$Accounts = CreateAccountsList($token)" +
                "\r\n" +
                "\r\nforeach($o in $Accounts)" +
                "\r\n{" +
                "\r\n    $_email = $o[\"AssignedTo\"];" +
                "\r\n    $_phonenumber = $o[\"PhoneNumber\"];" +
                "\r\n    $_phonenumberid = $o[\"PhoneNumberID\"];" +
                "\r\n" +
                "\r\n    $sms = CreateSMSMessagesList -token $token -email $_email -phonenumber $_phonenumber;" +
                "\r\n    if ($sms -ne \"\")" +
                "\r\n    {" +
                "\r\n        $AllSMSMessages = CreateHTMLEmlString -messages $sms -assignedUser $_email -phoneNumber $_phonenumber -phoneNumberID $_phonenumberid -email $archiveEmail" +
                "\r\n" +
                "\r\n        $emailsubject = \"Nightly SMS Report At: \" + (Get-Date).GetDateTimeFormats()[9]" +
                "\r\n        $emailbody = \"Source: Automated Powershell`nType: SMS\";" +
                "\r\n        $params = @{" +
                "\r\n            Message = @{" +
                "\r\n                Subject = $emailsubject" +
                "\r\n                Body = @{" +
                "\r\n                    ContentType = \"Text\"" +
                "\r\n                    Content = $emailbody" +
                "\r\n                }" +
                "\r\n                ToRecipients = @(" +
                "\r\n                    @{" +
                "\r\n                        EmailAddress = @{" +
                "\r\n                            Address = $archiveEmail" +
                "\r\n                        }" +
                "\r\n                    }" +
                "\r\n                )" +
                "\r\n                Attachments = @(" +
                "\r\n                    @{" +
                "\r\n                        \"@odata.type\" = \"#microsoft.graph.fileAttachment\"" +
                "\r\n                        Name = \"Archive.eml\"" +
                "\r\n                        ContentType = \"text/plain\"" +
                "\r\n                        ContentBytes = [System.Text.Encoding]::Default.GetBytes($AllSMSMessages)" +
                "\r\n                    }" +
                "\r\n                )" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        Send-MgUserMail -UserId $archiveEmail -BodyParameter $params" +
                "\r\n" +
                "\r\n        $headers = @{}" +
                "\r\n        $headers.Add(\"Authorization\", \"Bearer \" + $token)" +
                "\r\n        $headers.Add(\"OData-MaxVersion\", \"4.0\")" +
                "\r\n        $headers.Add(\"OData-Version\", \"4.0\")" +
                "\r\n" +
                "\r\n        foreach($item in $sms)" +
                "\r\n        {" +
                "\r\n            $URL = \"https://\" + $global:environmentName + \".crm.dynamics.com/api/data/v9.2/\" + $startingprefix + $SMSMessagesDBName + \"(\" + $item.id + \")\"" +
                "\r\n            $response = Invoke-RestMethod -Method Delete -ContentType \"application/json\" -Headers $headers -Uri $URL" +
                "\r\n            $results = ($response.Content | ConvertFrom-Json).Value" +
                "\r\n        }" +
                "\r\n    }" +
                "\r\n" +
                "\r\n    $whatsapp = CreateWhatsAppMessagesList -token $token -email $_email -phonenumber $_phonenumberid;" +
                "\r\n    if ($whatsapp -ne \"\")" +
                "\r\n    {" +
                "\r\n        $AllWhatsAppMessages = CreateHTMLEmlString -messages $whatsapp -assignedUser $_email -phoneNumber $_phonenumber -phoneNumberID $_phonenumberid -email $archiveEmail" +
                "\r\n" +
                "\r\n        $emailsubject = \"Nightly WhatsApp Report At: \" + (Get-Date).GetDateTimeFormats()[9]" +
                "\r\n        $emailbody = \"Source: Automated PowerShell`nType: WhatsApp\";" +
                "\r\n        $params = @{" +
                "\r\n            Message = @{" +
                "\r\n                Subject = $emailsubject" +
                "\r\n                Body = @{" +
                "\r\n                    ContentType = \"Text\"" +
                "\r\n                    Content = $emailbody" +
                "\r\n                }" +
                "\r\n                ToRecipients = @(" +
                "\r\n                    @{" +
                "\r\n                        EmailAddress = @{" +
                "\r\n                            Address = $archiveEmail" +
                "\r\n                        }" +
                "\r\n                    }" +
                "\r\n                )" +
                "\r\n                Attachments = @(" +
                "\r\n                    @{" +
                "\r\n                        \"@odata.type\" = \"#microsoft.graph.fileAttachment\"" +
                "\r\n                        Name = \"Archive.eml\"" +
                "\r\n                        ContentType = \"text/plain\"" +
                "\r\n                        ContentBytes = [System.Text.Encoding]::Default.GetBytes($AllWhatsAppMessages)" +
                "\r\n                    }" +
                "\r\n                )" +
                "\r\n            }" +
                "\r\n        }" +
                "\r\n        Send-MgUserMail -UserId $archiveEmail -BodyParameter $params" +
                "\r\n" +
                "\r\n        $headers = @{}" +
                "\r\n        $headers.Add(\"Authorization\", \"Bearer \" + $token)" +
                "\r\n        $headers.Add(\"OData-MaxVersion\", \"4.0\")" +
                "\r\n        $headers.Add(\"OData-Version\", \"4.0\")" +
                "\r\n" +
                "\r\n        foreach($item in $whatsapp)" +
                "\r\n        {" +
                "\r\n            $URL = \"https://\" + $global:environmentName + \".crm.dynamics.com/api/data/v9.2/\" + $startingprefix + $WhatsAppMessagesDBName + \"(\" + $item.id + \")\"" +
                "\r\n            $response = Invoke-RestMethod -Method Delete -ContentType \"application/json\" -Headers $headers -Uri $URL" +
                "\r\n            $results = ($response.Content | ConvertFrom-Json).Value" +
                "\r\n        }" +
                "\r\n    }" +
                "\r\n}";
        }
        static string AutoPowerShellCosmosArchiver()
        {
            return "$global:messagecount = \"100\"" +
                "\r\n$global:TimeOffset = (Get-Date).AddMonths(-1)" +
                "\r\n" +
                "\r\ntry" +
                "\r\n{" +
                "\r\n    Connect-AzAccount -Identity | Out-Null" +
                "\r\n}" +
                "\r\ncatch {" +
                "\r\n    Write-Error -Message $_.Exception" +
                "\r\n    throw $_.Exception" +
                "\r\n}" +
                "\r\n" +
                "\r\n$token = (Get-AzAccessToken).Token" +
                "\r\n#comment out the one you don't want to use here" +
                "\r\n$body = @{" +
                "\r\n    token = $token" +
                "\r\n    type = \"0\"" +
                "\r\n    count = $global:messagecount" +
                "\r\n    #time = $global:TimeOffset" +
                "\r\n}" +
                "\r\n$json = $body | ConvertTo-Json" +
                "\r\n" +
                "\r\n$vault = Get-AutomationVariable -Name 'InternalVault'" +
                "\r\n" +
                "\r\n$RestSite = (Get-AzKeyVaultSecret -VaultName $vault -Name \\\"CosmosRestSite\\\").SecretValue" +
                "\r\n$RestSite = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($RestSite)" +
                "\r\n$RestSite = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($RestSite)" +
                "\r\n" +
                "\r\n$CosmosRestURL = \"https://\" + $RestSite + \".azurewebsites.net/api/Function1\"" +
                "\r\n" +
                "\r\n$response = Invoke-RestMethod -Method Post -ContentType \"application/json\" -Uri $CosmosRestURL -Body $json" +
                "\r\n$response";
        }

        static async Task<Guid> CreateRunbook(AutomationAccountResource response, string code, string runbookname, AzureLocation SelectedRegion)
        {
            AutomationRunbookResource runbook = (await response.GetAutomationRunbooks().CreateOrUpdateAsync(
                WaitUntil.Completed,
                runbookname,
                new(AutomationRunbookType.PowerShell) { Location = SelectedRegion })).Value;

            try
            {
                using MemoryStream stream = new();
                using StreamWriter writer = new(stream);
                writer.Write(code);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                _ = await runbook.ReplaceContentRunbookDraftAsync(WaitUntil.Completed, stream);
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

#pragma warning disable CS8629 // Nullable value type may be null.
            return response.Data.Identity.PrincipalId.Value;
#pragma warning restore CS8629 // Nullable value type may be null.
        }
        async Task<Guid> CreateAutomationAccount(string desiredCosmosAccountName, string internalVaultName, CosmosDeploy form)
        {
            AutomationAccountResource response = await CreateVariables(
                form.SelectedGroup,
                form.SelectedRegion,
                desiredCosmosAccountName,
                internalVaultName);

#pragma warning disable CS8629 // Nullable value type may be null.
            ResourceIdentifier contributor = ResourceIdentifier.Parse("/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c");
            RoleAssignmentCreateOrUpdateContent authorizationroledefinition = new(contributor, response.Data.Identity.PrincipalId.Value)
            {
                PrincipalType = RoleManagementPrincipalType.ServicePrincipal
            };
            try { await form.SelectedGroup.GetRoleAssignments().CreateOrUpdateAsync(WaitUntil.Completed, Guid.NewGuid().ToString(), authorizationroledefinition); } catch { }
#pragma warning restore CS8629 // Nullable value type may be null.

            _ = await CreateRunbook(response, AutoPowerShellCosmosArchiver(), "AutoArchiver ", form.SelectedRegion);

            return await CreateRunbook(response, AutoPowerShellKeyCode(), "AutoRotation", form.SelectedRegion);
        }
        async Task<Guid> CreateAutomationAccount(JSONDefaultDataverseLibrary dataverseLibrary, JSONSecretNames secretNames, string internalVaultName, DataverseDeploy form)
        {
            AutomationAccountResource response = await CreateVariables(
                dataverseLibrary,
                form.SelectedGroup,
                form.SelectedRegion,
                secretNames,
                internalVaultName);

#pragma warning disable CS8629 // Nullable value type may be null.
            ResourceIdentifier contributor = ResourceIdentifier.Parse("/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c");
            RoleAssignmentCreateOrUpdateContent authorizationroledefinition = new(contributor, response.Data.Identity.PrincipalId.Value)
            {
                PrincipalType = RoleManagementPrincipalType.ServicePrincipal
            };
            try { await form.SelectedGroup.GetRoleAssignments().CreateOrUpdateAsync(WaitUntil.Completed, Guid.NewGuid().ToString(), authorizationroledefinition); } catch { }
#pragma warning restore CS8629 // Nullable value type may be null.

            _ = await CreateRunbook(response, AutoPowerShellDataverseArchiver(), "AutoArchiver", form.SelectedRegion);

            return await CreateRunbook(response, AutoPowerShellKeyCode(), "AutoRotation", form.SelectedRegion);
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
