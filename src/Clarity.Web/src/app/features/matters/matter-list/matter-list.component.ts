import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatterStore } from '../../../core/stores/matter.store';

@Component({
  selector: 'app-matter-list',
  standalone: true,
  imports: [RouterLink, FormsModule],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <div>
          <h1 class="page-title">Matters</h1>
          <p class="page-subtitle">Manage legal matters across all clients</p>
        </div>
      </div>

      <!-- Filter Bar -->
      <div class="filter-bar mb-6">
        <div class="flex-1 min-w-[200px]">
          <input type="text" [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Search by reference, title..."
            class="input input-sm w-full bg-slate-50 border-slate-200 focus:bg-white" />
        </div>
        <select [(ngModel)]="statusFilter" (ngModelChange)="onSearch()" class="select select-sm bg-slate-50 border-slate-200 text-sm">
          <option [ngValue]="undefined">All Statuses</option>
          <option [ngValue]="0">Open</option>
          <option [ngValue]="1">In Progress</option>
          <option [ngValue]="2">On Hold</option>
          <option [ngValue]="3">Awaiting Client</option>
          <option [ngValue]="4">Awaiting Third Party</option>
          <option [ngValue]="5">Billing Review</option>
          <option [ngValue]="6">Closed</option>
          <option [ngValue]="7">Archived</option>
        </select>
        <select [(ngModel)]="typeFilter" (ngModelChange)="onSearch()" class="select select-sm bg-slate-50 border-slate-200 text-sm">
          <option [ngValue]="undefined">All Types</option>
          <option [ngValue]="0">Conveyancing</option>
          <option [ngValue]="1">Litigation</option>
          <option [ngValue]="2">Family Law</option>
          <option [ngValue]="3">Commercial</option>
          <option [ngValue]="4">Employment</option>
          <option [ngValue]="7">Wills & Probate</option>
          <option [ngValue]="8">Personal Injury</option>
        </select>
        @if (searchTerm || statusFilter !== undefined || typeFilter !== undefined) {
          <button (click)="clearFilters()" class="text-xs text-blue-600 hover:text-blue-800 font-medium whitespace-nowrap">Clear filters</button>
        }
      </div>

      @if (store.loading()) {
        <div class="card-section">
          <div class="animate-pulse space-y-3">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="h-12 bg-slate-100 rounded"></div>
            }
          </div>
        </div>
      }

      @if (store.error()) {
        <div class="card-section border-red-200 bg-red-50">
          <p class="text-sm text-red-700">{{ store.error() }}</p>
          <button class="text-xs text-red-600 font-medium mt-2 hover:underline" (click)="onSearch()">Retry</button>
        </div>
      }

      @if (store.isEmpty()) {
        <div class="card-section text-center py-16">
          <svg class="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"/></svg>
          <h3 class="text-base font-semibold text-slate-700">No matters found</h3>
          <p class="text-sm text-slate-500 mt-2">Try adjusting your search or filters.</p>
        </div>
      }

      @if (!store.loading() && !store.isEmpty() && !store.error()) {
        <div class="card-section overflow-x-auto">
          <table class="table w-full">
            <thead>
              <tr>
                <th class="sortable-header" (click)="toggleSort('reference')">Reference</th>
                <th class="sortable-header" (click)="toggleSort('title')">Title</th>
                <th>Client</th>
                <th>Type</th>
                <th class="sortable-header" (click)="toggleSort('status')">Status</th>
                <th>Lead</th>
                <th class="sortable-header" (click)="toggleSort('opened')">Opened</th>
              </tr>
            </thead>
            <tbody>
              @for (matter of store.items(); track matter.id) {
                <tr class="cursor-pointer" [routerLink]="['/matters', matter.id]">
                  <td class="font-mono text-xs font-semibold text-slate-700">{{ matter.referenceNumber }}</td>
                  <td class="font-medium text-sm text-slate-900 max-w-[250px] truncate">{{ matter.title }}</td>
                  <td class="text-sm text-slate-600">{{ matter.clientName }}</td>
                  <td class="text-xs text-slate-500">{{ getMatterType(matter.matterType) }}</td>
                  <td><span class="text-[11px] px-2 py-0.5 rounded-full font-medium" [class]="getStatusClass(matter.status)">{{ getStatusLabel(matter.status) }}</span></td>
                  <td class="text-xs text-slate-500">{{ matter.leadConsultantName }}</td>
                  <td class="text-xs text-slate-400">{{ matter.openedDate }}</td>
                </tr>
              }
            </tbody>
          </table>
          <div class="flex items-center justify-between mt-4 pt-4 border-t border-slate-100">
            <span class="text-xs text-slate-500">Showing {{ store.items().length }} of {{ store.totalCount() }} matters</span>
          </div>
        </div>
      }
    </div>
  `
})
export class MatterListComponent implements OnInit {
  searchTerm = '';
  statusFilter: number | undefined;
  typeFilter: number | undefined;
  sortColumn = 'opened';
  sortDirection = 'desc';

  constructor(public store: MatterStore) {}

  ngOnInit(): void { this.store.loadMatters({ pageSize: 50 }); }

  onSearch(): void {
    this.store.loadMatters({ searchTerm: this.searchTerm, status: this.statusFilter, pageSize: 50 });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = undefined;
    this.typeFilter = undefined;
    this.onSearch();
  }

  toggleSort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
  }

  getStatusLabel(s: number): string { return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][s] ?? 'Unknown'; }
  getStatusClass(s: number): string { return ['status-info', 'status-active', 'status-warning', 'status-warning', 'status-warning', 'status-pending', 'status-closed', 'status-closed'][s] ?? ''; }
  getMatterType(t: number): string { return ['Conveyancing', 'Litigation', 'Family Law', 'Commercial', 'Employment', 'Criminal', 'Immigration', 'Wills & Probate', 'Personal Injury', 'Other'][t] ?? 'Other'; }
}
