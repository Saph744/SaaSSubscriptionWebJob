using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SaaSFulfillmentApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SaaSFulfillmentApp.Repository.Interface
{
    public class SubscriptiondbRepository : ISubscriptiondbRepository
    {
        private readonly AppDbContext _context;

        public SubscriptiondbRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            string query = @"SELECT 
                       ID, 
                       SubsID, 
                       StartDate, 
                       EndDate, 
                       SubscriptionName, 
                       PublisherID, 
                       OfferID, 
                       PlanID, 
                       Quantity, 
                       BeneficiaryEmail, 
                       BeneficiaryObjectID, 
                       BeneficiaryTenantID, 
                       BeneficiaryPuID, 
                       PurchaserEmail, 
                       PurchaserObjectID, 
                       PurchaserTenantID, 
                       PurchaserPuID, 
                       AllowedCustomerOperations, 
                       SessionMode, 
                       IsFreeTrial, 
                       AutoRenew, 
                       SandBoxType, 
                       SubscriptionStatus, 
                       TermUnit, 
                       IsTest, 
                       Created, 
                       LastModified
                   FROM tblSAASSubscriptions";

            var subscriptions = new List<Subscription>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        var subscription = new Subscription
                        {
                            ID = result.GetInt32(result.GetOrdinal("ID")),
                            SubsID = result.GetString(result.GetOrdinal("SubsID")),
                            StartDate = result.GetDateTime(result.GetOrdinal("StartDate")),
                            EndDate = result.GetDateTime(result.GetOrdinal("EndDate")),
                            SubscriptionName = result.GetString(result.GetOrdinal("SubscriptionName")),
                            PublisherID = result.GetString(result.GetOrdinal("PublisherID")),
                            OfferID = result.GetString(result.GetOrdinal("OfferID")),
                            PlanID = result.GetString(result.GetOrdinal("PlanID")),
                            Quantity = result.GetString(result.GetOrdinal("Quantity")),
                            BeneficiaryEmail = result.GetString(result.GetOrdinal("BeneficiaryEmail")),
                            BeneficiaryObjectID = result.GetString(result.GetOrdinal("BeneficiaryObjectID")),
                            BeneficiaryTenantID = result.GetString(result.GetOrdinal("BeneficiaryTenantID")),
                            BeneficiaryPuID = result.GetString(result.GetOrdinal("BeneficiaryPuID")),
                            PurchaserEmail = result.GetString(result.GetOrdinal("PurchaserEmail")),
                            PurchaserObjectID = result.GetString(result.GetOrdinal("PurchaserObjectID")),
                            PurchaserTenantID = result.GetString(result.GetOrdinal("PurchaserTenantID")),
                            PurchaserPuID = result.GetString(result.GetOrdinal("PurchaserPuID")),
                            SessionMode = result.GetString(result.GetOrdinal("SessionMode")),
                            IsFreeTrial = result.GetBoolean(result.GetOrdinal("IsFreeTrial")),
                            AutoRenew = result.GetBoolean(result.GetOrdinal("AutoRenew")),
                            SandBoxType = result.GetString(result.GetOrdinal("SandBoxType")),
                            SubscriptionStatus = result.GetString(result.GetOrdinal("SubscriptionStatus")),
                            TermUnit = result.GetString(result.GetOrdinal("TermUnit")),
                            IsTest = result.GetBoolean(result.GetOrdinal("IsTest")),
                            Created = result.GetDateTime(result.GetOrdinal("Created")),
                            LastModified = result.GetDateTime(result.GetOrdinal("LastModified"))
                        };

                        var allowedOperationsJson = result["AllowedCustomerOperations"] as string;
                        if (!string.IsNullOrEmpty(allowedOperationsJson))
                        {
                            try
                            {
                                subscription.AllowedCustomerOperations = new List<string> { allowedOperationsJson };
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing AllowedCustomerOperations: {ex.Message}");
                                subscription.AllowedCustomerOperations = new List<string>();
                            }
                        }
                        else
                        {
                            subscription.AllowedCustomerOperations = new List<string>();
                        }

                        subscriptions.Add(subscription);
                    }
                }
            }

            return subscriptions;
        }



        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            //await _context.Subscriptions.AddAsync(subscription);
            //await _context.SaveChangesAsync();
            try
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

                var startDate = AdjustDateTime(subscription.StartDate);
                var endDate = AdjustDateTime(subscription.EndDate);
                var created = AdjustDateTime(subscription.Created);
                var lastModified = AdjustDateTime(subscription.LastModified);

                var query = @"
                            INSERT INTO tblSAASSubscriptions (
                                SubsID, 
                                StartDate, 
                                EndDate, 
                                SubscriptionName, 
                                PublisherID, 
                                OfferID, 
                                PlanID, 
                                Quantity, 
                                BeneficiaryEmail, 
                                BeneficiaryObjectID, 
                                BeneficiaryTenantID, 
                                BeneficiaryPuID, 
                                PurchaserEmail, 
                                PurchaserObjectID, 
                                PurchaserTenantID, 
                                PurchaserPuID, 
                                AllowedCustomerOperations, 
                                SessionMode, 
                                IsFreeTrial, 
                                AutoRenew, 
                                SandBoxType, 
                                SubscriptionStatus, 
                                TermUnit, 
                                IsTest, 
                                Created, 
                                LastModified
                            )
                            VALUES (
                                @SubsID, 
                                @StartDate, 
                                @EndDate, 
                                @SubscriptionName, 
                                @PublisherID, 
                                @OfferID, 
                                @PlanID, 
                                @Quantity, 
                                @BeneficiaryEmail, 
                                @BeneficiaryObjectID, 
                                @BeneficiaryTenantID, 
                                @BeneficiaryPuID, 
                                @PurchaserEmail, 
                                @PurchaserObjectID, 
                                @PurchaserTenantID, 
                                @PurchaserPuID, 
                                @AllowedCustomerOperations, 
                                @SessionMode, 
                                @IsFreeTrial, 
                                @AutoRenew, 
                                @SandBoxType, 
                                @SubscriptionStatus, 
                                @TermUnit, 
                                @IsTest, 
                                @Created, 
                                @LastModified
                            )";

                await _context.Database.ExecuteSqlRawAsync(query,
                    new SqlParameter("@SubsID", subscription.SubsID),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@SubscriptionName", subscription.SubscriptionName),
                    new SqlParameter("@PublisherID", subscription.PublisherID),
                    new SqlParameter("@OfferID", subscription.OfferID),
                    new SqlParameter("@PlanID", subscription.PlanID),
                    new SqlParameter("@Quantity", subscription.Quantity ?? (object)DBNull.Value),
                    new SqlParameter("@BeneficiaryEmail", subscription.BeneficiaryEmail),
                    new SqlParameter("@BeneficiaryObjectID", subscription.BeneficiaryObjectID),
                    new SqlParameter("@BeneficiaryTenantID", subscription.BeneficiaryTenantID),
                    new SqlParameter("@BeneficiaryPuID", subscription.BeneficiaryPuID),
                    new SqlParameter("@PurchaserEmail", subscription.PurchaserEmail),
                    new SqlParameter("@PurchaserObjectID", subscription.PurchaserObjectID),
                    new SqlParameter("@PurchaserTenantID", subscription.PurchaserTenantID),
                    new SqlParameter("@PurchaserPuID", subscription.PurchaserPuID),
                    new SqlParameter("@AllowedCustomerOperations", string.Join(",", subscription.AllowedCustomerOperations)),
                    new SqlParameter("@SessionMode", subscription.SessionMode),
                    new SqlParameter("@IsFreeTrial", subscription.IsFreeTrial),
                    new SqlParameter("@AutoRenew", subscription.AutoRenew),
                    new SqlParameter("@SandBoxType", subscription.SandBoxType),
                    new SqlParameter("@SubscriptionStatus", subscription.SubscriptionStatus),
                    new SqlParameter("@TermUnit", subscription.TermUnit),
                    new SqlParameter("@IsTest", subscription.IsTest),
                    new SqlParameter("@Created", created),
                    new SqlParameter("@LastModified", lastModified)
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            //_context.Subscriptions.Update(subscription);
            //await _context.SaveChangesAsync();
            DateTime minDate = new DateTime(1753, 1, 1);
            DateTime maxDate = new DateTime(9999, 12, 31);

            DateTime validatedStartDate = subscription.StartDate < minDate ? minDate : (subscription.StartDate > maxDate ? maxDate : subscription.StartDate);
            DateTime validatedEndDate = subscription.EndDate < minDate ? minDate : (subscription.EndDate > maxDate ? maxDate : subscription.EndDate);
            DateTime validatedCreatedDate = subscription.Created < minDate ? minDate : (subscription.Created > maxDate ? maxDate : subscription.Created);
            DateTime validatedLastModifiedDate = subscription.LastModified < minDate ? minDate : (subscription.LastModified > maxDate ? maxDate : subscription.LastModified);

            var query = @"
                            UPDATE tblSAASSubscriptions
                            SET 
                                SubscriptionName = @SubscriptionName,
                                PublisherID = @PublisherID,
                                OfferID = @OfferID,
                                PlanID = @PlanID,
                                Quantity = @Quantity,
                                BeneficiaryEmail = @BeneficiaryEmail,
                                BeneficiaryObjectID = @BeneficiaryObjectID,
                                BeneficiaryTenantID = @BeneficiaryTenantID,
                                BeneficiaryPuID = @BeneficiaryPuID,
                                PurchaserEmail = @PurchaserEmail,
                                PurchaserObjectID = @PurchaserObjectID,
                                PurchaserTenantID = @PurchaserTenantID,
                                PurchaserPuID = @PurchaserPuID,
                                AllowedCustomerOperations = @AllowedCustomerOperations,
                                SessionMode = @SessionMode,
                                IsFreeTrial = @IsFreeTrial,
                                AutoRenew = @AutoRenew,
                                SandBoxType = @SandBoxType,
                                SubscriptionStatus = @SubscriptionStatus,
                                TermUnit = @TermUnit,
                                IsTest = @IsTest,
                                StartDate = @StartDate,
                                EndDate = @EndDate,
                                Created = @Created,
                                LastModified = @LastModified
                            WHERE 
                                SubsID = @SubsID";

                            await _context.Database.ExecuteSqlRawAsync(query,
                                new SqlParameter("@SubscriptionName", subscription.SubscriptionName),
                                new SqlParameter("@PublisherID", subscription.PublisherID),
                                new SqlParameter("@OfferID", subscription.OfferID),
                                new SqlParameter("@PlanID", subscription.PlanID),
                                new SqlParameter("@Quantity", subscription.Quantity),
                                new SqlParameter("@BeneficiaryEmail", subscription.BeneficiaryEmail),
                                new SqlParameter("@BeneficiaryObjectID", subscription.BeneficiaryObjectID),
                                new SqlParameter("@BeneficiaryTenantID", subscription.BeneficiaryTenantID),
                                new SqlParameter("@BeneficiaryPuID", subscription.BeneficiaryPuID),
                                new SqlParameter("@PurchaserEmail", subscription.PurchaserEmail),
                                new SqlParameter("@PurchaserObjectID", subscription.PurchaserObjectID),
                                new SqlParameter("@PurchaserTenantID", subscription.PurchaserTenantID),
                                new SqlParameter("@PurchaserPuID", subscription.PurchaserPuID),
                                new SqlParameter("@AllowedCustomerOperations", string.Join(",", subscription.AllowedCustomerOperations)), // Assuming a delimited string
                                new SqlParameter("@SessionMode", subscription.SessionMode),
                                new SqlParameter("@IsFreeTrial", subscription.IsFreeTrial),
                                new SqlParameter("@AutoRenew", subscription.AutoRenew),
                                new SqlParameter("@SandBoxType", subscription.SandBoxType),
                                new SqlParameter("@SubscriptionStatus", subscription.SubscriptionStatus),
                                new SqlParameter("@TermUnit", subscription.TermUnit),
                                new SqlParameter("@IsTest", subscription.IsTest),
                                new SqlParameter("@StartDate", validatedStartDate),
                                new SqlParameter("@EndDate", validatedEndDate),
                                new SqlParameter("@Created", validatedCreatedDate),
                                new SqlParameter("@LastModified", validatedLastModifiedDate),
                                new SqlParameter("@SubsID", subscription.SubsID));

        }
        public async Task<Subscription> GetSubscriptionByIdAsync(string id)
        {
            string query = "SELECT ID, SubsID, StartDate, EndDate, Type, Beneficiary, Purchaser, Status FROM tblSAASSubscriptions WHERE SubsID = @id";
            var parameters = new[] { new SqlParameter("@id", id) };
            return await _context.Subscriptions.FromSqlRaw(query, parameters).FirstOrDefaultAsync();
        }
        public async Task AddSaaSHistoryAsync(SaaSHistory saaSHistory)
        {
            var query = @"
                            IF EXISTS (SELECT 1 FROM tblSAASHistory WHERE SubscriptionID = @SubscriptionID)
                            BEGIN
                                UPDATE tblSAASHistory
                                SET
                                    SubsID = CASE WHEN SubsID != @SubsID THEN @SubsID ELSE SubsID END,
                                    StartDate = CASE WHEN StartDate != @StartDate THEN @StartDate ELSE StartDate END,
                                    EndDate = CASE WHEN EndDate != @EndDate THEN @EndDate ELSE EndDate END,
                                    SubscriptionName = CASE WHEN SubscriptionName != @SubscriptionName THEN @SubscriptionName ELSE SubscriptionName END,
                                    PublisherID = CASE WHEN PublisherID != @PublisherID THEN @PublisherID ELSE PublisherID END,
                                    OfferID = CASE WHEN OfferID != @OfferID THEN @OfferID ELSE OfferID END,
                                    PlanID = CASE WHEN PlanID != @PlanID THEN @PlanID ELSE PlanID END,
                                    Quantity = CASE WHEN Quantity != @Quantity THEN @Quantity ELSE Quantity END,
                                    BeneficiaryEmail = CASE WHEN BeneficiaryEmail != @BeneficiaryEmail THEN @BeneficiaryEmail ELSE BeneficiaryEmail END,
                                    BeneficiaryObjectID = CASE WHEN BeneficiaryObjectID != @BeneficiaryObjectID THEN @BeneficiaryObjectID ELSE BeneficiaryObjectID END,
                                    BeneficiaryTenantID = CASE WHEN BeneficiaryTenantID != @BeneficiaryTenantID THEN @BeneficiaryTenantID ELSE BeneficiaryTenantID END,
                                    BeneficiaryPuID = CASE WHEN BeneficiaryPuID != @BeneficiaryPuID THEN @BeneficiaryPuID ELSE BeneficiaryPuID END,
                                    PurchaserEmail = CASE WHEN PurchaserEmail != @PurchaserEmail THEN @PurchaserEmail ELSE PurchaserEmail END,
                                    PurchaserObjectID = CASE WHEN PurchaserObjectID != @PurchaserObjectID THEN @PurchaserObjectID ELSE PurchaserObjectID END,
                                    PurchaserTenantID = CASE WHEN PurchaserTenantID != @PurchaserTenantID THEN @PurchaserTenantID ELSE PurchaserTenantID END,
                                    PurchaserPuID = CASE WHEN PurchaserPuID != @PurchaserPuID THEN @PurchaserPuID ELSE PurchaserPuID END,
                                    AllowedCustomerOperations = CASE WHEN AllowedCustomerOperations != @AllowedCustomerOperations THEN @AllowedCustomerOperations ELSE AllowedCustomerOperations END,
                                    SessionMode = CASE WHEN SessionMode != @SessionMode THEN @SessionMode ELSE SessionMode END,
                                    IsFreeTrial = CASE WHEN IsFreeTrial != @IsFreeTrial THEN @IsFreeTrial ELSE IsFreeTrial END,
                                    AutoRenew = CASE WHEN AutoRenew != @AutoRenew THEN @AutoRenew ELSE AutoRenew END,
                                    SandBoxType = CASE WHEN SandBoxType != @SandBoxType THEN @SandBoxType ELSE SandBoxType END,
                                    SubscriptionStatus = CASE WHEN SubscriptionStatus != @SubscriptionStatus THEN @SubscriptionStatus ELSE SubscriptionStatus END,
                                    TermUnit = CASE WHEN TermUnit != @TermUnit THEN @TermUnit ELSE TermUnit END,
                                    Created = CASE WHEN Created != @Created THEN @Created ELSE Created END,
                                    LastModified = CASE WHEN LastModified != @LastModified THEN @LastModified ELSE LastModified END,
                                    IsTest = CASE WHEN IsTest != @IsTest THEN @IsTest ELSE IsTest END
                                WHERE SubscriptionID = @SubscriptionID;
                            END
                            ELSE
                            BEGIN
                                INSERT INTO tblSAASHistory 
                                    (
                                        SubsID, 
                                        StartDate, 
                                        EndDate, 
                                        SubscriptionName,
                                        PublisherID,
                                        OfferID,
                                        PlanID,
                                        Quantity,
                                        BeneficiaryEmail,
                                        BeneficiaryObjectID,
                                        BeneficiaryTenantID,
                                        BeneficiaryPuID,
                                        PurchaserEmail,
                                        PurchaserObjectID,
                                        PurchaserTenantID,
                                        PurchaserPuID,
                                        AllowedCustomerOperations,
                                        SessionMode,
                                        IsFreeTrial,
                                        AutoRenew,
                                        SandBoxType,
                                        SubscriptionStatus,
                                        TermUnit,
                                        Created,
                                        LastModified,
                                        IsTest,
                                        SubscriptionID
                                    )
                                VALUES 
                                    (
                                        @SubsID, 
                                        @StartDate, 
                                        @EndDate, 
                                        @SubscriptionName,
                                        @PublisherID,
                                        @OfferID,
                                        @PlanID,
                                        @Quantity,
                                        @BeneficiaryEmail,
                                        @BeneficiaryObjectID,
                                        @BeneficiaryTenantID,
                                        @BeneficiaryPuID,
                                        @PurchaserEmail,
                                        @PurchaserObjectID,
                                        @PurchaserTenantID,
                                        @PurchaserPuID,
                                        @AllowedCustomerOperations,
                                        @SessionMode,
                                        @IsFreeTrial,
                                        @AutoRenew,
                                        @SandBoxType,
                                        @SubscriptionStatus,
                                        @TermUnit,
                                        @Created,
                                        @LastModified,
                                        @IsTest,
                                        @SubscriptionID
                                    );
                            END";

                                        await _context.Database.ExecuteSqlRawAsync(query,
                                            new SqlParameter("@SubsID", saaSHistory.SubsID),
                                            new SqlParameter("@StartDate", saaSHistory.StartDate),
                                            new SqlParameter("@EndDate", saaSHistory.EndDate),
                                            new SqlParameter("@SubscriptionName", saaSHistory.SubscriptionName),
                                            new SqlParameter("@PublisherID", saaSHistory.PublisherID),
                                            new SqlParameter("@OfferID", saaSHistory.OfferID),
                                            new SqlParameter("@PlanID", saaSHistory.PlanID),
                                            new SqlParameter("@Quantity", saaSHistory.Quantity),
                                            new SqlParameter("@BeneficiaryEmail", saaSHistory.BeneficiaryEmail),
                                            new SqlParameter("@BeneficiaryObjectID", saaSHistory.BeneficiaryObjectID),
                                            new SqlParameter("@BeneficiaryTenantID", saaSHistory.BeneficiaryTenantID),
                                            new SqlParameter("@BeneficiaryPuID", saaSHistory.BeneficiaryPuID),
                                            new SqlParameter("@PurchaserEmail", saaSHistory.PurchaserEmail),
                                            new SqlParameter("@PurchaserObjectID", saaSHistory.PurchaserObjectID),
                                            new SqlParameter("@PurchaserTenantID", saaSHistory.PurchaserTenantID),
                                            new SqlParameter("@PurchaserPuID", saaSHistory.PurchaserPuID),
                                            new SqlParameter("@AllowedCustomerOperations", string.Join(",", saaSHistory.AllowedCustomerOperations)), // Adjust if needed based on how you store lists in the database
                                            new SqlParameter("@SessionMode", saaSHistory.SessionMode),
                                            new SqlParameter("@IsFreeTrial", saaSHistory.IsFreeTrial),
                                            new SqlParameter("@AutoRenew", saaSHistory.AutoRenew),
                                            new SqlParameter("@SandBoxType", saaSHistory.SandBoxType),
                                            new SqlParameter("@SubscriptionStatus", saaSHistory.SubscriptionStatus),
                                            new SqlParameter("@TermUnit", saaSHistory.TermUnit),
                                            new SqlParameter("@Created", saaSHistory.Created),
                                            new SqlParameter("@LastModified", saaSHistory.LastModified),
                                            new SqlParameter("@IsTest", saaSHistory.IsTest),
                                            new SqlParameter("@SubscriptionID", saaSHistory.SubscriptionID)
                                        );


        }
    }
}
