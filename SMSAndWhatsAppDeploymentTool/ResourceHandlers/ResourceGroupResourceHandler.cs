using Azure.ResourceManager.Resources;
using Azure;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class ResourceGroupResourceHandler
    {
        internal async Task<ResourceGroupResource> FullResourceGroupCheck(DataverseDeploy form)
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
        internal async Task<bool> CheckResourceGroupName(string desiredName, DataverseDeploy form)
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
        internal async Task<ResourceGroupResource> FullResourceGroupCheck(CosmosDeploy form)
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
        internal async Task<bool> CheckResourceGroupName(string desiredName, CosmosDeploy form)
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
    }
}
