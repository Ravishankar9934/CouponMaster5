import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CouponListComponent } from './components/coupon-list/coupon-list.component';
import { CouponFormComponent } from './components/coupon-form/coupon-form.component';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './components/register/register.component';
import { AdminGuard } from './guards/admin.guard';
import { CouponDetailComponent } from './components/coupon-detail/coupon-detail.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }, // <--- NEW
  // Protect these routes
  { path: '', component: CouponListComponent, canActivate: [AuthGuard] },
  { path: 'add', component: CouponFormComponent, canActivate: [AuthGuard, AdminGuard] },
  { path: 'edit/:id', component: CouponFormComponent, canActivate: [AuthGuard, AdminGuard] },
  { path: 'coupon/:id', component: CouponDetailComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '' } // Wildcard: redirect unknown to home
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
