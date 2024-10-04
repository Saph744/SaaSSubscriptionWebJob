using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaaSFulfillmentApp.Data;
using SaaSFulfillmentApp.Repository.Interface;
using SaaSFulfillmentApp.Services.Interface;

namespace SaaSFulfillmentApp.Services
{
    public class SubscriptionSyncService
    {
        private readonly ISubscriptiondbService _subscriptiondbService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmailService _emailService;

        public SubscriptionSyncService(ISubscriptiondbService subscriptiondbService, ISubscriptionService subscriptionService, IEmailService emailService)
        {
            _subscriptiondbService = subscriptiondbService;
            _subscriptionService = subscriptionService;
            _emailService = emailService;
        }

        public async Task SyncSubscriptionsAsync()
        {
            var apiSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            Console.WriteLine("All Subscriptions from SaaS API:");
            if (apiSubscriptions != null)
            {
                foreach (var subscription in apiSubscriptions)
                {
                    Console.WriteLine($"ID: {subscription.Id}, Publisher ID: {subscription.PublisherId}");
                }
            }
            var dbSubscriptions = new List<Subscription>();
            try
            {
                 dbSubscriptions = await _subscriptiondbService.GetAllSubscriptionsAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            Console.WriteLine("All Subscriptions from DB:");

            var pendingFulfillmentList = new List<Subscription>();
            var changedSubscriptionsList = new List<Subscription>();

            foreach (var subscription in dbSubscriptions)
            {
                Console.WriteLine($"ID: {subscription.ID}, SubsID: {subscription.SubsID}, Status: {subscription.SubscriptionStatus}");
            }

            if (apiSubscriptions != null && apiSubscriptions.Count > 0)
            {
                foreach (var apiSubscription in apiSubscriptions)
                {
                    var existingSubscription = dbSubscriptions.FirstOrDefault(dbSub => dbSub.SubsID == apiSubscription.Id);

                    if (existingSubscription == null)
                    {
                        var newSubscription = new Subscription
                        {
                            SubsID = apiSubscription.Id,
                            StartDate = apiSubscription.Term.StartDate,
                            EndDate = apiSubscription.Term.EndDate,
                            SubscriptionName = apiSubscription.Name,
                            PublisherID = apiSubscription.PublisherId,
                            OfferID = apiSubscription.OfferId,
                            PlanID = apiSubscription.PlanId,
                            Quantity = apiSubscription.Quantity,
                            BeneficiaryEmail = $"{apiSubscription.Beneficiary.EmailId}",
                            BeneficiaryObjectID = $"{apiSubscription.Beneficiary.ObjectId}",
                            BeneficiaryTenantID = $"{apiSubscription.Beneficiary.TenantId}",
                            BeneficiaryPuID = $"{apiSubscription.Beneficiary.Puid}",
                            PurchaserEmail = $"{apiSubscription.Purchaser.EmailId}",
                            PurchaserObjectID = $"{apiSubscription.Purchaser.ObjectId}",
                            PurchaserTenantID = $"{apiSubscription.Purchaser.TenantId}",
                            PurchaserPuID = $"{apiSubscription.Purchaser.Puid}",
                            AllowedCustomerOperations = apiSubscription.AllowedCustomerOperations,
                            SessionMode = apiSubscription.SessionMode,
                            IsTest = apiSubscription.IsTest,
                            IsFreeTrial = apiSubscription.IsFreeTrial,
                            AutoRenew = apiSubscription.AutoRenew,
                            SandBoxType = apiSubscription.SandBoxType,
                            Created = apiSubscription.Created,
                            LastModified = apiSubscription.LastModified,
                            SubscriptionStatus = apiSubscription.SaasSubscriptionStatus,
                            TermUnit = apiSubscription.Term.TermUnit
                        };

                        await _subscriptiondbService.AddSubscriptionAsync(newSubscription);
                        if (newSubscription.SubscriptionStatus.ToLower() == "pendingfulfillmentstart")
                        {
                            pendingFulfillmentList.Add(newSubscription);
                        }
                        Console.WriteLine($"Added Subscription ID: {newSubscription.ID} to DB");
                    }
                    else
                    {
                        var sqlDateTimeMin = new DateTime(1753, 1, 1);
                        var sqlDateTimeMax = new DateTime(9999, 12, 31);
                        DateTime AdjustDateTime(DateTime dt)
                        {
                            if (dt < sqlDateTimeMin)
                                return sqlDateTimeMin;
                            if (dt > sqlDateTimeMax)
                                return sqlDateTimeMax;
                            return dt;
                        }
                        var startDate = AdjustDateTime(apiSubscription.Term.StartDate);
                        var endDate = AdjustDateTime(apiSubscription.Term.EndDate);
                        var created = AdjustDateTime(apiSubscription.Created);
                        var lastModified = AdjustDateTime(apiSubscription.LastModified);
                        if (existingSubscription.SubsID == apiSubscription.Id
                            && (existingSubscription.StartDate != startDate
                            || existingSubscription.EndDate != endDate
                            || existingSubscription.SubscriptionName != apiSubscription.Name
                            || existingSubscription.PublisherID != apiSubscription.PublisherId
                            || existingSubscription.OfferID != apiSubscription.OfferId
                            || existingSubscription.PlanID != apiSubscription.PlanId
                            || existingSubscription.Quantity != apiSubscription.Quantity
                            || existingSubscription.BeneficiaryEmail != $"{apiSubscription.Beneficiary.EmailId}"
                            || existingSubscription.BeneficiaryObjectID != $"{apiSubscription.Beneficiary.ObjectId}"
                            || existingSubscription.BeneficiaryTenantID != $"{apiSubscription.Beneficiary.TenantId}"
                            || existingSubscription.BeneficiaryPuID != $"{apiSubscription.Beneficiary.Puid}"
                            || existingSubscription.PurchaserEmail != $"{apiSubscription.Purchaser.EmailId}"
                            || existingSubscription.PurchaserObjectID != $"{apiSubscription.Purchaser.ObjectId}"
                            || existingSubscription.PurchaserTenantID != $"{apiSubscription.Purchaser.TenantId}"
                            || existingSubscription.PurchaserPuID != $"{apiSubscription.Purchaser.Puid}"
                            || existingSubscription.AllowedCustomerOperations == apiSubscription.AllowedCustomerOperations
                            || existingSubscription.SessionMode != apiSubscription.SessionMode
                            || existingSubscription.IsTest != apiSubscription.IsTest
                            || existingSubscription.IsFreeTrial != apiSubscription.IsFreeTrial
                            || existingSubscription.AutoRenew != apiSubscription.AutoRenew
                            || existingSubscription.SandBoxType != apiSubscription.SandBoxType
                            || existingSubscription.Created == created
                            || existingSubscription.LastModified != lastModified
                            || existingSubscription.SubscriptionStatus != apiSubscription.SaasSubscriptionStatus
                            || existingSubscription.TermUnit != apiSubscription.Term.TermUnit
                           ))
                        {
                            SaaSHistory history = new SaaSHistory();
                            history.SubsID = existingSubscription.SubsID;
                            history.StartDate = existingSubscription.StartDate;
                            history.EndDate = existingSubscription.EndDate;
                            history.SubscriptionName = existingSubscription.SubscriptionName;
                            history.PublisherID = existingSubscription.PublisherID;
                            history.OfferID = existingSubscription.OfferID;
                            history.PlanID = existingSubscription.PlanID;
                            history.Quantity = existingSubscription.Quantity;
                            history.BeneficiaryEmail = existingSubscription.BeneficiaryEmail;
                            history.BeneficiaryObjectID = existingSubscription.BeneficiaryObjectID;
                            history.BeneficiaryPuID = existingSubscription.BeneficiaryPuID;
                            history.BeneficiaryTenantID = existingSubscription.BeneficiaryTenantID;
                            history.PurchaserEmail = existingSubscription.PurchaserEmail;
                            history.PurchaserObjectID = existingSubscription.PurchaserObjectID;
                            history.PurchaserPuID = existingSubscription.PurchaserPuID;
                            history.PurchaserTenantID = existingSubscription.PurchaserTenantID;
                            history.AllowedCustomerOperations = existingSubscription.AllowedCustomerOperations;
                            history.SessionMode = existingSubscription.SessionMode;
                            history.IsFreeTrial = existingSubscription.IsFreeTrial;
                            history.AutoRenew = existingSubscription.AutoRenew;
                            history.SandBoxType = existingSubscription.SandBoxType;
                            history.SubscriptionStatus = existingSubscription.SubscriptionStatus;
                            history.TermUnit = existingSubscription.TermUnit;
                            history.Created = existingSubscription.Created;
                            history.LastModified = existingSubscription.LastModified;
                            history.IsTest = existingSubscription.IsTest;
                            history.ID = existingSubscription.ID;
                            history.SubscriptionID = existingSubscription.ID;
                            await _subscriptiondbService.AddSaaSHistoryAsync(history);
                            Console.WriteLine($"added Subscription ID: {existingSubscription.ID} in DB");

                            existingSubscription.SubsID = apiSubscription.Id;
                            existingSubscription.StartDate = apiSubscription.Term.StartDate;
                            existingSubscription.EndDate = apiSubscription.Term.EndDate;
                            existingSubscription.SubscriptionName = apiSubscription.Name;
                            existingSubscription.PublisherID = apiSubscription.PublisherId;
                            existingSubscription.OfferID = apiSubscription.OfferId;
                            existingSubscription.PlanID = apiSubscription.PlanId;
                            existingSubscription.Quantity = apiSubscription.Quantity;
                            existingSubscription.BeneficiaryEmail = $"{apiSubscription.Beneficiary.EmailId}";
                            existingSubscription.BeneficiaryObjectID = $"{apiSubscription.Beneficiary.ObjectId}";
                            existingSubscription.BeneficiaryPuID = $"{apiSubscription.Beneficiary.Puid}";
                            existingSubscription.BeneficiaryTenantID = $"{apiSubscription.Beneficiary.TenantId}";
                            existingSubscription.PurchaserEmail = $"{apiSubscription.Purchaser.EmailId}";
                            existingSubscription.PurchaserObjectID = $"{apiSubscription.Purchaser.ObjectId}";
                            existingSubscription.PurchaserPuID = $"{apiSubscription.Purchaser.Puid}";
                            existingSubscription.PurchaserTenantID = $"{apiSubscription.Purchaser.TenantId}";
                            existingSubscription.AllowedCustomerOperations = apiSubscription.AllowedCustomerOperations;
                            existingSubscription.SessionMode = apiSubscription.SessionMode;
                            existingSubscription.IsFreeTrial = apiSubscription.IsFreeTrial;
                            existingSubscription.AutoRenew = apiSubscription.AutoRenew;
                            existingSubscription.SandBoxType = apiSubscription.SandBoxType;
                            existingSubscription.SubscriptionStatus = apiSubscription.SaasSubscriptionStatus;
                            existingSubscription.TermUnit = apiSubscription.Term.TermUnit;
                            existingSubscription.IsTest = apiSubscription.IsTest;
                            existingSubscription.Created = apiSubscription.Created;
                            existingSubscription.LastModified = apiSubscription.LastModified;
                            await _subscriptiondbService.UpdateSubscriptionAsync(existingSubscription);
                            changedSubscriptionsList.Add(existingSubscription);
                            Console.WriteLine($"Updated Subscription ID: {existingSubscription.ID} in DB");
                        }
                        else
                        {
                            Console.WriteLine($"Subscription with ID: {apiSubscription.Id} already exists in DB");
                        }

                    }
                }
            }
            await SendPendingFulfillmentEmailAsync(pendingFulfillmentList);
            await SendChangedSubscriptionsEmailAsync(changedSubscriptionsList);
        }
        private async Task SendPendingFulfillmentEmailAsync(List<Subscription> pendingFulfillmentList)
        {
            if (pendingFulfillmentList.Any())
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Dear Team,");
                    sb.AppendLine();
                    sb.AppendLine("This is to inform you that the following subscriptions are currently pending fulfillment:");
                    sb.AppendLine();

                    foreach (var subscription in pendingFulfillmentList)
                    {
                        sb.AppendLine($"- **Subscription ID:** {subscription.SubsID}");
                        sb.AppendLine($"  **Name:** {subscription.SubscriptionName}");
                        sb.AppendLine($"  **Start Date:** {subscription.StartDate:MMMM dd, yyyy}");
                        sb.AppendLine($"  **Status:** Pending Fulfillment");
                        sb.AppendLine();
                    }

                    sb.AppendLine("Please take the necessary actions to fulfill these subscriptions as soon as possible.");
                    sb.AppendLine("If you have any questions or need further assistance, do not hesitate to reach out.");
                    sb.AppendLine();
                    sb.AppendLine("Thank you for your attention to this matter.");
                    sb.AppendLine();
                    sb.AppendLine(new string(' ', 60) + "Best regards,");
                    sb.AppendLine(new string(' ', 60) + "The LawToolBox Team");

                    var subject = "Pending Subscription Fulfillment";
                    var body = sb.ToString();
                    await _emailService.SendEmailAsync("pallav.sharma@lawtoolbox.com", subject, body);
                    Console.WriteLine("Pending fulfillment email sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending pending fulfillment email: {ex.Message}");
                }
            }
        }
        private async Task SendChangedSubscriptionsEmailAsync(List<Subscription> changedSubscriptionsList)
        {
            if (changedSubscriptionsList.Any())
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Dear Team,");
                    sb.AppendLine();
                    sb.AppendLine("This is to inform you that the following subscriptions have been updated:");
                    sb.AppendLine();

                    foreach (var subscription in changedSubscriptionsList)
                    {
                        sb.AppendLine($"- **Subscription ID:** {subscription.SubsID}");
                        sb.AppendLine($"  **Name:** {subscription.SubscriptionName}");
                        sb.AppendLine($"  **Status:** {subscription.SubscriptionStatus}");
                        sb.AppendLine($"  **Updated On:** {DateTime.Now:MMMM dd, yyyy}");
                        sb.AppendLine();
                    }

                    sb.AppendLine("Please review these changes at your earliest convenience.");
                    sb.AppendLine("If you have any questions or need further assistance, feel free to reach out.");
                    sb.AppendLine();
                    sb.AppendLine("Thank you for your attention to this matter.");
                    sb.AppendLine();
                    sb.AppendLine(new string(' ', 60) + "Best regards,");
                    sb.AppendLine(new string(' ', 60) + "The LawToolBox Team");

                    var subject = "Subscription Updates";
                    var body = sb.ToString();
                    await _emailService.SendEmailAsync("pallav.sharma@lawtoolbox.com", subject, body);
                    Console.WriteLine("Subscription update email sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending changed subscriptions email: {ex.Message}");
                }
            }
        }


    }
}
