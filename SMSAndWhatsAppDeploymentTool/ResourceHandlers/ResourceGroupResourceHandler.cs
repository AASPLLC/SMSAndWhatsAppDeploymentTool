using Azure.ResourceManager.Resources;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    public class ResourceGroupResourceHandler
    {
        public static async Task<ResourceGroupResource> FullResourceGroupCheck(DataverseDeploy form)
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
        public static async Task<bool> CheckResourceGroupName(string desiredName, DataverseDeploy form)
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
        public static async Task<ResourceGroupResource> FullResourceGroupCheck(CosmosDeploy form)
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
        public static async Task<bool> CheckResourceGroupName(string desiredName, CosmosDeploy form)
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
