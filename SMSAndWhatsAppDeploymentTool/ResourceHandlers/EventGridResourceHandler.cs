using Azure;
using Azure.Core;
using Azure.ResourceManager.Communication;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.EventGrid.Models;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Storage;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool.ResourceHandlers
{
    internal class EventGridResourceHandler
    {
        internal virtual async Task InitialCreation(string desiredStorageName, StepByStepValues sbs)
        {
            if (await CheckEventGridTopicName(sbs))
                await CreateEventGridTopic(desiredStorageName, sbs);
        }
        internal virtual async Task InitialCreation(string desiredCommunicationsName, ResourceIdentifier smsIdentityId, ResourceIdentifier storageIdentityId, DataverseDeploy form)
        {
            if (await CheckEventGridTopicName(desiredCommunicationsName, form))
                await CreateEventGridTopic(desiredCommunicationsName, smsIdentityId, storageIdentityId, form);
        }
        internal virtual async Task InitialCreation(string desiredCommunicationsName, ResourceIdentifier smsIdentityId, ResourceIdentifier storageIdentityId, CosmosDeploy form)
        {
            if (await CheckEventGridTopicName(desiredCommunicationsName, form))
                await CreateEventGridTopic(desiredCommunicationsName, smsIdentityId, storageIdentityId, form);
        }

        static async Task CreateEventGridTopic(string desiredStorageName, StepByStepValues sbs)
        {
            Console.Write(Environment.NewLine + "Waiting for Event Grid Creation");
            var systemGridResponse = await sbs.SelectedGroup.GetSystemTopics().CreateOrUpdateAsync(WaitUntil.Completed, sbs.DesiredCommsName + "Event-topic123", new SystemTopicData("global")
            {
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                Source = (await sbs.SelectedGroup.GetCommunicationServiceResourceAsync(sbs.DesiredCommsName)).Value.Data.Id,
                TopicType = "Microsoft.Communication.CommunicationServices"
            });

            int counter = 0;
            var finishedSystemGridResponse = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                Console.Write(", " + counter);
                return systemGridResponse.WaitForCompletionResponse();
            });

            EventSubscriptionFilter eventSubscriptionFilter = new()
            {
                IsAdvancedFilteringOnArraysEnabled = true
            };
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSReceived");
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSDeliveryReportReceived");

            var eventSubscriptionResponse = await systemGridResponse.Value.GetSystemTopicEventSubscriptions().CreateOrUpdateAsync(WaitUntil.Completed, sbs.DesiredCommsName + "Event", new EventGridSubscriptionData()
            {
                //no deadletter or identity at this time.
                Filter = eventSubscriptionFilter,
                Destination = new StorageQueueEventSubscriptionDestination()
                {
                    QueueName = "smsq",
                    ResourceId = (await sbs.SelectedGroup.GetStorageAccountAsync(desiredStorageName)).Value.Id,
                    QueueMessageTimeToLiveInSeconds = -1
                },
                EventDeliverySchema = new EventDeliverySchema("EventGridSchema"),
                RetryPolicy = new EventSubscriptionRetryPolicy()
                {
                    EventTimeToLiveInMinutes = 1440,
                    MaxDeliveryAttempts = 30
                }
            });

            Console.Write(Environment.NewLine + "Event grid created for SMS link from Azure Communications to Storage Queue");
        }
        static async Task CreateEventGridTopic(string desiredName, ResourceIdentifier smsSourceId, ResourceIdentifier storageResourceId, DataverseDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Event Grid Creation";
            var systemGridResponse = await form.SelectedGroup.GetSystemTopics().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "Event-topic123", new SystemTopicData("global")
            {
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                Source = smsSourceId,
                TopicType = "Microsoft.Communication.CommunicationServices"
            });

            int counter = 0;
            var finishedSystemGridResponse = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                try { form.OutputRT.Text += ", " + counter; } catch { form.OutputRT.Invoke(() => { form.OutputRT.Text += ", " + counter; }); }
                return systemGridResponse.WaitForCompletionResponse();
            });

            EventSubscriptionFilter eventSubscriptionFilter = new()
            {
                IsAdvancedFilteringOnArraysEnabled = true
            };
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSReceived");
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSDeliveryReportReceived");

            var eventSubscriptionResponse = await systemGridResponse.Value.GetSystemTopicEventSubscriptions().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "Event", new EventGridSubscriptionData()
            {
                //no deadletter or identity at this time.
                Filter = eventSubscriptionFilter,
                Destination = new StorageQueueEventSubscriptionDestination()
                {
                    QueueName = "smsq",
                    ResourceId = storageResourceId,
                    QueueMessageTimeToLiveInSeconds = -1
                },
                EventDeliverySchema = new EventDeliverySchema("EventGridSchema"),
                RetryPolicy = new EventSubscriptionRetryPolicy()
                {
                    EventTimeToLiveInMinutes = 1440,
                    MaxDeliveryAttempts = 30
                }
            });

            form.OutputRT.Text += Environment.NewLine + "Event grid created for SMS link from Azure Communications to Storage Queue";
        }
        static async Task CreateEventGridTopic(string desiredName, ResourceIdentifier smsSourceId, ResourceIdentifier storageResourceId, CosmosDeploy form)
        {
            form.OutputRT.Text += Environment.NewLine + "Waiting for Event Grid Creation";
            var systemGridResponse = await form.SelectedGroup.GetSystemTopics().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "Event-topic123", new SystemTopicData("global")
            {
                Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned),
                Source = smsSourceId,
                TopicType = "Microsoft.Communication.CommunicationServices"
            });


            int counter = 0;
            var finishedSystemGridResponse = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                counter++;
                try { form.OutputRT.Text += ", " + counter; } catch { form.OutputRT.Invoke(() => { form.OutputRT.Text += ", " + counter; }); }
                return systemGridResponse.WaitForCompletionResponse();
            });

            EventSubscriptionFilter eventSubscriptionFilter = new()
            {
                IsAdvancedFilteringOnArraysEnabled = true
            };
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSReceived");
            eventSubscriptionFilter.IncludedEventTypes.Add("Microsoft.Communication.SMSDeliveryReportReceived");

            var eventSubscriptionResponse = await systemGridResponse.Value.GetSystemTopicEventSubscriptions().CreateOrUpdateAsync(WaitUntil.Completed, desiredName + "Event", new EventGridSubscriptionData()
            {
                //no deadletter or identity at this time.
                Filter = eventSubscriptionFilter,
                Destination = new StorageQueueEventSubscriptionDestination()
                {
                    QueueName = "smsq",
                    ResourceId = storageResourceId,
                    QueueMessageTimeToLiveInSeconds = -1
                },
                EventDeliverySchema = new EventDeliverySchema("EventGridSchema"),
                RetryPolicy = new EventSubscriptionRetryPolicy()
                {
                    EventTimeToLiveInMinutes = 1440,
                    MaxDeliveryAttempts = 30
                }
            });

            form.OutputRT.Text += Environment.NewLine + "Event grid created for SMS link from Azure Communications to Storage Queue";
        }

        static async Task<bool> CheckEventGridTopicName(StepByStepValues sbs)
        {
            try
            {
                _ = await sbs.SelectedGroup.GetSystemTopicAsync(sbs.DesiredCommsName + "Event-topic123");
                Console.Write(Environment.NewLine + "Event Grid Topic already exists in your environment, skipping.");
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckEventGridTopicName(string desiredName, DataverseDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetSystemTopicAsync(desiredName + "Event-topic123");
                form.OutputRT.Text += Environment.NewLine + "Event Grid Topic already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
        static async Task<bool> CheckEventGridTopicName(string desiredName, CosmosDeploy form)
        {
            try
            {
                _ = await form.SelectedGroup.GetSystemTopicAsync(desiredName + "Event-topic123");
                form.OutputRT.Text += Environment.NewLine + "Event Grid Topic already exists in your environment, skipping.";
                return false;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return true;
            }
        }
    }
}
