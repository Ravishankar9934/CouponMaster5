import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Coupon } from 'src/app/models/coupon.model';
import { AuthService } from 'src/app/services/auth.service';
import { CouponService } from 'src/app/services/coupon.service';

@Component({
  selector: 'app-my-coupons',
  templateUrl: './my-coupons.component.html',
  styleUrls: ['./my-coupons.component.scss']
})
export class MyCouponsComponent implements OnInit {

    coupons: Coupon[] = [];
    isAdmin = false; // <--- Track this
  
    constructor(private couponService: CouponService, private auth: AuthService, private router: Router, private messageService: MessageService) { }
  
    ngOnInit(): void {
      // Check role immediately
      this.isAdmin = this.auth.isAdmin();
  
      this.couponService.getMyCoupons().subscribe({
        next: (data) => this.coupons = data,
        error: (err) => console.error(err)
      });
    }

}
