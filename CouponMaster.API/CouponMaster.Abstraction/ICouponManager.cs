using CouponMaster.Models;
using CouponMaster.Models.DTOs;

namespace CouponMaster.Abstraction
{
    public interface ICouponManager
    {
        // The API will call this method
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();

        Task<CouponReadDto> CreateCouponAsync(CouponCreateDto couponDto);
        Task<bool> DeleteCouponAsync(int id);
        Task<bool> UpdateCouponAsync(int id, CouponCreateDto couponDto);

        // Returns the DTO, not the raw Model
        Task<CouponReadDto?> GetCouponByIdAsync(int id);

        Task<bool> RedeemCouponAsync(string username, int couponId);
        Task<IEnumerable<Coupon>> GetCouponsByUsernameAsync(string username);
    }
}