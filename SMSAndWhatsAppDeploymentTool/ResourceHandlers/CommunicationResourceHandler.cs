using Azure.Core;
using Azure.ResourceManager.Communication;
using Azure.ResourceManager;
using Azure;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Communication.Models;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class CommunicationResourceHandler
    {
        static string GetDesiredCommsName(string desiredCommsname, ResourceGroupResource SelectedGroup)
        {
            foreach (var item in SelectedGroup.GetCommunicationServiceResources())
            {
                desiredCommsname = item.Data.Name;
                break;
            }
            return desiredCommsname;
        }

        internal virtual async Task InitialCreation(string desiredCommunicationsName, StepByStepValues sbs)
        {
            foreach (var item in sbs.SelectedGroup.GetCommunicationServiceResources())
            {
                desiredCommunicationsName = item.Data.Name;
                break;
            }
            if (await CheckCommsName(desiredCommunicationsName, sbs))
            {
                await CreateAzureCommunicationsResource(desiredCommunicationsName, sbs);
                sbs.DesiredCommsName = desiredCommunicationsName;
            }
            await sbs.CreateSMSConnectionStringSecret();
        }
        internal virtual async Task<(ResourceIdentifier, string)> InitialCreation(string desiredCommunicationsName, DataverseDeploy form)
        {
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            ResourceIdentifier smsIdentity = new("1");
            string smsEndpoint = "";
            bool alreadyFoundOne = false;
            foreach (var item in form.SelectedGroup.GetCommunicationServiceResources())
            {
                alreadyFoundOne = true;
                (smsIdentity, smsEndpoint) = await SkipComms(item);
                break;
            }
            if (!alreadyFoundOne)
            {
                if (await CheckCommsName(desiredCommunicationsName, form))
                {
                    (smsIdentity, smsEndpoint) = await CreateAzureCommunicationsResource(desiredCommunicationsName, form);
                }
                //TODO: all unique names need this check before continuing
                else if (!form.OutputRT.Text.Contains("Azure Communications name has already been taken, try another."))
                {
                    (smsIdentity, smsEndpoint) = await SkipComms(form.SelectedGroup, desiredCommunicationsName);
                }
            } //this else should be impossible
            return (smsIdentity, smsEndpoint);
        }
        internal virtual async Task<(ResourceIdentifier, string)> InitialCreation(string desiredCommunicationsName, CosmosDeploy form)
        {
            desiredCommunicationsName = GetDesiredCommsName(desiredCommunicationsName, form.SelectedGroup);
            ResourceIdentifier smsIdentity = new("1");
            string smsEndpoint = "";
            bool alreadyFoundOne = false;
            foreach (var item in form.SelectedGroup.GetCommunicationServiceResources())
            {
                alreadyFoundOne = true;
                (smsIdentity, smsEndpoint) = await SkipComms(item);
                break;
            }
            if (!alreadyFoundOne)
            {
                if (await CheckCommsName(desiredCommunicationsName, form))
                {
                    (smsIdentity, smsEndpoint) = await CreateAzureCommunicationsResource(desiredCommunicationsName, form);
                }
                //TODO: all unique names need this check before continuing
                else if (!form.OutputRT.Text.Contains("Azure Communications name has already been taken, try another."))
                {
                    (smsIdentity, smsEndpoint) = await SkipComms(form.SelectedGroup, desiredCommunicationsName);
                }
            }
            return (smsIdentity, smsEndpoint);
        }

        static async Task CreateAzureCommunicationsResource(string desiredName, StepByStepValues sbs)
        {
            Console.Write(Environment.NewLine + "Waiting for Azure Communication Service Creation");

            _ = await sbs.SelectedGroup.GetCommunicationServiceResources().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new CommunicationServiceResourceData("global")
            {
                DataLocation = "unitedstates"
            });

            Console.Write(Environment.NewLine + "Azure Communication Service Created.");
        }
        static async Task<bool> CheckCommsName(string desiredName, StepByStepValues sbs)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = (await (await sbs.SelectedGroup.GetCommunicationServiceResourceAsync(desiredName)).Value.GetKeysAsync()).Value.PrimaryConnectionString;
                sbs.DesiredCommsName = desiredName;
                Console.Write(Environment.NewLine + desiredName + " already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    var nameResponse = await sbs.SelectedSubscription.CheckCommunicationNameAvailabilityAsync(new CommunicationServiceNameAvailabilityContent()
                    {
                        Name = desiredName,
                        ResourceType = "Microsoft.Communication/CommunicationServices"
                    });

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        Console.Write(Environment.NewLine + "Azure Communications name has already been taken, try another.");
                        return false;
                    }
                }
                else
                {
                    Console.Write(Environment.NewLine + ex.Message);
                    return false;
                }
            }
        }
        static async Task<(ResourceIdentifier, string)> CreateAzureCommunicationsResource(string desiredName, DataverseDeploy form)
        {
            //var newAzureCommsID = CommunicationServiceResource.CreateResourceIdentifier(SelectedSubscription.Id, SelectedGroup.Data.Name, communicationsNameTB.Text);

            form.OutputRT.Text += Environment.NewLine + "Waiting for Azure Communication Service Creation";

            ArmOperation<CommunicationServiceResource> comms = await form.SelectedGroup.GetCommunicationServiceResources().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new CommunicationServiceResourceData("global")
            {
                DataLocation = "unitedstates"
            });

            var keys = await comms.Value.GetKeysAsync();

            form.OutputRT.Text += Environment.NewLine + "Azure Communication Service Created.";

            return (comms.Value.Data.Id, keys.Value.PrimaryConnectionString);
        }
        static async Task<bool> CheckCommsName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetCommunicationServiceResourceAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    var nameResponse = await form.SelectedSubscription.CheckCommunicationNameAvailabilityAsync(new CommunicationServiceNameAvailabilityContent()
                    {
                        Name = desiredName,
                        ResourceType = "Microsoft.Communication/CommunicationServices"
                    });

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Azure Communications name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + ex.Message;
                    return false;
                }
            }
        }
        static async Task<(ResourceIdentifier, string)> CreateAzureCommunicationsResource(string desiredName, CosmosDeploy form)
        {
            //var newAzureCommsID = CommunicationServiceResource.CreateResourceIdentifier(SelectedSubscription.Id, SelectedGroup.Data.Name, communicationsNameTB.Text);

            form.OutputRT.Text += Environment.NewLine + "Waiting for Azure Communication Service Creation";

            ArmOperation<CommunicationServiceResource> comms = await form.SelectedGroup.GetCommunicationServiceResources().CreateOrUpdateAsync(WaitUntil.Completed, desiredName, new CommunicationServiceResourceData("global")
            {
                DataLocation = "unitedstates"
            });

            var keys = await comms.Value.GetKeysAsync();

            form.OutputRT.Text += Environment.NewLine + "Azure Communication Service Created.";

            return (comms.Value.Data.Id, keys.Value.PrimaryConnectionString);
        }
        static async Task<bool> CheckCommsName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedGroup.GetCommunicationServiceResourceAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    var nameResponse = await form.SelectedSubscription.CheckCommunicationNameAvailabilityAsync(new CommunicationServiceNameAvailabilityContent()
                    {
                        Name = desiredName,
                        ResourceType = "Microsoft.Communication/CommunicationServices"
                    });

                    if (nameResponse.Value.IsNameAvailable == true)
                    {
                        return true;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + "Azure Communications name has already been taken, try another.";
                        return false;
                    }
                }
                else
                {
                    form.OutputRT.Text += Environment.NewLine + ex.Message;
                    return false;
                }
            }
        }

        static async Task<(ResourceIdentifier, string)> SkipComms(ResourceGroupResource SelectedGroup, string desiredName)
        {
            var temp = await SelectedGroup.GetCommunicationServiceResourceAsync(desiredName);
            var keys = await temp.Value.GetKeysAsync();
            return (temp.Value.Data.Id, keys.Value.PrimaryConnectionString);
        }
        static async Task<(ResourceIdentifier, string)> SkipComms(CommunicationServiceResource temp)
        {
            var keys = await temp.GetKeysAsync();
            return (temp.Data.Id, keys.Value.PrimaryConnectionString);
        }
    }
}
