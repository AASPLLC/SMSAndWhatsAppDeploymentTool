using Azure.Core;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Network;
using Azure;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class VirtualNetworkResourceHandler
    {
        internal virtual async Task<ResourceIdentifier> InitialCreation(DataverseDeploy form)
        {
            if (await CheckVirtualNetworkName(form))
            {
                VirtualNetworkResource vnetResource = (await form.SelectedGroup.GetVirtualNetworks().CreateOrUpdateAsync(WaitUntil.Completed, "StorageConnection", CreateVirtualNetworkData(form.SelectedRegion, false, form))).Value;
                return vnetResource.Data.Subnets[1].Id;
            }
            else
            {
                return await SkipVirtualNetwork(form);
            }
        }
        internal virtual async Task<(ResourceIdentifier, string)> InitialCreation(CosmosDeploy form)
        {
            if (await CheckVirtualNetworkName(form))
            {
                VirtualNetworkData virtualNetworkData = (await form.SelectedGroup.GetVirtualNetworks().CreateOrUpdateAsync(WaitUntil.Completed, "StorageConnection", CreateVirtualNetworkData(form.SelectedRegion, true, form))).Value.Data;
                return (virtualNetworkData.Subnets[1].Id, virtualNetworkData.Name);
            }
            else
            {
                return await SkipVirtualNetwork(form);
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

        static VirtualNetworkData CreateVirtualNetworkData(string region, bool rest, dynamic form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Virtual Network Creation";
            VirtualNetworkData virtualNetwork = new()
            {
                EnableDdosProtection = false,
                Location = region,
            };
            virtualNetwork.Subnets.Add(CreateSubnetData("default", "10.1.0.0/29"));
            virtualNetwork.Subnets.Add(CreateSubnetData("RestAPIToCosmos", "10.1.0.32/27", region, rest));
            virtualNetwork.AddressPrefixes.Add("10.1.0.0/16");

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
