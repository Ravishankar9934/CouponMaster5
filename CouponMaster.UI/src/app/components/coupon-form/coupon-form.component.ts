import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CouponService } from 'src/app/services/coupon.service';

@Component({
  selector: 'app-coupon-form',
  templateUrl: './coupon-form.component.html',
  styleUrls: ['./coupon-form.component.scss']
})
export class CouponFormComponent implements OnInit {

  couponForm: FormGroup;
  isEditMode = false;
  couponId: number = 0;

  constructor(
    private fb: FormBuilder,
    private service: CouponService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // 1. Initialize the Form Structure
    this.couponForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: [''],
      discountAmount: [0, [Validators.required, Validators.min(1), Validators.max(100)]],
      expiryDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    // 2. Check URL for ID
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.couponId = Number(id);
      this.loadCouponData(this.couponId);
    }
  }

  loadCouponData(id: number) {
    this.service.getCouponById(id).subscribe(data => {
      // 3. PatchValue fills the form with existing data
      // Note: We need to format the date string for the input field (yyyy-MM-dd)
      console.log('Loaded coupon data:', data);
      const formattedDate = new Date(data.expiryDate).toISOString().split('T')[0];

      this.couponForm.patchValue({
        title: data.title,
        description: data.description,
        discountAmount: data.discountAmount,
        expiryDate: formattedDate
      });
    });
  }

  onSubmit() {
    if (this.couponForm.invalid) return;

    // const couponData = this.couponForm.value;
    const rawValue = this.couponForm.value;
    // FIX: Convert JS Date to String manually before sending
    // PrimeNG sends a Date object, we need "2025-12-31"
    if (rawValue.expiryDate instanceof Date) {
      // This trick gets the YYYY-MM-DD part reliably
      rawValue.expiryDate = rawValue.expiryDate.toISOString().split('T')[0];
    }

    if (this.isEditMode) {
      console.log('Inside update', rawValue);
      this.service.updateCoupon(this.couponId, rawValue).subscribe(() => {
        this.router.navigate(['/']); // Go back to list
      });
    } else {
      this.service.createCoupon(rawValue).subscribe(() => {
        this.router.navigate(['/']); // Go back to list
      });
    }
  }
}