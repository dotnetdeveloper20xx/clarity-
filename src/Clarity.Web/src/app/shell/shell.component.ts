import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="flex h-screen bg-slate-50">
      <!-- Sidebar - Deep slate professional -->
      <aside class="w-[260px] bg-slate-900 text-white flex flex-col border-r border-slate-800">
        <!-- Brand -->
        <div class="px-6 py-5 border-b border-slate-700/50">
          <a routerLink="/dashboard" class="block">
            <div class="flex items-center gap-2">
              <div class="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center text-sm font-bold">C</div>
              <div>
                <h1 class="text-base font-bold tracking-tight">Clarity</h1>
                <p class="text-[10px] text-slate-400 uppercase tracking-widest">Legal Practice</p>
              </div>
            </div>
          </a>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 px-3 py-4 space-y-0.5 overflow-y-auto">
          <div class="px-3 mb-2 text-[10px] font-semibold text-slate-500 uppercase tracking-widest">Practice</div>
          <a routerLink="/dashboard" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
            <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/></svg>
            Dashboard
          </a>
          <a routerLink="/clients" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
            <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/></svg>
            Clients
          </a>
          <a routerLink="/matters" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
            <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"/></svg>
            Matters
          </a>

          <div class="px-3 mb-2 mt-5 text-[10px] font-semibold text-slate-500 uppercase tracking-widest">Operations</div>
          <a routerLink="/time-recording" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
            <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
            Time Recording
          </a>
          @if (auth.hasAnyRole(['Finance', 'Admin'])) {
            <a routerLink="/billing" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
              <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
              Billing & Invoices
            </a>
          }

          @if (auth.hasAnyRole(['Compliance', 'Admin'])) {
            <div class="px-3 mb-2 mt-5 text-[10px] font-semibold text-slate-500 uppercase tracking-widest">Governance</div>
            <a routerLink="/compliance" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
              <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/></svg>
              Compliance
            </a>
          }

          <div class="px-3 mb-2 mt-5 text-[10px] font-semibold text-slate-500 uppercase tracking-widest">Activity</div>
          <a routerLink="/notifications" routerLinkActive="bg-blue-600/20 text-blue-300 border-l-2 border-blue-400" class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-[13px] text-slate-300 hover:bg-slate-800 hover:text-white transition-all">
            <svg class="w-4 h-4 opacity-70" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/></svg>
            Notifications
          </a>
        </nav>

        <!-- User section -->
        <div class="px-4 py-4 border-t border-slate-700/50">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 bg-slate-700 rounded-full flex items-center justify-center text-xs font-semibold text-slate-300">
              {{ auth.fullName().charAt(0) }}
            </div>
            <div class="flex-1 min-w-0">
              <div class="text-xs font-medium text-slate-200 truncate">{{ auth.fullName() }}</div>
              <div class="text-[10px] text-slate-500 truncate">{{ auth.roles().join(', ') }}</div>
            </div>
          </div>
          <button (click)="auth.logout()" class="mt-3 w-full text-[11px] text-slate-500 hover:text-slate-200 transition-colors text-left flex items-center gap-2">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
            Sign out
          </button>
        </div>
      </aside>

      <!-- Main Content Area -->
      <div class="flex-1 flex flex-col overflow-hidden">
        <!-- Top header bar -->
        <header class="h-[52px] bg-white border-b border-slate-200 flex items-center justify-between px-8 shrink-0">
          <div class="text-sm text-slate-600">
            <span class="font-medium text-slate-900">{{ auth.fullName() }}</span>
            <span class="text-slate-400 mx-2">•</span>
            <span class="text-slate-500">{{ getCurrentDate() }}</span>
          </div>
          <div class="flex items-center gap-3">
            <a routerLink="/notifications" class="relative p-2 text-slate-500 hover:text-slate-700 hover:bg-slate-100 rounded-lg transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/></svg>
            </a>
          </div>
        </header>

        <!-- Page Content -->
        <main class="flex-1 overflow-y-auto bg-slate-50">
          <router-outlet />
        </main>
      </div>
    </div>
  `
})
export class ShellComponent {
  constructor(public auth: AuthService) {}

  getCurrentDate(): string {
    return new Date().toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
  }
}
