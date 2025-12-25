using Microsoft.AspNetCore.Mvc;
using CouponMaster.Abstraction;
using CouponMaster.Models;
// removed Asp.Versioning usage to keep simple routes
//using Asp.Versioning.ApiExplorer;
using CouponMaster.Models.DTOs;

using Microsoft.AspNetCore.Authorization; // Import this

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

        // Constructor Injection: We ask for the Manager
        public CouponController(ICouponManager couponManager)
        {
            _couponManager = couponManager;
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
    }
}