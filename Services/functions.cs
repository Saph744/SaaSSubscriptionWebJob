using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SaaSFulfillmentApp.Services;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services
{
    public class Functions
    {
        private readonly SubscriptionSyncService _subscriptionSyncService;

        public Functions(SubscriptionSyncService subscriptionSyncService)
        {
            _subscriptionSyncService = subscriptionSyncService;
        }

        public async Task ProcessQueueMessage([QueueTrigger("myqueue-items")] string message, ILogger logger)
        {
            logger.LogInformation($"Processing queue message: {message}");
        }

        public async Task SyncSubscriptions([TimerTrigger("0 11 11 * * *")] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation($"SyncSubscriptions triggered at: {DateTime.Now}");
            await _subscriptionSyncService.SyncSubscriptionsAsync();
        }

    }
}
