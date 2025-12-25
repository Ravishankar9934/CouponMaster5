import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  errorMessage = '';
  successMessage = ''; // NEW: To show green text
  isLoading = false;   // NEW: To disable button while loading

  registerForm = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) { }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.auth.register(this.registerForm.value).subscribe({
      
      next: () => {
        console.log('Registration successful'); // For debugging
        this.isLoading = false;
        this.successMessage = 'Registration successful! Redirecting to login...';
        
        // Wait 2 seconds so user reads the message, then redirect
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        console.log(this.registerForm.value)
        console.error('Registration error:', err);
        this.isLoading = false;
        // Handle "Username taken" or API errors
        this.errorMessage = err.error || 'Registration failed. Try again.';
      }
    });
  }
}