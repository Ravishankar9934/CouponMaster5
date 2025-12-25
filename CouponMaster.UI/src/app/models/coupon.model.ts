export interface Coupon {
  id: number;
  title: string;
  description: string;
  discountAmount: number;
  expiryDate: string; // ISO Date string
//   status: string; // Matches "Active" or "Inactive" from your C# DTO
  isActive: boolean;
}