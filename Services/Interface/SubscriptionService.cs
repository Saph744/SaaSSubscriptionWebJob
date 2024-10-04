using SaaSFulfillmentApp.Models;
using SaaSFulfillmentApp.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services.Interface
{
    public class SubscriptionService : ISubscriptionService 
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<SaaSSubscription>> GetAllSubscriptionsAsync()
        {
            return await _subscriptionRepository.GetAllSubscriptionsAsync();
        }
    }
}
