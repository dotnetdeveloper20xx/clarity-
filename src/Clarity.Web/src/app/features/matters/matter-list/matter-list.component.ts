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
        <h1 class="page-title">Matters</h1>
      </div>

      <div class="card-section mb-6">
        <input type="text" [(ngModel)]="searchTerm" (ngModelChange)="onSearch()" placeholder="Search matters..." class="input input-bordered input-sm w-full max-w-md" />
      </div>

      @if (store.loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading matters...</div>
      }

      @if (store.error()) {
        <div class="alert alert-error">{{ store.error() }}</div>
      }

      @if (store.isEmpty()) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">📁</div>
          <h3 class="text-lg font-medium text-navy-700">No matters found</h3>
          <p class="text-sm text-navy-500 mt-2">Matters will appear here once created.</p>
        </div>
      }

      @if (!store.loading() && !store.isEmpty()) {
        <div class="card-section overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Reference</th>
                <th>Title</th>
                <th>Client</th>
                <th>Status</th>
                <th>Lead</th>
                <th>Opened</th>
              </tr>
            </thead>
            <tbody>
              @for (matter of store.items(); track matter.id) {
                <tr class="hover:bg-base-200 cursor-pointer" [routerLink]="['/matters', matter.id]">
                  <td class="font-mono text-xs">{{ matter.referenceNumber }}</td>
                  <td class="font-medium">{{ matter.title }}</td>
                  <td class="text-navy-500">{{ matter.clientName }}</td>
                  <td><span class="badge badge-sm" [class]="getMatterStatusClass(matter.status)">{{ getMatterStatusLabel(matter.status) }}</span></td>
                  <td class="text-navy-500 text-xs">{{ matter.leadConsultantName }}</td>
                  <td class="text-navy-400 text-xs">{{ matter.openedDate }}</td>
                </tr>
              }
            </tbody>
          </table>
          <div class="flex items-center justify-between mt-4 text-sm text-navy-500">
            <span>{{ store.totalCount() }} matters total</span>
          </div>
        </div>
      }
    </div>
  `
})
export class MatterListComponent implements OnInit {
  searchTerm = '';

  constructor(public store: MatterStore) {}

  ngOnInit(): void {
    this.store.loadMatters();
  }

  onSearch(): void {
    this.store.loadMatters({ searchTerm: this.searchTerm });
  }

  getMatterStatusLabel(status: number): string {
    return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][status] ?? 'Unknown';
  }

  getMatterStatusClass(status: number): string {
    return ['badge-info', 'badge-primary', 'badge-warning', 'badge-warning', 'badge-warning', 'badge-accent', 'badge-ghost', 'badge-ghost'][status] ?? '';
  }
}
