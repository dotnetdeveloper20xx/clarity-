import { Component, OnInit, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '@env/environment';
import { InvoiceDto, PaginatedList } from '../../../core/models/api.models';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <h1 class="page-title">Billing & Invoices</h1>
      </div>

      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div class="card-section">
          <div class="data-label">Draft Invoices</div>
          <div class="text-2xl font-bold text-navy-900 mt-1">{{ draftCount() }}</div>
        </div>
        <div class="card-section">
          <div class="data-label">Outstanding</div>
          <div class="text-2xl font-bold text-amber-600 mt-1">£{{ outstandingTotal().toLocaleString() }}</div>
        </div>
        <div class="card-section">
          <div class="data-label">Paid This Month</div>
          <div class="text-2xl font-bold text-green-600 mt-1">£{{ paidTotal().toLocaleString() }}</div>
        </div>
      </div>

      @if (loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading invoices...</div>
      }

      @if (!loading() && items().length > 0) {
        <div class="card-section overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Invoice #</th>
                <th>Client</th>
                <th>Matter</th>
                <th>Status</th>
                <th>Total</th>
                <th>Paid</th>
                <th>Outstanding</th>
              </tr>
            </thead>
            <tbody>
              @for (inv of items(); track inv.id) {
                <tr class="hover:bg-base-200">
                  <td class="font-mono text-xs font-medium">{{ inv.invoiceNumber }}</td>
                  <td>{{ inv.clientName }}</td>
                  <td class="font-mono text-xs">{{ inv.matterReference }}</td>
                  <td><span class="badge badge-sm" [class]="getInvoiceStatusClass(inv.status)">{{ getInvoiceStatusLabel(inv.status) }}</span></td>
                  <td class="font-medium">£{{ inv.totalAmount.toLocaleString() }}</td>
                  <td class="text-green-600">£{{ inv.paidAmount.toLocaleString() }}</td>
                  <td class="text-amber-600 font-medium">£{{ inv.outstandingAmount.toLocaleString() }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      @if (!loading() && items().length === 0) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">💰</div>
          <h3 class="text-lg font-medium text-navy-700">No invoices yet</h3>
          <p class="text-sm text-navy-500 mt-2">Invoices will appear here once generated from approved time entries.</p>
        </div>
      }
    </div>
  `
})
export class InvoiceListComponent implements OnInit {
  items = signal<InvoiceDto[]>([]);
  loading = signal(false);
  draftCount = signal(0);
  outstandingTotal = signal(0);
  paidTotal = signal(0);

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<PaginatedList<InvoiceDto>>(`${environment.apiUrl}/invoices`).subscribe({
      next: (result) => {
        this.items.set(result.items);
        this.draftCount.set(result.items.filter(i => i.status === 0).length);
        this.outstandingTotal.set(result.items.reduce((sum, i) => sum + i.outstandingAmount, 0));
        this.paidTotal.set(result.items.filter(i => i.status === 3).reduce((sum, i) => sum + i.totalAmount, 0));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getInvoiceStatusLabel(status: number): string {
    return ['Draft', 'Issued', 'Partially Paid', 'Paid', 'Cancelled', 'Written Off'][status] ?? 'Unknown';
  }

  getInvoiceStatusClass(status: number): string {
    return ['badge-ghost', 'badge-info', 'badge-warning', 'badge-success', 'badge-error', 'badge-error'][status] ?? '';
  }
}
