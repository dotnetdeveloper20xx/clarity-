import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ClientStore } from '../../../core/stores/client.store';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [RouterLink, FormsModule],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <h1 class="page-title">Clients</h1>
        <a routerLink="/clients/new" class="btn btn-primary btn-sm">+ New Client</a>
      </div>

      <!-- Search & Filters -->
      <div class="card-section mb-6">
        <div class="flex gap-4">
          <input type="text" [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Search clients..." class="input input-bordered input-sm flex-1" />
          <select [(ngModel)]="statusFilter" (ngModelChange)="onSearch()" class="select select-bordered select-sm">
            <option [ngValue]="undefined">All Statuses</option>
            <option [ngValue]="0">Pending</option>
            <option [ngValue]="1">Active</option>
            <option [ngValue]="2">On Hold</option>
          </select>
        </div>
      </div>

      <!-- Loading -->
      @if (store.loading()) {
        <div class="card-section">
          <div class="flex items-center gap-3">
            <span class="loading loading-spinner loading-md text-primary"></span>
            <span class="text-sm text-navy-600">Loading clients...</span>
          </div>
        </div>
      }

      <!-- Error -->
      @if (store.error()) {
        <div class="alert alert-error mb-4">
          <span>{{ store.error() }}</span>
          <button class="btn btn-sm btn-ghost" (click)="loadClients()">Retry</button>
        </div>
      }

      <!-- Empty State -->
      @if (store.isEmpty()) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">👥</div>
          <h3 class="text-lg font-medium text-navy-700">No clients found</h3>
          <p class="text-sm text-navy-500 mt-2">Create your first client to get started.</p>
          <a routerLink="/clients/new" class="btn btn-primary btn-sm mt-4">+ Create Client</a>
        </div>
      }

      <!-- Data Table -->
      @if (!store.loading() && !store.isEmpty() && !store.error()) {
        <div class="card-section overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Reference</th>
                <th>Name</th>
                <th>Email</th>
                <th>Status</th>
                <th>Type</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (client of store.items(); track client.id) {
                <tr class="hover:bg-base-200 cursor-pointer" [routerLink]="['/clients', client.id]">
                  <td class="font-mono text-xs">{{ client.referenceNumber }}</td>
                  <td class="font-medium">{{ client.name }}</td>
                  <td class="text-navy-500">{{ client.email }}</td>
                  <td>
                    <span class="badge badge-sm" [class]="getStatusClass(client.status)">{{ getStatusLabel(client.status) }}</span>
                  </td>
                  <td class="text-navy-500 text-xs">{{ client.clientType === 0 ? 'Individual' : 'Organisation' }}</td>
                  <td><a [routerLink]="['/clients', client.id]" class="btn btn-ghost btn-xs">View</a></td>
                </tr>
              }
            </tbody>
          </table>

          <!-- Pagination -->
          <div class="flex items-center justify-between mt-4 text-sm text-navy-500">
            <span>Showing {{ store.items().length }} of {{ store.totalCount() }} clients</span>
          </div>
        </div>
      }
    </div>
  `
})
export class ClientListComponent implements OnInit {
  searchTerm = '';
  statusFilter: number | undefined;

  constructor(public store: ClientStore) {}

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.store.loadClients({ searchTerm: this.searchTerm, status: this.statusFilter });
  }

  onSearch(): void {
    this.loadClients();
  }

  getStatusLabel(status: number): string {
    return ['Pending', 'Active', 'On Hold', 'Archived'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-warning', 'badge-success', 'badge-error', 'badge-ghost'][status] ?? '';
  }
}
