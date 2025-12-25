namespace CouponMaster.Models.DTOs
{
    public class CouponReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Active" or "Expired"
        // Note: We hid "IsActive" boolean and "ExpiryDate" logic here.
        public bool IsActive { get; set; } // Added to support mapping in CouponManager 
    }
}