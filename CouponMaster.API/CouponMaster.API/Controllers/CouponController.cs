using Microsoft.AspNetCore.Mvc;
using CouponMaster.Abstraction;
using CouponMaster.Models;
// removed Asp.Versioning usage to keep simple routes
//using Asp.Versioning.ApiExplorer;
using CouponMaster.Models.DTOs;

using Microsoft.AspNetCore.Authorization; // Import this

using System.Security.Claims; // Needed to read Token
using Microsoft.Extensions.Logging;

namespace CouponMaster.API.Controllers
{
    // // [Route] defines the URL pattern: https://localhost:xxxx/api/coupon
    // [Route("api/[controller]")]
    // [ApiController]

    [Authorize] // <--- THIS LOCKS THE ENTIRE CONTROLLER
    [ApiController]
    [Route("api/[controller]")]
    // The URL will now be: api/coupon
    public class CouponController : ControllerBase
    {
        private readonly ICouponManager _couponManager;
        private readonly ILogger<CouponController> _logger;

        // Constructor Injection: We ask for the Manager
        public CouponController(ICouponManager couponManager, ILogger<CouponController> logger)
        {
            _couponManager = couponManager;
            _logger = logger;
        }

        // GET: api/coupon
        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            try
            {
                // Call the Business Layer
                var coupons = await _couponManager.GetActiveCouponsAsync();

                // Return 200 OK with the data
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                // In a real app, we would log this error here
                // Return 500 Internal Server Error
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/coupon/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var couponDto = await _couponManager.GetCouponByIdAsync(id);

            if (couponDto == null)
            {
                // Returns HTTP 404 Not Found
                return NotFound($"Coupon with Id {id} not found.");
            }

            // Returns HTTP 200 OK with the data
            return Ok(couponDto);
        }


        [Authorize(Roles = "Admin")]
        // POST: api/v1/coupon
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponCreateDto couponDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCoupon = await _couponManager.CreateCouponAsync(couponDto);

            // Returns 201 Created and the location of the new resource
            // We assume you have a GetById method, if not, simple Ok(createdCoupon) is fine for now
            return CreatedAtAction(nameof(GetCoupons), new { id = createdCoupon.Id }, createdCoupon);
        }

        // PUT: api/v1/coupon/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CouponCreateDto couponDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _couponManager.UpdateCouponAsync(id, couponDto);

            if (!result)
            {
                return NotFound($"Coupon with Id {id} not found.");
            }

            return NoContent(); // 204 No Content is standard for Updates
        }


        // DELETE: api/v1/coupon/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponManager.DeleteCouponAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("redeem/{id}")]
        public async Task<IActionResult> RedeemCoupon(int id)
        {
            _logger.LogInformation("RedeemCoupon called for coupon id: {Id}", id);
            // 1. Get User ID from the Token (The "Sub" or "NameIdentifier" claim)
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // If your token saves Username instead of ID, you might need to look up ID first.
            // Assuming we saved ID in the claim (Requires a small tweak to AuthController, see note below).
            // For now, let's assume we fetch the user by username:
            var username = User.Identity.Name;

            // Call Manager (We'll implement logic there)
            var success = await _couponManager.RedeemCouponAsync(username, id);

            if (!success) return BadRequest("You have already redeemed this coupon!");

            return Ok(new { message = "Coupon redeemed successfully!" });
        }

        [HttpGet("my-coupons")]
        public async Task<IActionResult> GetMyCoupons()
        {
            var username = User.Identity.Name;
            var coupons = await _couponManager.GetCouponsByUsernameAsync(username);
            return Ok(coupons);
        }
    }
}