import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Coupon } from '../models/coupon.model';

@Injectable({
  providedIn: 'root'
})
export class CouponService {
  // REPLACE 5000 WITH YOUR ACTUAL .NET PORT (Check your terminal)
  private apiUrl = 'http://localhost:5074/api/coupon';

  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>('http://localhost:5074/api/auth/login', { username, password });
  }

  // Returns an Observable (Stream of data)
  getCoupons(): Observable<Coupon[]> {
    return this.http.get<Coupon[]>(this.apiUrl);
  }

  // 1. Get Single (For Editing later)
  getCouponById(id: number): Observable<Coupon> {
    return this.http.get<Coupon>(`${this.apiUrl}/${id}`);
  }

  // 2. Create (POST)
  createCoupon(coupon: any): Observable<any> {
    // We send 'coupon' as the JSON Payload
    return this.http.post(this.apiUrl, coupon);
  }

  // 3. Update (PUT)
  updateCoupon(id: number, coupon: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, coupon);
  }

  // 4. Delete (DELETE)
  deleteCoupon(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  redeemCoupon(id: number) {
    return this.http.post(`${this.apiUrl}/redeem/${id}`, {});
  }

  getMyCoupons() {
    return this.http.get<Coupon[]>(`${this.apiUrl}/my-coupons`);
  }
}