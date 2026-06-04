import { Component, OnInit, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ClientStore } from '../../../core/stores/client.store';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../shared/components/toast/toast.component';
import { ClientDto } from '../../../core/models/api.models';
import { environment } from '@env/environment';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [RouterLink, FormsModule, ModalComponent, ConfirmDialogComponent],
  template: `
    <div class="page-container">
      <!-- Page Header -->
      <div class="flex items-center justify-between mb-6">
        <div>
          <h1 class="page-title">Clients</h1>
          <p class="text-sm text-slate-500 mt-1">{{ store.totalCount() }} clients registered</p>
        </div>
        <button (click)="openCreateModal()" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm">+ New Client</button>
      </div>

      <!-- Filter Bar -->
      <div class="filter-bar mb-6">
        <div class="flex-1 min-w-[200px]">
          <input type="text" [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Search by name, reference, email..."
            class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 bg-white" />
        </div>
        <select [(ngModel)]="statusFilter" (ngModelChange)="onSearch()" class="border border-slate-300 rounded-lg px-3 py-2 text-sm bg-white">
          <option [ngValue]="undefined">All Statuses</option>
          <option [ngValue]="0">Pending</option>
          <option [ngValue]="1">Active</option>
          <option [ngValue]="2">On Hold</option>
          <option [ngValue]="3">Archived</option>
        </select>
        <select [(ngModel)]="typeFilter" (ngModelChange)="onSearch()" class="border border-slate-300 rounded-lg px-3 py-2 text-sm bg-white">
          <option [ngValue]="undefined">All Types</option>
          <option [ngValue]="0">Individual</option>
          <option [ngValue]="1">Organisation</option>
        </select>
        @if (hasFilters()) {
          <button (click)="clearFilters()" class="text-xs text-blue-600 hover:text-blue-800 font-medium">Clear</button>
        }
      </div>

      <!-- Loading -->
      @if (store.loading()) {
        <div class="card-section"><div class="animate-pulse space-y-3">@for (i of [1,2,3,4,5]; track i) { <div class="h-12 bg-slate-100 rounded"></div> }</div></div>
      }

      <!-- Error -->
      @if (store.error()) {
        <div class="card-section border-red-200 bg-red-50"><p class="text-sm text-red-700">{{ store.error() }}</p><button class="text-xs text-red-600 font-medium mt-2 hover:underline" (click)="onSearch()">Retry</button></div>
      }

      <!-- Empty -->
      @if (store.isEmpty()) {
        <div class="card-section text-center py-16">
          <svg class="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
          <h3 class="text-base font-semibold text-slate-700">No clients found</h3>
          <p class="text-sm text-slate-500 mt-2">Create your first client to get started.</p>
          <button (click)="openCreateModal()" class="mt-4 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg">+ New Client</button>
        </div>
      }

      <!-- Table -->
      @if (!store.loading() && !store.isEmpty() && !store.error()) {
        <div class="card-section overflow-x-auto">
          <table class="table w-full">
            <thead>
              <tr><th>Reference</th><th>Name</th><th>Type</th><th>Email</th><th>City</th><th>Status</th><th class="text-right">Actions</th></tr>
            </thead>
            <tbody>
              @for (client of store.items(); track client.id) {
                <tr class="cursor-pointer" (click)="openEditModal(client)">
                  <td class="font-mono text-xs font-semibold text-slate-700">{{ client.referenceNumber }}</td>
                  <td class="font-medium text-sm text-slate-900">{{ client.name }}</td>
                  <td class="text-xs text-slate-500">{{ client.clientType === 0 ? 'Individual' : 'Organisation' }}</td>
                  <td class="text-sm text-slate-600">{{ client.email || '—' }}</td>
                  <td class="text-sm text-slate-500">{{ client.city || '—' }}</td>
                  <td><span class="text-[11px] px-2 py-0.5 rounded-full font-medium" [class]="getStatusClass(client.status)">{{ getStatusLabel(client.status) }}</span></td>
                  <td class="text-right">
                    <button (click)="openEditModal(client); $event.stopPropagation()" class="text-xs text-blue-600 hover:text-blue-800 font-medium mr-3">Edit</button>
                    <a [routerLink]="['/clients', client.id]" (click)="$event.stopPropagation()" class="text-xs text-slate-500 hover:text-slate-700 font-medium mr-3">View</a>
                    <button (click)="confirmDelete(client); $event.stopPropagation()" class="text-xs text-red-500 hover:text-red-700 font-medium">Delete</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
          <div class="flex items-center justify-between mt-4 pt-4 border-t border-slate-100">
            <span class="text-xs text-slate-500">Showing {{ store.items().length }} of {{ store.totalCount() }} clients</span>
          </div>
        </div>
      }

      <!-- CREATE/EDIT MODAL -->
      @if (showModal()) {
        <app-modal [title]="editingClient() ? 'Edit Client' : 'New Client'" size="xl" (closed)="closeModal()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="md:col-span-2">
              <label class="text-xs font-medium text-slate-600 block mb-1">Client Name *</label>
              <input [(ngModel)]="form.name" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500" placeholder="Harrington & Partners LLP">
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Type *</label>
              <select [(ngModel)]="form.clientType" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                <option [ngValue]="0">Individual</option><option [ngValue]="1">Organisation</option>
              </select>
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Email</label>
              <input [(ngModel)]="form.email" type="email" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="contact&#64;example.com">
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Phone</label>
              <input [(ngModel)]="form.phone" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="0207 123 4567">
            </div>
            @if (form.clientType === 1) {
              <div>
                <label class="text-xs font-medium text-slate-600 block mb-1">Company Number</label>
                <input [(ngModel)]="form.companyNumber" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="12345678">
              </div>
            }
            <div class="md:col-span-2">
              <label class="text-xs font-medium text-slate-600 block mb-1">Address</label>
              <input [(ngModel)]="form.addressLine1" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="45 Chancery Lane">
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">City</label>
              <input [(ngModel)]="form.city" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="London">
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Post Code</label>
              <input [(ngModel)]="form.postCode" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="WC2A 1JE">
            </div>
            <div>
              <label class="text-xs font-medium text-slate-600 block mb-1">Country</label>
              <input [(ngModel)]="form.country" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" value="United Kingdom">
            </div>
            <div class="md:col-span-2">
              <label class="text-xs font-medium text-slate-600 block mb-1">Notes</label>
              <textarea [(ngModel)]="form.notes" rows="3" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="Additional notes..."></textarea>
            </div>
          </div>
          <div footer>
            <button (click)="closeModal()" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="saveClient()" [disabled]="!form.name || saving()" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">
              @if (saving()) { <span class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-1"></span> }
              {{ editingClient() ? 'Save Changes' : 'Create Client' }}
            </button>
          </div>
        </app-modal>
      }

      <!-- DELETE CONFIRMATION -->
      @if (showDelete()) {
        <app-confirm-dialog title="Delete Client" [message]="'Are you sure you want to delete ' + deleteTarget()?.name + '? This action cannot be undone.'" confirmText="Delete" variant="danger" (confirmed)="deleteClient()" (cancelled)="showDelete.set(false)" />
      }
    </div>
  `
})
export class ClientListComponent implements OnInit {
  searchTerm = '';
  statusFilter: number | undefined;
  typeFilter: number | undefined;
  showModal = signal(false);
  showDelete = signal(false);
  editingClient = signal<ClientDto | null>(null);
  deleteTarget = signal<ClientDto | null>(null);
  saving = signal(false);
  form: any = {};

