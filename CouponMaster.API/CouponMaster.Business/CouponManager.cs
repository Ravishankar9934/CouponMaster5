using CouponMaster.Abstraction;
using CouponMaster.Logging;
using CouponMaster.Models;
using CouponMaster.Models.DTOs;

namespace CouponMaster.Business
{
    public class CouponManager : ICouponManager
    {
        // Dependency Injection: The Manager needs the Repository
        private readonly ICouponRepository _repo;
        private readonly ILoggerManager _logger; // We inject our new logger

        public CouponManager(ICouponRepository repo, ILoggerManager logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            // 1. Get raw data from the Pantry (Repository)
            _logger.LogInfo("Fetching all coupons from repository.");
            var allCoupons = await _repo.GetAllCouponsAsync();

            // 2. Apply Business Logic (The "Chef's" work)
            // Example: Only return coupons that are Active and not expired
            var activeCoupons = allCoupons
                .Where(c => c.IsActive && c.ExpiryDate > DateTime.Now)
                .ToList();

            return activeCoupons;
        }

        public async Task<CouponReadDto?> GetCouponByIdAsync(int id)
        {
            // 1. Fetch raw data
            var coupon = await _repo.GetCouponByIdAsync(id);

            // 2. Return null immediately if not found (Controller will handle the 404)
            if (coupon == null)
            {
                _logger.LogWarn($"Coupon with Id {id} was requested but not found.");
                return null;
            }

            // 3. Map to DTO
            return new CouponReadDto
            {
                Id = coupon.Id,
                Title = coupon.Title,
                Description = coupon.Description,
                DiscountAmount = coupon.DiscountAmount,
                ExpiryDate = coupon.ExpiryDate,
                // Example of simple logic: Convert boolean to readable string
                Status = coupon.IsActive ? "Active" : "Inactive",
                IsActive = coupon.IsActive
            };
        }

        public async Task<CouponReadDto> CreateCouponAsync(CouponCreateDto couponDto)
        {
            _logger.LogInfo($"Creating coupon: {couponDto.Title}");

            // 1. Map DTO to Domain Model (Manual Mapping)
            // In PGCoupon, AutoMapper does this automatically.
            var coupon = new Coupon
            {
                Title = couponDto.Title,
                Description = couponDto.Description,
                DiscountAmount = couponDto.DiscountAmount,
                ExpiryDate = couponDto.ExpiryDate,
                IsActive = true // Default to active
            };

            // 2. Call Repository
            var newId = await _repo.CreateCouponAsync(coupon);

            // 3. Map back to ReadDto
            return new CouponReadDto
            {
                Id = newId,
                Title = coupon.Title,
                Status = "Active",
                DiscountAmount = coupon.DiscountAmount
            };
        }

        public async Task<bool> UpdateCouponAsync(int id, CouponCreateDto couponDto)
        {
            // 1. Check if the coupon actually exists before trying to update it
            var existingCoupon = await _repo.GetCouponByIdAsync(id);

            if (existingCoupon == null)
            {
                _logger.LogWarn($"Update failed. Coupon with Id {id} not found.");
                return false;
            }

            // 2. Map the new values from DTO to the Existing Model
            // We only update the fields the user is allowed to change
            existingCoupon.Title = couponDto.Title;
            existingCoupon.Description = couponDto.Description;
            existingCoupon.DiscountAmount = couponDto.DiscountAmount;
            existingCoupon.ExpiryDate = couponDto.ExpiryDate;

            // Note: We are NOT changing 'IsActive' or 'Id' here. 
            // If you wanted to update status, you'd usually have a separate "Patch" endpoint 
            // or include it in the DTO.

            // 3. Save to Database
            await _repo.UpdateCouponAsync(existingCoupon);

            _logger.LogInfo($"Coupon {id} updated successfully.");
            return true;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var existing = await _repo.GetCouponByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarn($"Attempted to delete non-existing coupon {id}");
                return false;
            }
            await _repo.DeleteCouponAsync(id);
            _logger.LogInfo($"Deleted coupon {id}");
            return true;
        }
    }
}