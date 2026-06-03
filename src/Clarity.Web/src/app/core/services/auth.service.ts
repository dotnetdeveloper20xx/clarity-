import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '@env/environment';
import { LoginRequest, LoginResponse } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenSignal = signal<string | null>(localStorage.getItem('clarity_token'));
  private userSignal = signal<LoginResponse | null>(this.loadUser());
  private loadingSignal = signal(false);
  private errorSignal = signal<string | null>(null);

  readonly token = this.tokenSignal.asReadonly();
  readonly user = this.userSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.tokenSignal());
  readonly roles = computed(() => this.userSignal()?.roles ?? []);
  readonly fullName = computed(() => this.userSignal()?.fullName ?? '');

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).subscribe({
      next: (response) => {
        localStorage.setItem('clarity_token', response.token);
        localStorage.setItem('clarity_user', JSON.stringify(response));
        this.tokenSignal.set(response.token);
        this.userSignal.set(response);
        this.loadingSignal.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorSignal.set(err?.error?.message ?? 'Login failed. Please try again.');
        this.loadingSignal.set(false);
      }
    });
  }

  logout(): void {
    localStorage.removeItem('clarity_token');
    localStorage.removeItem('clarity_user');
    this.tokenSignal.set(null);
    this.userSignal.set(null);
    this.router.navigate(['/login']);
  }

  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(r => this.roles().includes(r));
  }

  private loadUser(): LoginResponse | null {
    const stored = localStorage.getItem('clarity_user');
    return stored ? JSON.parse(stored) : null;
  }
}
