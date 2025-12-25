using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq; // The Mocking Library
using CouponMaster.Business;
using CouponMaster.Abstraction;
using CouponMaster.Models;
using CouponMaster.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CouponMaster.Test
{
    [TestClass]
    public class CouponManagerTests
    {
        // These are the "Fakes"
        private Mock<ICouponRepository> _mockRepo;
        private Mock<ILoggerManager> _mockLogger;

        // This is the Real class we are testing
        private CouponManager _manager;

        [TestInitialize] // Runs before EVERY test
        public void Setup()
        {
            // 1. Create the fakes
            _mockRepo = new Mock<ICouponRepository>();
            _mockLogger = new Mock<ILoggerManager>();

            // 2. Inject fakes into the Manager (Constructor Injection)
            _manager = new CouponManager(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetActiveCoupons_Should_OnlyReturn_ActiveAndNonExpired()
        {
            // --- ARRANGE (Prepare the fake data) ---
            var fakeList = new List<Coupon>
            {
                // Coupon 1: Valid
                new Coupon { Id=1, Title="VALID", IsActive=true, ExpiryDate=DateTime.Now.AddDays(5) },
                // Coupon 2: Expired
                new Coupon { Id=2, Title="EXPIRED", IsActive=true, ExpiryDate=DateTime.Now.AddDays(-1) },
                // Coupon 3: Inactive
                new Coupon { Id=3, Title="INACTIVE", IsActive=false, ExpiryDate=DateTime.Now.AddDays(5) }
            };

            // Teach the Mock: "When someone calls GetAllCouponsAsync, return this fakeList"
            _mockRepo.Setup(repo => repo.GetAllCouponsAsync())
                     .ReturnsAsync(fakeList);

            // --- ACT (Run the real logic) ---
            var result = await _manager.GetActiveCouponsAsync();

            // --- ASSERT (Verify the result) ---
            // We expect only 1 coupon (the valid one)
            Assert.AreEqual(1, result.Count(), "Should return exactly 1 coupon");
            Assert.AreEqual("VALID", result.First().Title, "Should return the correct coupon");
        }

        [TestMethod]
        public async Task DeleteCoupon_Should_LogWarning_When_Id_NotFound()
        {
            // --- ARRANGE ---
            int invalidId = 99;

            // Teach Mock: "If asked for ID 99, return null"
            _mockRepo.Setup(repo => repo.GetCouponByIdAsync(invalidId))
                     .ReturnsAsync((Coupon)null);

            // --- ACT ---
            var result = await _manager.DeleteCouponAsync(invalidId);

            // --- ASSERT ---
            Assert.IsFalse(result); // Should return false

            // Verify: Did the Manager call _logger.LogWarn?
            // Times.Once() ensures it happened exactly once.
            _mockLogger.Verify(log => log.LogWarn(It.IsAny<string>()), Times.Once);
        }
    }
}