  hasFilters = computed(() => !!this.searchTerm || this.statusFilter !== undefined || this.typeFilter !== undefined);

  constructor(public store: ClientStore, private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void { this.store.loadClients(); }

  onSearch(): void { this.store.loadClients({ searchTerm: this.searchTerm, status: this.statusFilter }); }
  clearFilters(): void { this.searchTerm = ''; this.statusFilter = undefined; this.typeFilter = undefined; this.onSearch(); }

  openCreateModal(): void {
    this.editingClient.set(null);
    this.form = { name: '', clientType: 1, email: '', phone: '', addressLine1: '', city: '', postCode: '', country: 'United Kingdom', companyNumber: '', notes: '' };
    this.showModal.set(true);
  }

  openEditModal(client: ClientDto): void {
    this.editingClient.set(client);
    this.form = { ...client };
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); this.editingClient.set(null); }

  saveClient(): void {
    this.saving.set(true);
    const editing = this.editingClient();
    const req = editing
      ? this.http.put(`${environment.apiUrl}/clients/${editing.id}`, { id: editing.id, ...this.form })
      : this.http.post(`${environment.apiUrl}/clients`, this.form);

    req.subscribe({
      next: () => {
        this.saving.set(false);
        this.closeModal();
        this.store.loadClients({ searchTerm: this.searchTerm, status: this.statusFilter });
        this.toast.success(editing ? 'Client updated successfully' : 'Client created successfully');
      },
      error: (err) => { this.saving.set(false); this.toast.error(err?.error?.title || 'Failed to save client'); }
    });
  }

  confirmDelete(client: ClientDto): void { this.deleteTarget.set(client); this.showDelete.set(true); }

  deleteClient(): void {
    const target = this.deleteTarget();
    if (!target) return;
    this.http.delete(`${environment.apiUrl}/clients/${target.id}`).subscribe({
      next: () => { this.showDelete.set(false); this.store.loadClients(); this.toast.success('Client deleted'); },
      error: () => { this.showDelete.set(false); this.toast.error('Failed to delete client'); }
    });
  }

  getStatusLabel(s: number): string { return ['Pending', 'Active', 'On Hold', 'Archived'][s] ?? 'Unknown'; }
  getStatusClass(s: number): string { return ['status-pending', 'status-active', 'status-error', 'status-closed'][s] ?? ''; }
}
