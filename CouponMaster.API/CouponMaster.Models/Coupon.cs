namespace CouponMaster.Models
{
    // This class represents a row in your SQL Database Table
    public class Coupon
    {
        // 'prop' is the snippet to create these quickly
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}