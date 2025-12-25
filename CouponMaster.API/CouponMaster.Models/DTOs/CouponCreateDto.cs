using System.ComponentModel.DataAnnotations; // For Validation

namespace CouponMaster.Models.DTOs
{
    public class CouponCreateDto
    {
        // We don't ask for ID, because DB generates it.
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 100)] // Validation: Discount must be 1-100%
        public int DiscountAmount { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}