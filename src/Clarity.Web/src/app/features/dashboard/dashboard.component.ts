import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { DashboardApiService, DashboardData } from '../../core/services/dashboard-api.service';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '@env/environment';

interface RecentActivity { id: string; timestamp: string; action: string; entityType: string; entityId: string; userEmail: string; }

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-8">
        <div><h1 class="page-title">Dashboard</h1><p class="text-sm text-slate-500 mt-1">Welcome back, {{ auth.fullName() }}</p></div>
        <button (click)="load()" class="px-3 py-1.5 text-xs font-medium text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">↻ Refresh</button>
      </div>

      <!-- KPI Cards -->
      @if (loading()) {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">@for (i of [1,2,3,4]; track i) { <div class="kpi-card animate-pulse"><div class="h-4 bg-slate-200 rounded w-1/2 mb-3"></div><div class="h-8 bg-slate-200 rounded w-1/3"></div></div> }</div>
      }

      @if (data(); as d) {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <a routerLink="/matters" class="kpi-card cursor-pointer">
            <div class="kpi-label">Open Matters</div>
            <div class="kpi-value">{{ d.openMattersCount }}</div>
            @if (d.overdueTasksCount > 0) { <div class="kpi-trend-down">{{ d.overdueTasksCount }} overdue tasks</div> }
            @else { <div class="kpi-trend-up">All on track</div> }
          </a>
          <a routerLink="/time-recording" class="kpi-card cursor-pointer">
            <div class="kpi-label">Unbilled Time (WIP)</div>
            <div class="kpi-value">£{{ formatMoney(d.unbilledTimeValue) }}</div>
            <div class="kpi-trend-neutral">{{ d.pendingApprovalsCount }} awaiting approval</div>
          </a>
          <a routerLink="/billing" class="kpi-card cursor-pointer">
            <div class="kpi-label">Outstanding Invoices</div>
            <div class="kpi-value text-amber-600">£{{ formatMoney(d.outstandingInvoicesTotal) }}</div>
            <div class="kpi-trend-up">£{{ formatMoney(d.paidThisMonthTotal) }} received this month</div>
          </a>
          <a routerLink="/compliance" class="kpi-card cursor-pointer">
            <div class="kpi-label">Compliance Alerts</div>
            <div class="kpi-value" [class]="d.complianceAlertsCount > 0 ? 'text-red-600' : 'text-emerald-600'">{{ d.complianceAlertsCount }}</div>
            <div class="kpi-trend-neutral">{{ d.pendingComplianceCount }} pending checks</div>
          </a>
        </div>

        <!-- Second Row -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
          <!-- Quick Actions -->
          <div class="card-section">
            <h2 class="section-title mb-4">Quick Actions</h2>
            <div class="space-y-2">
              <a routerLink="/time-recording" class="flex items-center gap-3 px-3 py-2.5 rounded-lg hover:bg-slate-50 border border-slate-200 text-sm text-slate-700 font-medium transition-colors">
                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
                Record Time
              </a>
              <a routerLink="/clients" class="flex items-center gap-3 px-3 py-2.5 rounded-lg hover:bg-slate-50 border border-slate-200 text-sm text-slate-700 font-medium transition-colors">
                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"/></svg>
                New Client
              </a>
              <a routerLink="/matters" class="flex items-center gap-3 px-3 py-2.5 rounded-lg hover:bg-slate-50 border border-slate-200 text-sm text-slate-700 font-medium transition-colors">
                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 13h6m-3-3v6m5 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/></svg>
                View Matters
              </a>
              <a routerLink="/billing" class="flex items-center gap-3 px-3 py-2.5 rounded-lg hover:bg-slate-50 border border-slate-200 text-sm text-slate-700 font-medium transition-colors">
                <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 14l6-6m-5.5.5h.01m4.99 5h.01M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2v16z"/></svg>
                View Invoices
              </a>
            </div>
          </div>

          <!-- Your Summary -->
          <div class="card-section">
            <h2 class="section-title mb-4">Your Status</h2>
            <div class="space-y-4">
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Draft Time Entries</span>
                <span class="text-sm font-semibold px-2 py-0.5 bg-slate-100 rounded">{{ d.draftTimeEntriesCount }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Pending Approvals</span>
                <span class="text-sm font-semibold px-2 py-0.5 bg-blue-50 text-blue-700 rounded">{{ d.pendingApprovalsCount }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Unread Notifications</span>
                <span class="text-sm font-semibold px-2 py-0.5 bg-amber-50 text-amber-700 rounded">{{ d.unreadNotificationsCount }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Compliance Pending</span>
                <span class="text-sm font-semibold px-2 py-0.5 rounded" [class]="d.pendingComplianceCount > 0 ? 'bg-red-50 text-red-700' : 'bg-emerald-50 text-emerald-700'">{{ d.pendingComplianceCount }}</span>
              </div>
            </div>
          </div>

          <!-- Recent Activity -->
          <div class="card-section">
            <h2 class="section-title mb-4">Recent Activity</h2>
            @if (recentActivity().length === 0) {
              <p class="text-sm text-slate-500">No recent activity to display.</p>
            }
            <div class="space-y-3">
              @for (activity of recentActivity(); track activity.id) {
                <div class="flex items-start gap-3">
                  <div class="w-2 h-2 mt-2 rounded-full bg-blue-400 shrink-0"></div>
                  <div class="flex-1 min-w-0">
                    <p class="text-xs text-slate-700 truncate"><span class="font-medium">{{ activity.action }}</span> on {{ activity.entityType }}</p>
                    <p class="text-[10px] text-slate-400 mt-0.5">{{ activity.userEmail }} • {{ formatTimeAgo(activity.timestamp) }}</p>
                  </div>
                </div>
              }
            </div>
          </div>
        </div>
      }

      @if (error()) {
        <div class="card-section border-red-200 bg-red-50"><p class="text-sm text-red-700">{{ error() }}</p><button class="text-xs text-red-600 font-medium mt-2" (click)="load()">Retry</button></div>
      }
    </div>
  `
})
export class DashboardComponent implements OnInit {
  data = signal<DashboardData | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  recentActivity = signal<RecentActivity[]>([]);

  constructor(private api: DashboardApiService, public auth: AuthService, private http: HttpClient) {}

  ngOnInit(): void { this.load(); this.loadActivity(); }

  load(): void {
    this.loading.set(true); this.error.set(null);
    this.api.getDashboard().subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.title ?? 'Failed to load dashboard.'); this.loading.set(false); }
    });
  }

  loadActivity(): void {
    this.http.get<any[]>(`${environment.apiUrl}/audit?pageSize=8`).subscribe({
      next: (r: any) => this.recentActivity.set(r.items || []),
      error: () => {}
    });
  }

  formatMoney(val: number): string { return val >= 1000 ? (val / 1000).toFixed(1) + 'k' : val.toFixed(0); }

  formatTimeAgo(dateStr: string): string {
    const diff = Date.now() - new Date(dateStr).getTime();
    const hours = Math.floor(diff / 3600000);
    if (hours < 1) return 'Just now';
    if (hours < 24) return `${hours}h ago`;
    return `${Math.floor(hours / 24)}d ago`;
  }
}
