using Azure;
using Azure.Core;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.AppService.Models;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class AppServicePlanResourceHandler
    {
        internal virtual async Task<ResourceIdentifier> InitialCreation(DataverseDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetAppServicePlans())
            {
                return item.Id;
            }
            if (await CheckMinimumAppPlanName(form))
            {
                return await CreateMinimumAppPlan(form);
            }
            else
            {
                return await SkipMinimumAppPlan(form);
            }
        }
        internal virtual async Task<ResourceIdentifier> InitialCreation(CosmosDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetAppServicePlans())
            {
                return item.Id;
            }
            if (await CheckMinimumAppPlanName(form))
            {
                return await CreateMinimumAppPlan(form);
            }
            else
            {
                return await SkipMinimumAppPlan(form);
            }
        }

        internal virtual async Task<ResourceIdentifier> InitialCreationFree(DataverseDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetAppServicePlans())
            {
                return item.Id;
            }
            if (await CheckFreeAppPlanName(form))
            {
                return await CreateFreeAppPlan(form);
            }
            else
            {
                return await SkipFreeAppPlan(form);
            }
        }
        internal virtual async Task<ResourceIdentifier> InitialCreationFree(CosmosDeploy form)
        {
            foreach (var item in form.SelectedGroup.GetAppServicePlans())
            {
                return item.Id;
            }
            if (await CheckFreeAppPlanName(form))
            {
                return await CreateFreeAppPlan(form);
            }
            else
            {
                return await SkipFreeAppPlan(form);
            }
        }

        static async Task<ResourceIdentifier> CreateFreeAppPlan(DataverseDeploy form)
        {
            var appPlanResonse = await form.SelectedGroup.GetAppServicePlans().CreateOrUpdateAsync(WaitUntil.Completed, "FreePlan", new AppServicePlanData(form.SelectedRegion)
            {
                Sku = new AppServiceSkuDescription()
                {
                    Name = "F1",
                    Tier = "Free",
                    Size = "F1",
                    Family = "F",
                    Capacity = 0
                },
                Kind = "app",
                IsPerSiteScaling = false,
                IsElasticScaleEnabled = false,
                MaximumElasticWorkerCount = 0,
                IsSpot = false,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                TargetWorkerCount = 0,
                TargetWorkerSizeId = 0,
                IsZoneRedundant = false
            });

            form.OutputRT.Text += Environment.NewLine + "Free App plan for WhatsApp Function Apps created";

            return appPlanResonse.Value.Id;
        }
        static async Task<ResourceIdentifier> CreateFreeAppPlan(CosmosDeploy form)
        {
            var appPlanResonse = await form.SelectedGroup.GetAppServicePlans().CreateOrUpdateAsync(WaitUntil.Completed, "FreePlan", new AppServicePlanData(form.SelectedRegion)
            {
                Sku = new AppServiceSkuDescription()
                {
                    Name = "F1",
                    Tier = "Free",
                    Size = "F1",
                    Family = "F",
                    Capacity = 0
                },
                Kind = "app",
                IsPerSiteScaling = false,
                IsElasticScaleEnabled = false,
                MaximumElasticWorkerCount = 0,
                IsSpot = false,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                TargetWorkerCount = 0,
                TargetWorkerSizeId = 0,
                IsZoneRedundant = false
            });

            form.OutputRT.Text += Environment.NewLine + "Free App plan for WhatsApp Function Apps created";

            return appPlanResonse.Value.Id;
        }

        static async Task<ResourceIdentifier> CreateMinimumAppPlan(DataverseDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for App Plan Creation";
            var appPlanResonse = await form.SelectedGroup.GetAppServicePlans().CreateOrUpdateAsync(WaitUntil.Completed, "MinimumPlanForFunctionAppWithSubnet", new AppServicePlanData(form.SelectedRegion)
            {
                Sku = new AppServiceSkuDescription()
                {
                    Name = "S1",
                    Tier = "Standard",
                    Size = "S1",
                    Family = "S",
                    Capacity = 1
                },
                Kind = "app",
                IsPerSiteScaling = false,
                IsElasticScaleEnabled = false,
                MaximumElasticWorkerCount = 0,
                IsSpot = false,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                TargetWorkerCount = 0,
                TargetWorkerSizeId = 0,
                IsZoneRedundant = false
            });

            form.OutputRT.Text += Environment.NewLine + "Minimum App plan for SMS Function App created";

            return appPlanResonse.Value.Id;
        }
        static async Task<ResourceIdentifier> CreateMinimumAppPlan(CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for App Plan Creation";
            var appPlanResonse = await form.SelectedGroup.GetAppServicePlans().CreateOrUpdateAsync(WaitUntil.Completed, "MinimumPlanForFunctionAppWithSubnet", new AppServicePlanData(form.SelectedRegion)
            {
                Sku = new AppServiceSkuDescription()
                {
                    Name = "S1",
                    Tier = "Standard",
                    Size = "S1",
                    Family = "S",
                    Capacity = 1
                },
                Kind = "app",
                IsPerSiteScaling = false,
                IsElasticScaleEnabled = false,
                MaximumElasticWorkerCount = 0,
                IsSpot = false,
                IsReserved = false,
                IsXenon = false,
                IsHyperV = false,
                TargetWorkerCount = 0,
                TargetWorkerSizeId = 0,
                IsZoneRedundant = false
            });

            form.OutputRT.Text += Environment.NewLine + "Minimum App plan for SMS Function App created";

            return appPlanResonse.Value.Id;
        }

        static async Task<ResourceIdentifier> SkipFreeAppPlan(DataverseDeploy form)
        {
            var temp = (await form.SelectedGroup.GetAppServicePlanAsync("FreePlan")).Value;

            return temp.Id;
        }
        static async Task<ResourceIdentifier> SkipFreeAppPlan(CosmosDeploy form)
        {
            var temp = (await form.SelectedGroup.GetAppServicePlanAsync("FreePlan")).Value;

            return temp.Id;
        }

        static async Task<ResourceIdentifier> SkipMinimumAppPlan(DataverseDeploy form)
        {
            var temp = (await form.SelectedGroup.GetAppServicePlanAsync("MinimumPlanForFunctionAppWithSubnet")).Value;

            return temp.Id;
        }
        static async Task<ResourceIdentifier> SkipMinimumAppPlan(CosmosDeploy form)
        {
            var temp = (await form.SelectedGroup.GetAppServicePlanAsync("MinimumPlanForFunctionAppWithSubnet")).Value;

            return temp.Id;
        }

        static async Task<bool> CheckFreeAppPlanName(DataverseDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetAppServicePlanAsync("FreePlan");
                form.OutputRT.Text += Environment.NewLine + "Free App Plan already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckFreeAppPlanName(CosmosDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetAppServicePlanAsync("FreePlan");
                form.OutputRT.Text += Environment.NewLine + "Free App Plan already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }

        static async Task<bool> CheckMinimumAppPlanName(DataverseDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetAppServicePlanAsync("MinimumPlanForFunctionAppWithSubnet");
                form.OutputRT.Text += Environment.NewLine + "Minimum App Plan already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckMinimumAppPlanName(CosmosDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetAppServicePlanAsync("MinimumPlanForFunctionAppWithSubnet");
                form.OutputRT.Text += Environment.NewLine + "Minimum App Plan already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
    }
}
