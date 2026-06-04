import { Component, OnInit, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatterStore } from '../../../core/stores/matter.store';
import { MatterDto, PaginatedList, ClientDto } from '../../../core/models/api.models';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../shared/components/toast/toast.component';
import { environment } from '@env/environment';

@Component({
  selector: 'app-matter-list',
  standalone: true,
  imports: [RouterLink, FormsModule, ModalComponent, ConfirmDialogComponent],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <div><h1 class="page-title">Matters</h1><p class="text-sm text-slate-500 mt-1">{{ store.totalCount() }} matters across all clients</p></div>
        <button (click)="openCreateModal()" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm">+ New Matter</button>
      </div>

      <!-- Filter Bar -->
      <div class="filter-bar mb-6">
        <div class="flex-1 min-w-[200px]">
          <input type="text" [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Search by reference, title..." class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 bg-white" />
        </div>
        <select [(ngModel)]="statusFilter" (ngModelChange)="onSearch()" class="border border-slate-300 rounded-lg px-3 py-2 text-sm bg-white">
          <option [ngValue]="undefined">All Statuses</option>
          <option [ngValue]="0">Open</option><option [ngValue]="1">In Progress</option><option [ngValue]="2">On Hold</option>
          <option [ngValue]="3">Awaiting Client</option><option [ngValue]="4">Awaiting 3rd Party</option>
          <option [ngValue]="5">Billing Review</option><option [ngValue]="6">Closed</option><option [ngValue]="7">Archived</option>
        </select>
        @if (searchTerm || statusFilter !== undefined) {
          <button (click)="searchTerm=''; statusFilter=undefined; onSearch()" class="text-xs text-blue-600 font-medium">Clear</button>
        }
      </div>

      @if (store.loading()) { <div class="card-section"><div class="animate-pulse space-y-3">@for(i of [1,2,3,4,5];track i){<div class="h-12 bg-slate-100 rounded"></div>}</div></div> }

      @if (store.isEmpty()) {
        <div class="card-section text-center py-16">
          <svg class="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"/></svg>
          <h3 class="text-base font-semibold text-slate-700">No matters found</h3>
          <p class="text-sm text-slate-500 mt-2">Create a new matter to begin legal work.</p>
          <button (click)="openCreateModal()" class="mt-4 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg">+ New Matter</button>
        </div>
      }

      @if (!store.loading() && !store.isEmpty()) {
        <div class="card-section overflow-x-auto">
          <table class="table w-full">
            <thead><tr><th>Reference</th><th>Title</th><th>Client</th><th>Type</th><th>Status</th><th>Lead</th><th>Opened</th><th class="text-right">Actions</th></tr></thead>
            <tbody>
              @for (m of store.items(); track m.id) {
                <tr class="cursor-pointer" [routerLink]="['/matters', m.id]">
                  <td class="font-mono text-xs font-semibold text-slate-700">{{ m.referenceNumber }}</td>
                  <td class="font-medium text-sm text-slate-900 max-w-[220px] truncate">{{ m.title }}</td>
                  <td class="text-sm text-slate-600">{{ m.clientName }}</td>
                  <td class="text-xs text-slate-500">{{ getMatterType(m.matterType) }}</td>
                  <td><span class="text-[11px] px-2 py-0.5 rounded-full font-medium" [class]="getStatusClass(m.status)">{{ getStatusLabel(m.status) }}</span></td>
                  <td class="text-xs text-slate-500">{{ m.leadConsultantName }}</td>
                  <td class="text-xs text-slate-400">{{ m.openedDate }}</td>
                  <td class="text-right" (click)="$event.stopPropagation()">
                    <a [routerLink]="['/matters', m.id]" class="text-xs text-blue-600 font-medium mr-2">View</a>
                  </td>
                </tr>
              }
            </tbody>
          </table>
          <div class="flex items-center justify-between mt-4 pt-4 border-t border-slate-100">
            <span class="text-xs text-slate-500">Showing {{ store.items().length }} of {{ store.totalCount() }} matters</span>
          </div>
        </div>
      }

      <!-- CREATE MATTER MODAL -->
      @if (showCreateModal()) {
        <app-modal title="New Matter" size="xl" (closed)="showCreateModal.set(false)">
          <div class="space-y-4">
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Client *</label>
              <select [(ngModel)]="form.clientId" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                <option value="">Select a client...</option>
                @for (c of clientOptions(); track c.id) { <option [value]="c.id">{{ c.name }} ({{ c.referenceNumber }})</option> }
              </select>
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Title *</label>
              <input [(ngModel)]="form.title" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="Residential Purchase - 45 Oak Lane">
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="text-xs font-medium text-slate-600 block mb-1">Matter Type *</label>
                <select [(ngModel)]="form.matterType" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                  <option [ngValue]="0">Conveyancing</option><option [ngValue]="1">Litigation</option><option [ngValue]="2">Family Law</option>
                  <option [ngValue]="3">Commercial</option><option [ngValue]="4">Employment</option><option [ngValue]="5">Criminal Law</option>
                  <option [ngValue]="7">Wills & Probate</option><option [ngValue]="8">Personal Injury</option><option [ngValue]="99">Other</option>
                </select>
              </div>
              <div>
                <label class="text-xs font-medium text-slate-600 block mb-1">Fee Arrangement *</label>
                <select [(ngModel)]="form.feeArrangement" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                  <option [ngValue]="0">Hourly</option><option [ngValue]="1">Fixed Fee</option><option [ngValue]="2">Hybrid</option>
                </select>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="text-xs font-medium text-slate-600 block mb-1">Estimated Value (£)</label>
                <input type="number" [(ngModel)]="form.estimatedValue" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="10000">
              </div>
              <div>
                <label class="text-xs font-medium text-slate-600 block mb-1">Lead Consultant *</label>
                <select [(ngModel)]="form.leadConsultantId" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                  <option value="">Select consultant...</option>
                  <option value="bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb">Sarah Johnson</option>
                  <option value="cccccccc-cccc-cccc-cccc-cccccccccccc">James Wilson</option>
                  <option value="11111111-aaaa-bbbb-cccc-dddddddddddd">Michael Chen</option>
                </select>
              </div>
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Description</label>
              <textarea [(ngModel)]="form.description" rows="3" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="Brief description of the legal matter..."></textarea>
            </div>
          </div>
          <div footer>
            <button (click)="showCreateModal.set(false)" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="createMatter()" [disabled]="!form.clientId || !form.title || !form.leadConsultantId || saving()" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">
              @if (saving()) { <span class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-1"></span> }
              Create Matter
            </button>
          </div>
        </app-modal>
      }
    </div>
  `
})
export class MatterListComponent implements OnInit {
  searchTerm = '';
  statusFilter: number | undefined;
  showCreateModal = signal(false);
  saving = signal(false);
  clientOptions = signal<ClientDto[]>([]);
  form: any = {};

  constructor(public store: MatterStore, private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void { this.store.loadMatters({ pageSize: 50 }); }
  onSearch(): void { this.store.loadMatters({ searchTerm: this.searchTerm, status: this.statusFilter, pageSize: 50 }); }

  openCreateModal(): void {
    this.form = { clientId: '', title: '', matterType: 3, feeArrangement: 0, estimatedValue: null, leadConsultantId: '', description: '' };
    this.showCreateModal.set(true);
    // Load clients for dropdown
    if (this.clientOptions().length === 0) {
      this.http.get<PaginatedList<ClientDto>>(`${environment.apiUrl}/clients?pageSize=100`).subscribe({
        next: (r) => this.clientOptions.set(r.items)
      });
    }
  }

  createMatter(): void {
    this.saving.set(true);
    this.http.post(`${environment.apiUrl}/matters`, this.form).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreateModal.set(false);
        this.store.loadMatters({ pageSize: 50 });
        this.toast.success('Matter created successfully');
      },
      error: (err) => { this.saving.set(false); this.toast.error(err?.error?.title || 'Failed to create matter'); }
    });
  }

  getStatusLabel(s: number): string { return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][s] ?? ''; }
  getStatusClass(s: number): string { return ['status-info', 'status-active', 'status-warning', 'status-warning', 'status-warning', 'status-pending', 'status-closed', 'status-closed'][s] ?? ''; }
  getMatterType(t: number): string { return ['Conveyancing', 'Litigation', 'Family Law', 'Commercial', 'Employment', 'Criminal', 'Immigration', 'Wills & Probate', 'Personal Injury', 'Other'][t] ?? 'Other'; }
}
