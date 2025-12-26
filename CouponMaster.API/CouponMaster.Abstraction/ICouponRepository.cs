using CouponMaster.Models;

namespace CouponMaster.Abstraction
{
    public interface ICouponRepository
    {
        // We return a Task because database calls are "Async" (they take time)
        // IEnumerable is a basic list (lighter than List<T>)
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();

        // Later we will add: Task<int> CreateCouponAsync(Coupon coupon);

        Task<int> CreateCouponAsync(Coupon coupon); // Returns ID of new coupon
        Task<int> UpdateCouponAsync(Coupon coupon); // Returns rows affected
        Task<int> DeleteCouponAsync(int id);
        
       // Returns "Coupon?" because it might be null if the ID doesn't exist
        Task<Coupon?> GetCouponByIdAsync(int id);

        Task<bool> RedeemCouponAsync(string username, int couponId);
        Task<IEnumerable<Coupon>> GetCouponsByUsernameAsync(string username);

    }
}