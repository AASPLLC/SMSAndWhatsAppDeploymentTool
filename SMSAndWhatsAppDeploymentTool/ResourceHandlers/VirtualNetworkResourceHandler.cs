using Azure.Core;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Network;
using Azure;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class VirtualNetworkResourceHandler
    {
        internal virtual async Task InitialCreation(string defaultSubnet, string appSubnet, StepByStepValues sbs)
        {
            if (defaultSubnet == "")
                defaultSubnet = "10.1.0.0";
            if (appSubnet == "")
                appSubnet = "10.1.0.32";

            if (await CheckVirtualNetworkName(sbs))
            {
                _ = await sbs.SelectedGroup.GetVirtualNetworks().CreateOrUpdateAsync(WaitUntil.Completed, "StorageConnection", CreateVirtualNetworkData(defaultSubnet, appSubnet, sbs.SelectedRegion, false));
            }
            else
            {
                SubnetResource subnet = await sbs.SelectedGroup.GetVirtualNetwork("StorageConnection").Value.GetSubnetAsync("RestAPIToCosmos");
                await subnet.UpdateAsync(WaitUntil.Completed, CreateSubnetData("RestAPIToCosmos", appSubnet + "/27", sbs.SelectedRegion, false));
            }
        }
        internal virtual async Task<ResourceIdentifier> InitialCreation(string defaultSubnet, string appSubnet, DataverseDeploy form)
        {
            if (defaultSubnet == "")
                defaultSubnet = "10.1.0.0";
            if (appSubnet == "")
                appSubnet = "10.1.0.32";

            if (await CheckVirtualNetworkName(form))
            {
                VirtualNetworkResource vnetResource = (await form.SelectedGroup.GetVirtualNetworks().CreateOrUpdateAsync(WaitUntil.Completed, "StorageConnection", CreateVirtualNetworkData(defaultSubnet, appSubnet, form.SelectedRegion, false, form))).Value;
                return vnetResource.Data.Subnets[1].Id;
            }
            else
            {
                SubnetResource subnet = await form.SelectedGroup.GetVirtualNetwork("StorageConnection").Value.GetSubnetAsync("RestAPIToCosmos");
                await subnet.UpdateAsync(WaitUntil.Completed, CreateSubnetData("RestAPIToCosmos", appSubnet + "/27", form.SelectedRegion, false));
                return await SkipVirtualNetwork(form);
            }
        }
        internal virtual async Task<(ResourceIdentifier, string)> InitialCreation(string defaultSubnet, string appSubnet, CosmosDeploy form)
        {
            if (defaultSubnet == "")
                defaultSubnet = "10.1.0.0";
            if (appSubnet == "")
                appSubnet = "10.1.0.32";

            if (await CheckVirtualNetworkName(form))
            {
                VirtualNetworkData virtualNetworkData = (await form.SelectedGroup.GetVirtualNetworks().CreateOrUpdateAsync(WaitUntil.Completed, "StorageConnection", CreateVirtualNetworkData(defaultSubnet, appSubnet, form.SelectedRegion, true, form))).Value.Data;
                return (virtualNetworkData.Subnets[1].Id, virtualNetworkData.Name);
            }
            else
            {
                SubnetResource subnet = await form.SelectedGroup.GetVirtualNetwork("StorageConnection").Value.GetSubnetAsync("RestAPIToCosmos");
                await subnet.UpdateAsync(WaitUntil.Completed, CreateSubnetData("RestAPIToCosmos", appSubnet + "/27", form.SelectedRegion, true));
                return await SkipVirtualNetwork(form);
            }
        }

        static async Task<bool> CheckVirtualNetworkName(StepByStepValues sbs)
        {
            try
            {
                _ = await sbs.SelectedGroup.GetVirtualNetworkAsync("StorageConnection");
                Console.Write(Environment.NewLine + "StorageConnection already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckVirtualNetworkName(DataverseDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetVirtualNetworkAsync("StorageConnection");
                form.OutputRT.Text += Environment.NewLine + "StorageConnection already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckVirtualNetworkName(CosmosDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetVirtualNetworkAsync("StorageConnection");
                form.OutputRT.Text += Environment.NewLine + "StorageConnection already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }

        static async Task<ResourceIdentifier> SkipVirtualNetwork(DataverseDeploy form)
        {
            var temp = await form.SelectedGroup.GetVirtualNetworkAsync("StorageConnection");
            return temp.Value.Data.Subnets[1].Id;
        }
        static async Task<(ResourceIdentifier, string)> SkipVirtualNetwork(CosmosDeploy form)
        {
            var temp = await form.SelectedGroup.GetVirtualNetworkAsync("StorageConnection");
            return (temp.Value.Data.Subnets[1].Id, temp.Value.Data.Name);
        }

        static VirtualNetworkData CreateVirtualNetworkData(string defaultSubnet, string appSubnet, string region, bool rest)
        {
            Console.Write(Environment.NewLine + "Waiting for Virtual Network Creation");
            VirtualNetworkData virtualNetwork = new()
            {
                EnableDdosProtection = false,
                Location = region,
            };
            virtualNetwork.Subnets.Add(CreateSubnetData("default", defaultSubnet + "/29"));
            virtualNetwork.Subnets.Add(CreateSubnetData("RestAPIToCosmos", appSubnet + "/27", region, rest));
            virtualNetwork.AddressPrefixes.Add(defaultSubnet + "/16");

            Console.Write(Environment.NewLine + "Virtual network for internal storage connection created");
            return virtualNetwork;
        }
        static VirtualNetworkData CreateVirtualNetworkData(string defaultSubnet, string appSubnet, string region, bool rest, dynamic form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Virtual Network Creation";
            VirtualNetworkData virtualNetwork = new()
            {
                EnableDdosProtection = false,
                Location = region,
            };
            virtualNetwork.Subnets.Add(CreateSubnetData("default", defaultSubnet + "/29"));
            virtualNetwork.Subnets.Add(CreateSubnetData("RestAPIToCosmos", appSubnet + "/27", region, rest));
            virtualNetwork.AddressPrefixes.Add(defaultSubnet + "/16");

            form.OutputRT.Text += Environment.NewLine + "Virtual network for internal storage connection created";
            return virtualNetwork;
        }

        static SubnetData CreateSubnetData(string name, string addressPrefix)
        {
            SubnetData subnetData = new()
            {
                Name = name,
                AddressPrefix = addressPrefix,
                PrivateEndpointNetworkPolicy = VirtualNetworkPrivateEndpointNetworkPolicy.Disabled,
                PrivateLinkServiceNetworkPolicy = VirtualNetworkPrivateLinkServiceNetworkPolicy.Enabled,
                ResourceType = "Microsoft.Network/virutalNetworks/subnets"
            };

            return subnetData;
        }
        static SubnetData CreateSubnetData(string name, string addressPrefix, string region, bool rest)
        {
            SubnetData subnetData = CreateSubnetData(name, addressPrefix);
            subnetData.ServiceEndpoints.Add(CreateServiceEndpoint("Microsoft.Storage", region));
            subnetData.ServiceEndpoints.Add(CreateServiceEndpoint("Microsoft.KeyVault"));
            if (rest)
                subnetData.ServiceEndpoints.Add(CreateServiceEndpoint("Microsoft.AzureCosmosDB"));
            ServiceDelegation serviceDelegation = new()
            {
                Name = "delegation",
                ServiceName = "Microsoft.Web/serverfarms"
            };
            subnetData.Delegations.Add(serviceDelegation);
            return subnetData;
        }

        static ServiceEndpointProperties CreateServiceEndpoint(string ServiceEndpointName)
        {
            ServiceEndpointProperties storageProperties = new() { Service = ServiceEndpointName };
            return storageProperties;
        }
        static ServiceEndpointProperties CreateServiceEndpoint(string ServiceEndpointName, string region)
        {
            ServiceEndpointProperties storageProperties = new();
            storageProperties.Locations.Add(region);
            storageProperties.Service = ServiceEndpointName;
            return storageProperties;
        }
    }
}
