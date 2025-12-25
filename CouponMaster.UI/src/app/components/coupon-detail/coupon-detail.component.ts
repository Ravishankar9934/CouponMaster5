import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CouponService } from 'src/app/services/coupon.service';
import { Coupon } from 'src/app/models/coupon.model';

@Component({
  selector: 'app-coupon-detail',
  templateUrl: './coupon-detail.component.html',
  styleUrls: ['./coupon-detail.component.scss']
})
export class CouponDetailComponent implements OnInit {

  coupon: Coupon | null = null;
  loading = true;
  couponId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private service: CouponService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // 1. Get ID from URL
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      this.couponId = Number(id);
      this.loadCoupon(this.couponId);
    }
  }

  loadCoupon(id: number) {
    this.service.getCouponById(id).subscribe({
      next: (data) => {
        this.coupon = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false; // Show error state if needed
      }
    });
  }
  
  goBack() {
    this.router.navigate(['/']);
  }
  
  redeem() {
    alert(`Coupon ${this.coupon?.title} Redeemed! Code copied to clipboard.`);
  }
}