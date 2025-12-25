using System.Data;
// using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Dapper;
using CouponMaster.Abstraction;
using CouponMaster.Models;
using Microsoft.Extensions.Configuration; // To read config

namespace CouponMaster.Repository
{
    // This class signs the contract (ICouponRepository)
    public class CouponRepository : ICouponRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // Constructor Injection: We ask for Configuration to get the Connection String
        public CouponRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            // In a real app, this comes from appsettings.json
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(_connectionString),
            "Connection string 'DefaultConnection' is missing.");
        }

        // [Obsolete]
        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            // 1. Create a connection to SQL Server
            // "using" ensures the connection closes automatically even if errors occur
            using (var connection = new SqlConnection(_connectionString))
            {
                // 2. Define your SQL query
                var sql = "SELECT Id, Title, Description, DiscountAmount, ExpiryDate, IsActive FROM Coupons";

                // 3. Execute with Dapper
                // QueryAsync<Coupon> tells Dapper: "Take the result and fit it into the Coupon model"
                var result = await connection.QueryAsync<Coupon>(sql);

                return result;
            }
        }

        // ... inside the class

        public async Task<int> CreateCouponAsync(Coupon coupon)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Coupons (Title, Description, DiscountAmount, ExpiryDate, IsActive) 
                    VALUES (@Title, @Description, @DiscountAmount, @ExpiryDate, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() as int);"; // Returns the new ID
                return await connection.ExecuteScalarAsync<int>(sql, coupon);
            }
        }

        public async Task<int> UpdateCouponAsync(Coupon coupon)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Coupons 
                    SET Title = @Title, Description = @Description, 
                        DiscountAmount = @DiscountAmount, ExpiryDate = @ExpiryDate, IsActive = @IsActive
                    WHERE Id = @Id";
                return await connection.ExecuteAsync(sql, coupon);
            }
        }

        public async Task<int> DeleteCouponAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Coupons WHERE Id = @Id";
                return await connection.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<Coupon?> GetCouponByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // We use QuerySingleOrDefaultAsync because we expect 0 or 1 row.
                // If >1 row exists (duplicate IDs), it throws an error (good for integrity).
                var sql = "SELECT * FROM Coupons WHERE Id = @Id";

                return await connection.QuerySingleOrDefaultAsync<Coupon>(sql, new { Id = id });
            }
        }
    }
}