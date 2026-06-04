import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { InvoiceDto, PaginatedList } from '../../../core/models/api.models';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ToastService } from '../../../shared/components/toast/toast.component';
import { environment } from '@env/environment';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [FormsModule, ModalComponent],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <div><h1 class="page-title">Billing & Invoices</h1><p class="text-sm text-slate-500 mt-1">Manage invoices and track payments</p></div>
      </div>

      <!-- KPI Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div class="kpi-card">
          <div class="kpi-label">Draft</div>
          <div class="kpi-value">{{ draftCount() }}</div>
          <div class="kpi-trend-neutral">Awaiting review</div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">Outstanding</div>
          <div class="kpi-value text-amber-600">£{{ outstandingTotal().toLocaleString() }}</div>
          <div class="kpi-trend-down">{{ issuedCount() }} invoices</div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">Paid (All Time)</div>
          <div class="kpi-value text-emerald-600">£{{ paidTotal().toLocaleString() }}</div>
          <div class="kpi-trend-up">{{ paidCount() }} invoices</div>
        </div>
        <div class="kpi-card">
          <div class="kpi-label">Total Invoiced</div>
          <div class="kpi-value">£{{ totalInvoiced().toLocaleString() }}</div>
          <div class="kpi-trend-neutral">{{ items().length }} invoices</div>
        </div>
      </div>

      <!-- Filter Bar -->
      <div class="filter-bar mb-6">
        <select [(ngModel)]="statusFilter" (ngModelChange)="load()" class="border border-slate-300 rounded-lg px-3 py-2 text-sm bg-white">
          <option [ngValue]="undefined">All Statuses</option>
          <option [ngValue]="0">Draft</option><option [ngValue]="1">Issued</option><option [ngValue]="2">Partially Paid</option>
          <option [ngValue]="3">Paid</option><option [ngValue]="4">Cancelled</option><option [ngValue]="5">Written Off</option>
        </select>
        @if (statusFilter !== undefined) {
          <button (click)="statusFilter = undefined; load()" class="text-xs text-blue-600 font-medium">Clear</button>
        }
      </div>

      @if (loading()) {
        <div class="card-section"><div class="animate-pulse space-y-3">@for (i of [1,2,3,4]; track i) { <div class="h-12 bg-slate-100 rounded"></div> }</div></div>
      }

      @if (!loading() && items().length > 0) {
        <div class="card-section overflow-x-auto">
          <table class="table w-full">
            <thead><tr><th>Invoice #</th><th>Client</th><th>Matter</th><th>Status</th><th>Issue Date</th><th>Due Date</th><th class="text-right">Total</th><th class="text-right">Paid</th><th class="text-right">Outstanding</th><th>Actions</th></tr></thead>
            <tbody>
              @for (inv of items(); track inv.id) {
                <tr>
                  <td class="font-mono text-sm font-semibold text-slate-800">{{ inv.invoiceNumber }}</td>
                  <td class="text-sm text-slate-700">{{ inv.clientName }}</td>
                  <td class="font-mono text-xs text-slate-500">{{ inv.matterReference }}</td>
                  <td><span class="text-[11px] px-2 py-0.5 rounded-full font-medium" [class]="getStatusClass(inv.status)">{{ getStatusLabel(inv.status) }}</span></td>
                  <td class="text-xs text-slate-500">{{ inv.issueDate || '—' }}</td>
                  <td class="text-xs text-slate-500">{{ inv.dueDate || '—' }}</td>
                  <td class="text-right text-sm font-medium">£{{ inv.totalAmount.toLocaleString() }}</td>
                  <td class="text-right text-sm text-emerald-600">£{{ inv.paidAmount.toLocaleString() }}</td>
                  <td class="text-right text-sm font-medium text-amber-600">£{{ (inv.totalAmount - inv.paidAmount).toLocaleString() }}</td>
                  <td>
                    @if (inv.status === 0) {
                      <button (click)="issueInvoice(inv)" class="text-xs text-blue-600 hover:text-blue-800 font-medium mr-2">Issue</button>
                    }
                    @if (inv.status === 1 || inv.status === 2) {
                      <button (click)="openPaymentModal(inv)" class="text-xs text-emerald-600 hover:text-emerald-800 font-medium">Record Payment</button>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      @if (!loading() && items().length === 0) {
        <div class="card-section text-center py-16">
          <svg class="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
          <h3 class="text-base font-semibold text-slate-700">No invoices found</h3>
          <p class="text-sm text-slate-500 mt-2">Invoices are generated from approved time entries on matters.</p>
        </div>
      }

      <!-- RECORD PAYMENT MODAL -->
      @if (showPaymentModal()) {
        <app-modal title="Record Payment" size="md" (closed)="showPaymentModal.set(false)">
          <div class="space-y-4">
            <div class="bg-slate-50 rounded-lg p-3 text-sm">
              <div class="flex justify-between"><span class="text-slate-600">Invoice:</span><span class="font-semibold">{{ paymentInvoice()?.invoiceNumber }}</span></div>
              <div class="flex justify-between mt-1"><span class="text-slate-600">Outstanding:</span><span class="font-semibold text-amber-600">£{{ ((paymentInvoice()?.totalAmount || 0) - (paymentInvoice()?.paidAmount || 0)).toLocaleString() }}</span></div>
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Amount (£) *</label>
              <input type="number" [(ngModel)]="paymentForm.amount" min="0.01" step="0.01" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500">
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Payment Date *</label>
              <input type="date" [(ngModel)]="paymentForm.paymentDate" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Method</label>
              <select [(ngModel)]="paymentForm.paymentMethod" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                <option [ngValue]="0">Bank Transfer</option><option [ngValue]="1">Card</option><option [ngValue]="2">Cheque</option><option [ngValue]="3">Cash</option>
              </select>
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Reference</label>
              <input [(ngModel)]="paymentForm.reference" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="BACS-REF-001">
            </div>
          </div>
          <div footer>
            <button (click)="showPaymentModal.set(false)" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="savePayment()" [disabled]="!paymentForm.amount || !paymentForm.paymentDate" class="px-4 py-2 text-sm font-medium text-white bg-emerald-600 rounded-lg hover:bg-emerald-700 disabled:opacity-50">Record Payment</button>
          </div>
        </app-modal>
      }
    </div>
  `
})
export class InvoiceListComponent implements OnInit {
  items = signal<InvoiceDto[]>([]);
  loading = signal(false);
  statusFilter: number | undefined;
  showPaymentModal = signal(false);
  paymentInvoice = signal<InvoiceDto | null>(null);
  paymentForm = { amount: 0, paymentDate: '', paymentMethod: 0, reference: '' };

  draftCount = computed(() => this.items().filter(i => i.status === 0).length);
  issuedCount = computed(() => this.items().filter(i => i.status === 1 || i.status === 2).length);
  paidCount = computed(() => this.items().filter(i => i.status === 3).length);
  outstandingTotal = computed(() => this.items().filter(i => i.status === 1 || i.status === 2).reduce((s, i) => s + i.totalAmount - i.paidAmount, 0));
  paidTotal = computed(() => this.items().filter(i => i.status === 3).reduce((s, i) => s + i.totalAmount, 0));
  totalInvoiced = computed(() => this.items().reduce((s, i) => s + i.totalAmount, 0));

  constructor(private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    let url = `${environment.apiUrl}/invoices?pageSize=50`;
    if (this.statusFilter !== undefined) url += `&status=${this.statusFilter}`;
    this.http.get<PaginatedList<InvoiceDto>>(url).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  issueInvoice(inv: InvoiceDto): void {
    this.http.put(`${environment.apiUrl}/invoices/${inv.id}/issue`, {}).subscribe({
      next: () => { this.load(); this.toast.success(`Invoice ${inv.invoiceNumber} issued`); },
      error: (err) => this.toast.error(err?.error?.title || 'Failed to issue invoice')
    });
  }

  openPaymentModal(inv: InvoiceDto): void {
    this.paymentInvoice.set(inv);
    this.paymentForm = { amount: inv.totalAmount - inv.paidAmount, paymentDate: new Date().toISOString().split('T')[0], paymentMethod: 0, reference: '' };
    this.showPaymentModal.set(true);
  }

  savePayment(): void {
    const inv = this.paymentInvoice();
    if (!inv) return;
    this.http.post(`${environment.apiUrl}/payments`, { invoiceId: inv.id, ...this.paymentForm }).subscribe({
      next: () => { this.showPaymentModal.set(false); this.load(); this.toast.success('Payment recorded successfully'); },
      error: (err) => this.toast.error(err?.error?.title || 'Failed to record payment')
    });
  }

  getStatusLabel(s: number): string { return ['Draft', 'Issued', 'Partially Paid', 'Paid', 'Cancelled', 'Written Off'][s] ?? ''; }
  getStatusClass(s: number): string { return ['status-closed', 'status-info', 'status-warning', 'status-active', 'status-error', 'status-error'][s] ?? ''; }
}
