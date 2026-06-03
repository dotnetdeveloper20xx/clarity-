import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="flex h-screen bg-base-200">
      <!-- Sidebar -->
      <aside class="w-64 bg-navy-900 text-white flex flex-col shadow-lg">
        <div class="p-4 border-b border-navy-700">
          <h1 class="text-xl font-bold tracking-wide">Clarity</h1>
          <p class="text-xs text-navy-300 mt-1">Legal Practice Management</p>
        </div>

        <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
          <a routerLink="/dashboard" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
            <span>📊</span> Dashboard
          </a>
          <a routerLink="/clients" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
            <span>👥</span> Clients
          </a>
          <a routerLink="/matters" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
            <span>📁</span> Matters
          </a>
          <a routerLink="/time-recording" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
            <span>⏱️</span> Time Recording
          </a>
          @if (auth.hasAnyRole(['Finance', 'Admin'])) {
            <a routerLink="/billing" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
              <span>💰</span> Billing
            </a>
          }
          @if (auth.hasAnyRole(['Compliance', 'Admin'])) {
            <a routerLink="/compliance" routerLinkActive="bg-navy-700" class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm hover:bg-navy-800 transition-colors">
              <span>🛡️</span> Compliance
            </a>
          }
        </nav>

        <div class="p-4 border-t border-navy-700">
          <div class="text-sm text-navy-300">{{ auth.fullName() }}</div>
          <div class="text-xs text-navy-400">{{ auth.roles().join(', ') }}</div>
          <button (click)="auth.logout()" class="mt-2 text-xs text-navy-400 hover:text-white transition-colors">
            Sign out
          </button>
        </div>
      </aside>

      <!-- Main Content -->
      <div class="flex-1 flex flex-col overflow-hidden">
        <!-- Top Bar -->
        <header class="h-14 bg-base-100 border-b border-base-300 flex items-center justify-between px-6 shadow-sm">
          <div class="text-sm text-navy-600 font-medium">
            Welcome back, {{ auth.fullName() }}
          </div>
          <div class="flex items-center gap-4">
            <button class="btn btn-ghost btn-sm btn-circle">🔔</button>
          </div>
        </header>

        <!-- Page Content -->
        <main class="flex-1 overflow-y-auto">
          <router-outlet />
        </main>
      </div>
    </div>
  `
})
export class ShellComponent {
  constructor(public auth: AuthService) {}
}
