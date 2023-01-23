using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool
{
    public class ArmClientHandler
    {
        public ArmClient? client;

        public ArmClientHandler()
        {
            client = TokenHandler.CreateArmClient();
        }

        public (List<string>, List<SubscriptionResource>) SetupSubscriptionName()
        {
            List<string> names = new();
            List<SubscriptionResource> ids = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            SubscriptionCollection subscriptions = client.GetSubscriptions();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            List<SubscriptionResource> subs = subscriptions.GetAll().ToList();
            for (int i = 0; i < subs.Count; i++)
            {
                names.Add(subs[i].Data.DisplayName);
                ids.Add(subs[i]);
            }
            return (names, ids);
                //can create new resource group
                //var temp = await client.GetSubscriptionResource().GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, "namefornewgroup", );

            //can update resource
            // temp = client.GetSubscriptionResource().GetGenericResources().ToList();
            //temp[0].Update();


        }

        public static (List<string>, List<ResourceGroupResource>) SetupResourceName(SubscriptionResource sr)
        {
            List<string> names = new();
            List<ResourceGroupResource> ids = new();
            ResourceGroupCollection resourceGroups = sr.GetResourceGroups();
            foreach (var item in resourceGroups)
            {
                names.Add(item.Data.Name);
                ids.Add(item);
            }
            return (names, ids);
        }

        public ArmClient GetArmClient()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return client;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
