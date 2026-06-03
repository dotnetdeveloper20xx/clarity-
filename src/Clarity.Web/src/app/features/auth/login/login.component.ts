import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-navy-900">
      <div class="card w-96 bg-base-100 shadow-xl">
        <div class="card-body">
          <div class="text-center mb-6">
            <h1 class="text-2xl font-bold text-navy-900">Clarity</h1>
            <p class="text-sm text-navy-500 mt-1">Legal Practice Management</p>
          </div>

          @if (auth.error()) {
            <div class="alert alert-error text-sm mb-4">
              <span>{{ auth.error() }}</span>
            </div>
          }

          <form (ngSubmit)="onLogin()">
            <div class="form-control mb-4">
              <label class="label"><span class="label-text">Email</span></label>
              <input type="email" [(ngModel)]="email" name="email" class="input input-bordered w-full" placeholder="admin&#64;clarity.local" required />
            </div>

            <div class="form-control mb-6">
              <label class="label"><span class="label-text">Password</span></label>
              <input type="password" [(ngModel)]="password" name="password" class="input input-bordered w-full" placeholder="••••••••" required />
            </div>

            <button type="submit" class="btn btn-primary w-full" [disabled]="auth.loading()">
              @if (auth.loading()) {
                <span class="loading loading-spinner loading-sm"></span>
              }
              Sign In
            </button>
          </form>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';

  constructor(public auth: AuthService) {}

  onLogin(): void {
    if (this.email && this.password) {
      this.auth.login({ email: this.email, password: this.password });
    }
  }
}
