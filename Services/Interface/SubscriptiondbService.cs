using SaaSFulfillmentApp.Data;
using SaaSFulfillmentApp.Models;
using SaaSFulfillmentApp.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services.Interface
{
    public class SubscriptiondbService : ISubscriptiondbService
    {
        private readonly ISubscriptiondbRepository _subscriptiondbRepository;

        public SubscriptiondbService(ISubscriptiondbRepository subscriptiondbRepository)
        {
            _subscriptiondbRepository = subscriptiondbRepository;
        }

        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _subscriptiondbRepository.GetAllSubscriptionsAsync();
        }

        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            await _subscriptiondbRepository.AddSubscriptionAsync(subscription);
        }
        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            await _subscriptiondbRepository.UpdateSubscriptionAsync(subscription);
        }
        public async Task<Subscription> GetSubscriptionByIdAsync(string id)
        {
            return await _subscriptiondbRepository.GetSubscriptionByIdAsync(id);
        }
        public async Task AddSaaSHistoryAsync(SaaSHistory saaSHistory)
        {
            await _subscriptiondbRepository.AddSaaSHistoryAsync(saaSHistory);
        }
    }
}
