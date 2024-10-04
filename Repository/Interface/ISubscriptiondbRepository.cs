using SaaSFulfillmentApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Repository.Interface
{
    public interface ISubscriptiondbRepository
    {
        Task<List<Subscription>> GetAllSubscriptionsAsync();
        Task AddSubscriptionAsync(Subscription subscription);
        Task UpdateSubscriptionAsync(Subscription subscription);
        Task<Subscription> GetSubscriptionByIdAsync(string id);
        Task AddSaaSHistoryAsync(SaaSHistory saaSHistory);
    }
}
