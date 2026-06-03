import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DashboardApiService, DashboardData } from '../../core/services/dashboard-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="page-container">
      <h1 class="page-title mb-6">Dashboard</h1>

      @if (loading()) {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          @for (i of [1,2,3,4]; track i) {
            <div class="card-section animate-pulse">
              <div class="h-4 bg-base-300 rounded w-1/2 mb-2"></div>
              <div class="h-8 bg-base-300 rounded w-1/3"></div>
            </div>
          }
        </div>
      }

      @if (data(); as d) {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <a routerLink="/matters" class="card-section hover:shadow-md transition-shadow cursor-pointer">
            <div class="data-label">Open Matters</div>
            <div class="text-3xl font-bold text-navy-900 mt-2">{{ d.openMattersCount }}</div>
            @if (d.overdueTasksCount > 0) {
              <div class="text-xs text-amber-600 mt-1">{{ d.overdueTasksCount }} overdue tasks</div>
            }
          </a>

          <a routerLink="/time-recording" class="card-section hover:shadow-md transition-shadow cursor-pointer">
            <div class="data-label">Unbilled Time</div>
            <div class="text-3xl font-bold text-navy-900 mt-2">£{{ d.unbilledTimeValue.toLocaleString('en-GB', {minimumFractionDigits: 0, maximumFractionDigits: 0}) }}</div>
            <div class="text-xs text-navy-500 mt-1">{{ d.pendingApprovalsCount }} awaiting approval</div>
          </a>

          <a routerLink="/billing" class="card-section hover:shadow-md transition-shadow cursor-pointer">
            <div class="data-label">Outstanding Invoices</div>
            <div class="text-3xl font-bold text-navy-900 mt-2">£{{ d.outstandingInvoicesTotal.toLocaleString('en-GB', {minimumFractionDigits: 0, maximumFractionDigits: 0}) }}</div>
            <div class="text-xs text-green-600 mt-1">£{{ d.paidThisMonthTotal.toLocaleString('en-GB', {minimumFractionDigits: 0, maximumFractionDigits: 0}) }} paid this month</div>
          </a>

          <a routerLink="/compliance" class="card-section hover:shadow-md transition-shadow cursor-pointer">
            <div class="data-label">Compliance Alerts</div>
            <div class="text-3xl font-bold mt-2" [class]="d.complianceAlertsCount > 0 ? 'text-red-600' : 'text-green-600'">{{ d.complianceAlertsCount }}</div>
            <div class="text-xs text-navy-500 mt-1">{{ d.pendingComplianceCount }} pending checks</div>
          </a>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-section">
            <h2 class="section-title mb-4">Quick Actions</h2>
            <div class="space-y-2">
              <a routerLink="/time-recording" class="btn btn-sm btn-outline btn-primary w-full justify-start">⏱️ Record Time</a>
              <a routerLink="/clients/new" class="btn btn-sm btn-outline btn-primary w-full justify-start">👤 New Client</a>
              <a routerLink="/matters" class="btn btn-sm btn-outline btn-primary w-full justify-start">📁 View Matters</a>
            </div>
          </div>

          <div class="card-section">
            <h2 class="section-title mb-4">Your Summary</h2>
            <div class="space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-navy-600">Draft Time Entries</span>
                <span class="font-medium">{{ d.draftTimeEntriesCount }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-navy-600">Unread Notifications</span>
                <span class="font-medium">{{ d.unreadNotificationsCount }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-navy-600">Pending Approvals</span>
                <span class="font-medium">{{ d.pendingApprovalsCount }}</span>
              </div>
            </div>
          </div>
        </div>
      }

      @if (error()) {
        <div class="alert alert-error">
          <span>{{ error() }}</span>
          <button class="btn btn-sm btn-ghost" (click)="load()">Retry</button>
        </div>
      }
    </div>
  `
})
export class DashboardComponent implements OnInit {
  data = signal<DashboardData | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(private api: DashboardApiService, public auth: AuthService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getDashboard().subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.title ?? 'Failed to load dashboard.'); this.loading.set(false); }
    });
  }
}
