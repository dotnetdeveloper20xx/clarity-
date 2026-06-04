import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';

interface ComplianceCheckDto {
  id: string;
  clientId: string;
  clientName: string;
  checkType: number;
  status: number;
  riskLevel: number | null;
  performedById: string | null;
  performedAt: string | null;
  notes: string | null;
  createdAt: string;
}

@Component({
  selector: 'app-compliance-list',
  standalone: true,
  template: `
    <div class="page-container">
      <h1 class="page-title mb-6">Compliance</h1>

      <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div class="card-section border-l-4 border-green-500">
          <div class="data-label">Passed</div>
          <div class="text-2xl font-bold text-green-600 mt-1">{{ passedCount() }}</div>
        </div>
        <div class="card-section border-l-4 border-amber-500">
          <div class="data-label">Pending</div>
          <div class="text-2xl font-bold text-amber-600 mt-1">{{ pendingCount() }}</div>
        </div>
        <div class="card-section border-l-4 border-red-500">
          <div class="data-label">Failed</div>
          <div class="text-2xl font-bold text-red-600 mt-1">{{ failedCount() }}</div>
        </div>
        <div class="card-section border-l-4 border-purple-500">
          <div class="data-label">Under Investigation</div>
          <div class="text-2xl font-bold text-purple-600 mt-1">{{ investigationCount() }}</div>
        </div>
      </div>

      @if (loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading compliance checks...</div>
      }

      @if (!loading() && items().length > 0) {
        <div class="card-section">
          <h2 class="section-title mb-4">All Compliance Checks</h2>
          <div class="overflow-x-auto">
            <table class="table table-sm">
              <thead>
                <tr class="text-navy-600">
                  <th>Client</th>
                  <th>Check Type</th>
                  <th>Risk Level</th>
                  <th>Status</th>
                  <th>Notes</th>
                  <th>Date</th>
                </tr>
              </thead>
              <tbody>
                @for (check of items(); track check.id) {
                  <tr class="hover:bg-base-200">
                    <td class="font-medium">{{ check.clientName }}</td>
                    <td>{{ getCheckType(check.checkType) }}</td>
                    <td><span class="badge badge-sm" [class]="getRiskClass(check.riskLevel)">{{ getRiskLabel(check.riskLevel) }}</span></td>
                    <td><span class="badge badge-sm" [class]="getStatusClass(check.status)">{{ getStatusLabel(check.status) }}</span></td>
                    <td class="max-w-xs truncate text-xs text-navy-500">{{ check.notes || '—' }}</td>
                    <td class="text-xs text-navy-400">{{ check.performedAt ? formatDate(check.performedAt) : 'Pending' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }
    </div>
  `
})
export class ComplianceListComponent implements OnInit {
  items = signal<ComplianceCheckDto[]>([]);
  loading = signal(false);
  passedCount = signal(0);
  pendingCount = signal(0);
  failedCount = signal(0);
  investigationCount = signal(0);

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loading.set(true);
    // Use the audit endpoint to get compliance data - or direct query
    this.http.get<any[]>(`${environment.apiUrl}/compliance/checks`).subscribe({
      next: (checks) => {
        this.items.set(checks);
        this.passedCount.set(checks.filter(c => c.status === 1).length);
        this.pendingCount.set(checks.filter(c => c.status === 0).length);
        this.failedCount.set(checks.filter(c => c.status === 2).length);
        this.investigationCount.set(checks.filter(c => c.status === 3).length);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getCheckType(type: number): string {
    return ['AML', 'KYC', 'Conflict of Interest', 'Risk Assessment'][type] ?? 'Unknown';
  }

  getStatusLabel(status: number): string {
    return ['Pending', 'Pass', 'Fail', 'Investigation'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-warning', 'badge-success', 'badge-error', 'badge-secondary'][status] ?? '';
  }

  getRiskLabel(risk: number | null): string {
    if (risk === null) return 'N/A';
    return ['Low', 'Medium', 'High', 'Critical'][risk] ?? 'Unknown';
  }

  getRiskClass(risk: number | null): string {
    if (risk === null) return 'badge-ghost';
    return ['badge-success', 'badge-warning', 'badge-error', 'badge-error'][risk] ?? '';
  }

  formatDate(dateStr: string): string {
    return dateStr ? dateStr.substring(0, 10) : '';
  }
}
