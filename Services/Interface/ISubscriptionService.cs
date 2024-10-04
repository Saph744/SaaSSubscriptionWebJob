using SaaSFulfillmentApp.Data;
using SaaSFulfillmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Services.Interface
{
    public interface ISubscriptionService
    {
        Task<List<SaaSSubscription>> GetAllSubscriptionsAsync();
    }
}
