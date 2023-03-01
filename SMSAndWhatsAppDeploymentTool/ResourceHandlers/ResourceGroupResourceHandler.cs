using Azure.ResourceManager.Resources;
using Azure;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class ResourceGroupResourceHandler
    {
        static internal async Task<ResourceGroupResource> FullResourceGroupCheck(DataverseDeploy form)
        {
            string groupname = "SMSAndWhatsAppResourceGroup";
            if (await CheckResourceGroupName(groupname, form))
            {
                var createResourceGroupResponse = await form.SelectedSubscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, groupname, new ResourceGroupData(form.SelectedRegion));
                form.OutputRT.Text = "Initital Resource Group \"" + groupname + "\" created.";
                form.SelectedGroup = createResourceGroupResponse.Value;
                return form.SelectedGroup;
            }
            else
            {
                form.SelectedGroup = (await form.SelectedSubscription.GetResourceGroupAsync(groupname));
                return form.SelectedGroup;
            }
        }
        static async Task<bool> CheckResourceGroupName(string desiredName, DataverseDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedSubscription.GetResourceGroupAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static internal async Task<ResourceGroupResource> FullResourceGroupCheck(CosmosDeploy form)
        {
            string groupname = "SMSAndWhatsAppResourceGroup";
            if (await CheckResourceGroupName(groupname, form))
            {
                var createResourceGroupResponse = await form.SelectedSubscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, groupname, new ResourceGroupData(form.SelectedRegion));
                form.OutputRT.Text = "Initital Resource Group \"" + groupname + "\" created.";
                form.SelectedGroup = createResourceGroupResponse.Value;
                return form.SelectedGroup;
            }
            else
            {
                form.SelectedGroup = (await form.SelectedSubscription.GetResourceGroupAsync(groupname));
                return form.SelectedGroup;
            }
        }
        static async Task<bool> CheckResourceGroupName(string desiredName, CosmosDeploy form)
        {
            desiredName = desiredName.Trim();
            try
            {
                _ = await form.SelectedSubscription.GetResourceGroupAsync(desiredName);
                form.OutputRT.Text += Environment.NewLine + desiredName + " already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }

        static internal async Task<ResourceGroupResource> FullResourceGroupCheck(StepByStepValues sbs)
        {
            string groupname = "SMSAndWhatsAppResourceGroup";
            if (await CheckResourceGroupName(groupname, sbs))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var createResourceGroupResponse = await sbs.SelectedSubscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, groupname, new ResourceGroupData(sbs.SelectedRegion));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Console.WriteLine("Initital Resource Group \"" + groupname + "\" created.");

                sbs.SelectedGroup = createResourceGroupResponse.Value;
                return sbs.SelectedGroup;
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                sbs.SelectedGroup = await sbs.SelectedSubscription.GetResourceGroupAsync(groupname);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return sbs.SelectedGroup;
            }
        }
        static async Task<bool> CheckResourceGroupName(string desiredName, StepByStepValues sbs)
        {
            desiredName = desiredName.Trim();
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _ = await sbs.SelectedSubscription.GetResourceGroupAsync(desiredName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Console.Write(Environment.NewLine + desiredName + " already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
    }
}
