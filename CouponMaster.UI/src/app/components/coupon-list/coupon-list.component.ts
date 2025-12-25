import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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

  constructor(private couponService: CouponService, private auth: AuthService,private router: Router) { }

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
    else{
      console.log("Deletion cancelled");
      this.router.navigate(['/']);
    }
  }
}