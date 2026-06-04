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
  private refreshTimer: any;

  readonly token = this.tokenSignal.asReadonly();
  readonly user = this.userSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.tokenSignal());
  readonly roles = computed(() => this.userSignal()?.roles ?? []);
  readonly fullName = computed(() => this.userSignal()?.fullName ?? '');

  constructor(private http: HttpClient, private router: Router) {
    if (this.isAuthenticated()) {
      this.scheduleTokenRefresh();
    }
  }

  login(request: LoginRequest): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).subscribe({
      next: (response) => {
        this.storeTokens(response);
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
    const refreshToken = localStorage.getItem('clarity_refresh_token');
    if (refreshToken) {
      this.http.post(`${environment.apiUrl}/auth/logout`, { refreshToken }).subscribe();
    }
    this.clearTokens();
    this.router.navigate(['/login']);
  }

  refreshAccessToken(): void {
    const refreshToken = localStorage.getItem('clarity_refresh_token');
    if (!refreshToken) {
      this.clearTokens();
      this.router.navigate(['/login']);
      return;
    }

    this.http.post<LoginResponse>(`${environment.apiUrl}/auth/refresh`, { refreshToken }).subscribe({
      next: (response) => this.storeTokens(response),
      error: () => {
        this.clearTokens();
        this.router.navigate(['/login']);
      }
    });
  }

  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(r => this.roles().includes(r));
  }

  private storeTokens(response: LoginResponse): void {
    localStorage.setItem('clarity_token', response.token);
    localStorage.setItem('clarity_refresh_token', (response as any).refreshToken ?? '');
    localStorage.setItem('clarity_user', JSON.stringify(response));
    this.tokenSignal.set(response.token);
    this.userSignal.set(response);
    this.scheduleTokenRefresh();
  }

  private clearTokens(): void {
    localStorage.removeItem('clarity_token');
    localStorage.removeItem('clarity_refresh_token');
    localStorage.removeItem('clarity_user');
    this.tokenSignal.set(null);
    this.userSignal.set(null);
    if (this.refreshTimer) clearTimeout(this.refreshTimer);
  }

  private scheduleTokenRefresh(): void {
    if (this.refreshTimer) clearTimeout(this.refreshTimer);
    // Refresh 1 minute before expiry (token lasts 15 min = 900s, refresh at 840s)
    this.refreshTimer = setTimeout(() => this.refreshAccessToken(), 840_000);
  }

  private loadUser(): LoginResponse | null {
    const stored = localStorage.getItem('clarity_user');
    return stored ? JSON.parse(stored) : null;
  }
}
