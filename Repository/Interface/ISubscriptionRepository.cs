using SaaSFulfillmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Repository.Interface
{
    public interface ISubscriptionRepository
    {
        Task<List<SaaSSubscription>> GetAllSubscriptionsAsync();
    }
}
