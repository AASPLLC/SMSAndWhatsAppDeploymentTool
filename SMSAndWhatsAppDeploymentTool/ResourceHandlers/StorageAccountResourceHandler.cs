using Azure.ResourceManager.Storage.Models;
using Azure;
using Azure.ResourceManager.Storage;
using Azure.Core;
using Azure.ResourceManager.Resources;
using SMSAndWhatsAppDeploymentTool.StepByStep;
using System.Windows.Forms;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class StorageAccountResourceHandler
    {
        static string GetDesiredStorageName(string desiredStorageName, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetStorageAccounts())
            {
                desiredStorageName = item.Data.Name;
                break;
            }
            return desiredStorageName;
        }

        internal virtual async Task<bool> InitialCreation(string desiredStorageName, StepByStepValues sbs)
        {
            foreach (var item in sbs.SelectedGroup.GetStorageAccounts())
            {
                desiredStorageName = item.Data.Name;
                break;
            }
            sbs.DesiredStorageName = desiredStorageName;
            if (await CheckStorageAccountName(desiredStorageName, sbs))
            {
                await CreateStorageAccountResource(desiredStorageName, sbs);
            }
            if (desiredStorageName == "")
                return false;
            else
                return true;
        }
        internal virtual async Task<(StorageAccountResource, string)> InitialCreation(string desiredStorageName, DataverseDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            if (await CheckStorageAccountName(desiredStorageName, form))
            {
                return await CreateStorageAccountResource(desiredStorageName, form);
            }
            else
            {
                return await SkipStorageAccount(desiredStorageName, form);
            }
        }
        internal virtual async Task<(StorageAccountResource, string)> InitialCreation(string desiredStorageName, CosmosDeploy form)
        {
            desiredStorageName = GetDesiredStorageName(desiredStorageName, form.SelectedGroup);
            if (await CheckStorageAccountName(desiredStorageName, form))
            {
                return await CreateStorageAccountResource(desiredStorageName, form);
            }
            else
            {
                return await SkipStorageAccount(desiredStorageName, form);
            }
        }

        internal virtual async Task CreateStorageAccountNetworkRuleSet(ResourceGroupResource SelectedGroup, AzureLocation SelectedRegion, ResourceIdentifier vnetId, string[] outboundips, string desiredName)
        {
            StorageAccountNetworkRuleSet networkRules = new(StorageNetworkDefaultAction.Deny)
            {
                Bypass = StorageNetworkBypass.AzureServices
            };
            for (int i = 0; i < outboundips.Length; i++)
            {
                networkRules.IPRules.Add(new StorageAccountIPRule(outboundips[i]) { Action = StorageAccountNetworkRuleAction.Allow });
            }
            StorageAccountVirtualNetworkRule virtualNetworkRule = new(vnetId)
            {
                Action = StorageAccountNetworkRuleAction.Allow,
                State = StorageAccountNetworkRuleState.Succeeded
            };

            networkRules.VirtualNetworkRules.Add(virtualNetworkRule);
            _ = await SelectedGroup.GetStorageAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new StorageAccountCreateOrUpdateContent(new StorageSku(StorageSkuName.StandardLrs), StorageKind.Storage, SelectedRegion)
            { NetworkRuleSet = networkRules });
        }

        static async Task<(StorageAccountResource, string)> CreateStorageAccountResource(string desiredName, StepByStepValues sbs)
        {
            Console.Write(Environment.NewLine + "Waiting for Storage Account Creation");

            StorageAccountEncryption storageAccountEncryption = new()
            {
                KeySource = "Microsoft.Storage"
            };

            StorageAccountEncryptionServices storageAccountEncryptionServices = new()
            {
                File = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                },
                Blob = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                }
                //may need to add Queue
                /*storageAccountEncryptionServices.Queue = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                };*/

            };
            storageAccountEncryption.Services = storageAccountEncryptionServices;

            //these settings will likely need to change
            var storageAccountResponse = await sbs.SelectedGroup.GetStorageAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new StorageAccountCreateOrUpdateContent(new StorageSku(StorageSkuName.StandardLrs), StorageKind.Storage, sbs.SelectedRegion)
            {
                IsDefaultToOAuthAuthentication = true,
                PublicNetworkAccess = StoragePublicNetworkAccess.Enabled,
                MinimumTlsVersion = StorageMinimumTlsVersion.Tls1_0,
                AllowBlobPublicAccess = false,
                AllowSharedKeyAccess = true,
                //NetworkRuleSet = networkRules,
                EnableHttpsTrafficOnly = true,
                Encryption = storageAccountEncryption
            });

            int counter = 0;
            var finishedStorageAccount = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                try { Console.Write(", " + counter); } catch { sbs.InvokableText.Invoke(() => { sbs.InvokableText.Text += ", " + counter; }); }
                return storageAccountResponse.WaitForCompletionResponse();
            });

            var storageAccountResource = storageAccountResponse.Value;

            var queueResponse = await storageAccountResource.GetQueueService().GetStorageQueues().CreateOrUpdateAsync(WaitUntil.Completed, "smsq", new StorageQueueData());

            Page<StorageAccountKey>[] keys = storageAccountResource.GetKeys().AsPages().ToArray();

            Console.Write(Environment.NewLine + "Storage account created");

            return (storageAccountResource, keys[0].Values[0].Value);
        }
        static async Task<(StorageAccountResource, string)> CreateStorageAccountResource(string desiredName, DataverseDeploy form)
            {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Storage Account Creation";

            StorageAccountEncryption storageAccountEncryption = new()
            {
                KeySource = "Microsoft.Storage"
            };

            StorageAccountEncryptionServices storageAccountEncryptionServices = new()
            {
                File = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                },
                Blob = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                }
                //may need to add Queue
                /*storageAccountEncryptionServices.Queue = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                };*/

            };
            storageAccountEncryption.Services = storageAccountEncryptionServices;

            //these settings will likely need to change
            var storageAccountResponse = await form.SelectedGroup.GetStorageAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new StorageAccountCreateOrUpdateContent(new StorageSku(StorageSkuName.StandardLrs), StorageKind.Storage, form.SelectedRegion)
            {
                IsDefaultToOAuthAuthentication = true,
                PublicNetworkAccess = StoragePublicNetworkAccess.Enabled,
                MinimumTlsVersion = StorageMinimumTlsVersion.Tls1_0,
                AllowBlobPublicAccess = false,
                AllowSharedKeyAccess = true,
                //NetworkRuleSet = networkRules,
                EnableHttpsTrafficOnly = true,
                Encryption = storageAccountEncryption
            });

            int counter = 0;
            var finishedStorageAccount = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                try { form.OutputRT.Text += ", " + counter; } catch { form.OutputRT.Invoke(() => { form.OutputRT.Text += ", " + counter; }); }
                return storageAccountResponse.WaitForCompletionResponse();
            });

            var storageAccountResource = storageAccountResponse.Value;

            var queueResponse = await storageAccountResource.GetQueueService().GetStorageQueues().CreateOrUpdateAsync(WaitUntil.Completed, "smsq", new StorageQueueData());

            Page<StorageAccountKey>[] keys = storageAccountResource.GetKeys().AsPages().ToArray();

            form.OutputRT.Text += Environment.NewLine + "Storage account created";

            return (storageAccountResource, keys[0].Values[0].Value);
        }
        static async Task<(StorageAccountResource, string)> CreateStorageAccountResource(string desiredName, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Storage Account Creation";

            StorageAccountEncryption storageAccountEncryption = new()
            {
                KeySource = "Microsoft.Storage"
            };

            StorageAccountEncryptionServices storageAccountEncryptionServices = new()
            {
                File = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                },
                Blob = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                }
                //may need to add Queue
                /*storageAccountEncryptionServices.Queue = new StorageEncryptionService()
                {
                    KeyType = "Account",
                    IsEnabled = true
                };*/

            };
            storageAccountEncryption.Services = storageAccountEncryptionServices;

            //these settings will likely need to change
            var storageAccountResponse = await form.SelectedGroup.GetStorageAccounts().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new StorageAccountCreateOrUpdateContent(new StorageSku(StorageSkuName.StandardLrs), StorageKind.Storage, form.SelectedRegion)
            {
                IsDefaultToOAuthAuthentication = true,
                PublicNetworkAccess = StoragePublicNetworkAccess.Enabled,
                MinimumTlsVersion = StorageMinimumTlsVersion.Tls1_0,
                AllowBlobPublicAccess = false,
                AllowSharedKeyAccess = true,
                //NetworkRuleSet = networkRules,
                EnableHttpsTrafficOnly = true,
                Encryption = storageAccountEncryption
            });

            int counter = 0;
            var finishedStorageAccount = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                try { form.OutputRT.Text += ", " + counter; } catch { form.OutputRT.Invoke(() => { form.OutputRT.Text += ", " + counter; }); }
                return storageAccountResponse.WaitForCompletionResponse();
            });

            var storageAccountResource = storageAccountResponse.Value;

            var queueResponse = await storageAccountResource.GetQueueService().GetStorageQueues().CreateOrUpdateAsync(WaitUntil.Completed, "smsq", new StorageQueueData());

            Page<StorageAccountKey>[] keys = storageAccountResource.GetKeys().AsPages().ToArray();

            form.OutputRT.Text += Environment.NewLine + "Storage account created";

            return (storageAccountResource, keys[0].Values[0].Value);
        }

        static async Task<(StorageAccountResource, string)> SkipStorageAccount(string desiredName, DataverseDeploy form)
        {
            var temp = (await form.SelectedGroup.GetStorageAccountAsync(desiredName)).Value;
            Page<StorageAccountKey>[] keys = temp.GetKeys().AsPages().ToArray();

            _ = await temp.GetQueueService().GetStorageQueues().CreateOrUpdateAsync(WaitUntil.Completed, "smsq", new StorageQueueData());

            return (temp, keys[0].Values[0].Value);
        }
        static async Task<(StorageAccountResource, string)> SkipStorageAccount(string desiredName, CosmosDeploy form)
        {
            var temp = (await form.SelectedGroup.GetStorageAccountAsync(desiredName)).Value;
            Page<StorageAccountKey>[] keys = temp.GetKeys().AsPages().ToArray();

            _ = await temp.GetQueueService().GetStorageQueues().CreateOrUpdateAsync(WaitUntil.Completed, "smsq", new StorageQueueData());

            return (temp, keys[0].Values[0].Value);
        }

        static async Task<bool> CheckStorageAccountName(string desiredName, StepByStepValues sbs)
        {
            desiredName = desiredName.Trim();
            try
            {
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Storage name is empty and an existing service could not be found.");
                    return false;
                }
                _ = await sbs.SelectedGroup.GetStorageAccountAsync(desiredName);
                Console.Write(Environment.NewLine + desiredName + " already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await sbs.SelectedSubscription.CheckStorageAccountNameAvailabilityAsync(new StorageAccountNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        Console.Write(Environment.NewLine + "Storage Account name has already been taken, try another.");
                        return false;
                    }
                }
                else
                {
                    Console.Write(Environment.NewLine + "Storage Account text is empty.");
                    return false;
                }
            }
        }
        static async Task<bool> CheckStorageAccountName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Storage name is empty and an existing service could not be found.");
                    return false;
                }
                _ = await form.SelectedGroup.GetStorageAccountAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckStorageAccountNameAvailabilityAsync(new StorageAccountNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Storage Account name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Storage Account text is empty.";
                    return false;
                }
            }
        }
        static async Task<bool> CheckStorageAccountName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                if (desiredName == "")
                {
                    Console.Write(Environment.NewLine + "Storage name is empty and an existing service could not be found.");
                    return false;
                }
                _ = await form.SelectedGroup.GetStorageAccountAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                if (desiredName != "")
                {
                    var nameResponse = await form.SelectedSubscription.CheckStorageAccountNameAvailabilityAsync(new StorageAccountNameAvailabilityContent(desiredName));

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Storage Account name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + "Storage Account text is empty.";
                    return false;
                }
            }
        }
    }
}
