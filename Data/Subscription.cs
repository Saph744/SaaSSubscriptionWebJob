using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Data
{
    public class Subscription
    {
        public int ID { get; set; }
        public string SubsID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SubscriptionName { get; set; }
        public string PublisherID { get; set; }
        public string OfferID { get; set; }
        public string PlanID { get; set; }
        public string Quantity { get; set; }
        public string BeneficiaryEmail { get; set; }
        public string BeneficiaryObjectID { get; set; }
        public string BeneficiaryTenantID { get; set; }
        public string BeneficiaryPuID { get; set; }
        public string PurchaserEmail { get; set; }
        public string PurchaserObjectID { get; set; }
        public string PurchaserTenantID { get; set; }
        public string PurchaserPuID { get; set; }

        [JsonProperty("allowedCustomerOperations")]
        public List<string> AllowedCustomerOperations { get; set; }
        public string SessionMode { get; set; }
        public bool IsFreeTrial { get; set; }
        public bool AutoRenew { get; set; }
        public string SandBoxType { get; set; }
        public string SubscriptionStatus { get; set; }
        public string TermUnit { get; set; }
        public bool IsTest { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

    }
}
