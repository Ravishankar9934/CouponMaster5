using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Dapper;

namespace CouponMaster.WebJob
{
    public class Functions
    {
        // Connection string (Hardcoded for simplicity in this learning step)
        private static string _connectionString = "Server=localhost\\SQLEXPRESS;Database=CouponMasterDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        // This function runs every 1 minute
        // Cron Expression: "0 */1 * * * *" means "At second 0 of every 1st minute"
        public async Task ProcessExpiredCoupons([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"Janitor WebJob Started at: {DateTime.Now}");

            using (var connection = new SqlConnection(_connectionString))
            {
                // SQL Logic: Find coupons where ExpiryDate is in the past, but IsActive is still 1
                var sql = @"UPDATE Coupons 
                            SET IsActive = 0 
                            WHERE ExpiryDate < GETDATE() AND IsActive = 1";

                var rowsAffected = await connection.ExecuteAsync(sql);

                if (rowsAffected > 0)
                {
                    logger.LogInformation($"Cleaned up! Deactivated {rowsAffected} expired coupons.");
                }
                else
                {
                    logger.LogInformation("No expired coupons found.");
                }
            }
        }
    }
}