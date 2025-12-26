import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Coupon } from 'src/app/models/coupon.model';
import { AuthService } from 'src/app/services/auth.service';
import { CouponService } from 'src/app/services/coupon.service';

@Component({
  selector: 'app-coupon-list',
  templateUrl: './coupon-list.component.html',
  styleUrls: ['./coupon-list.component.scss']
})
export class CouponListComponent implements OnInit {

  coupons: Coupon[] = [];
  isAdmin = false; // <--- Track this

  constructor(private couponService: CouponService, private auth: AuthService, private router: Router, private messageService: MessageService) { }

  ngOnInit(): void {
    // Check role immediately
    this.isAdmin = this.auth.isAdmin();

    this.couponService.getCoupons().subscribe({
      next: (data) => this.coupons = data,
      error: (err) => console.error(err)
    });
  }

  deleteCoupon(id: number) {
    if (confirm("Are you sure you want to delete this coupon?")) {
      this.couponService.deleteCoupon(id).subscribe(() => {
        // Refresh list after delete
        this.coupons = this.coupons.filter(c => c.id !== id);
        this.router.navigate(['/']);
      });
    }
    else {
      console.log("Deletion cancelled");
      this.router.navigate(['/']);
    }
  }

  redeemCoupon(coupon: Coupon) {
    // Call API (We will add this method to service next)
    this.couponService.redeemCoupon(coupon.id).subscribe({
      next: () => {
        // Show Success Toast
        this.messageService.add({
          severity: 'success',
          summary: 'Redeemed!',
          detail: `You have added ${coupon.title} to your wallet.`
        });
      },
      error: (err) => {
        // Show Error Toast
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'Already redeemed.'
        });
      }
    });
  }
}