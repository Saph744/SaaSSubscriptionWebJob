using Newtonsoft.Json;

namespace SaaSFulfillmentApp.Models
{
    public class SaaSSubscription
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        [JsonProperty("planId")]
        public string PlanId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("beneficiary")]
        public SaaSUser Beneficiary { get; set; }

        [JsonProperty("purchaser")]
        public SaaSUser Purchaser { get; set; }

        [JsonProperty("term")]
        public SaasSubscriptionTerm Term { get; set; }

        [JsonProperty("saasSubscriptionStatus")]
        public string SaasSubscriptionStatus { get; set; }

        [JsonProperty("isTest")]
        public bool IsTest { get; set; }

        [JsonProperty("isFreeTrial")]
        public bool IsFreeTrial { get; set; }

        [JsonProperty("autoRenew")]
        public bool AutoRenew { get; set; }

        [JsonProperty("sandboxType")]
        public string SandBoxType { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("sessionMode")]
        public string SessionMode { get; set; }

        [JsonProperty("allowedCustomerOperations")]
        public List<string> AllowedCustomerOperations { get; set; }
    }

    public class SaaSUser
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("puid")]
        public string Puid { get; set; }
    }

    public class SaasSubscriptionTerm
    {
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("termUnit")]
        public string TermUnit { get; set; }
    }
}
