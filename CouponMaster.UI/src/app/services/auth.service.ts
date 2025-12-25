import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5074/api/auth';

  // Real-time state: Are we logged in?
  // We check localStorage initially to set the default state
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  register(user: any) {
    // POST to /api/auth/register
    // Note: We change the URL slightly to point to register
    return this.http.post(this.apiUrl + '/register', user);
  }

  login(credentials: any) {
    return this.http.post<any>(this.apiUrl + '/login', credentials).pipe(
      tap(response => {
        // 1. Save Token
        localStorage.setItem('authToken', response.token);

        // 2. Notify the app: "User is now logged in!"
        this.isLoggedInSubject.next(true);
      })
    );
  }

  logout() {
    // 1. Clear Token
    localStorage.removeItem('authToken');

    // 2. Notify app
    this.isLoggedInSubject.next(false);

    // 3. Kick user out
    this.router.navigate(['/login']);
  }

  getToken() {
    return localStorage.getItem('authToken');
  }

  getRole(): string {
    const token = this.getToken();
    if (!token) return '';

    try {
      const decoded: any = jwtDecode(token);
      // The claim name for Role is usually slightly different in .NET tokens
      // It's often "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      // or simply "role" depending on your config.
      return decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
    } catch (e) {
      return '';
    }
  }

  // 2. Simple check
  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }
}