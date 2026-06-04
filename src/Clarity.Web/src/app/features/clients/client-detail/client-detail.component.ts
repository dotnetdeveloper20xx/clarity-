import { Component, OnInit, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ClientStore } from '../../../core/stores/client.store';
import { MatterDto, PaginatedList } from '../../../core/models/api.models';
import { environment } from '@env/environment';

@Component({
  selector: 'app-client-detail',
  standalone: true,
  imports: [RouterLink, DatePipe],
  template: `
    <div class="page-container">
      <div class="mb-4">
        <a routerLink="/clients" class="text-sm text-primary hover:underline">← Back to Clients</a>
      </div>

      @if (store.loading()) {
        <div class="card-section animate-pulse">
          <div class="h-6 bg-base-300 rounded w-1/3 mb-4"></div>
          <div class="h-4 bg-base-300 rounded w-1/2 mb-2"></div>
          <div class="h-4 bg-base-300 rounded w-1/4"></div>
        </div>
      }

      @if (store.error()) {
        <div class="alert alert-error">{{ store.error() }}</div>
      }

      @if (store.selected(); as client) {
        <div class="flex items-center justify-between mb-6">
          <div>
            <h1 class="page-title">{{ client.name }}</h1>
            <p class="text-sm text-navy-500 mt-1">{{ client.referenceNumber }} • {{ client.clientType === 0 ? 'Individual' : 'Organisation' }}</p>
          </div>
          <span class="badge badge-lg" [class]="getStatusClass(client.status)">{{ getStatusLabel(client.status) }}</span>
        </div>

        <!-- Tabs -->
        <div role="tablist" class="tabs tabs-bordered mb-6">
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'details'" (click)="activeTab.set('details')">Details</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'matters'" (click)="loadMatters()">Matters</a>
        </div>

        <!-- Details Tab -->
        @if (activeTab() === 'details') {
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div class="card-section">
              <h2 class="section-title mb-4">Contact Details</h2>
              <div class="space-y-3">
                <div><div class="data-label">Email</div><div class="data-value">{{ client.email || '—' }}</div></div>
                <div><div class="data-label">Phone</div><div class="data-value">{{ client.phone || '—' }}</div></div>
                <div>
                  <div class="data-label">Address</div>
                  <div class="data-value">{{ client.addressLine1 || '—' }}</div>
                  @if (client.city || client.postCode) {
                    <div class="data-value">{{ client.city }} {{ client.postCode }}</div>
                  }
                </div>
                <div><div class="data-label">Country</div><div class="data-value">{{ client.country || '—' }}</div></div>
              </div>
            </div>

            <div class="card-section">
              <h2 class="section-title mb-4">Client Information</h2>
              <div class="space-y-3">
                <div><div class="data-label">Type</div><div class="data-value">{{ client.clientType === 0 ? 'Individual' : 'Organisation' }}</div></div>
                @if (client.companyNumber) {
                  <div><div class="data-label">Company Number</div><div class="data-value">{{ client.companyNumber }}</div></div>
                }
                <div><div class="data-label">Status</div><div class="data-value">{{ getStatusLabel(client.status) }}</div></div>
                <div><div class="data-label">Client Since</div><div class="data-value">{{ client.createdAt | date:'mediumDate' }}</div></div>
              </div>
            </div>
          </div>

          @if (client.notes) {
            <div class="card-section mt-6">
              <h2 class="section-title mb-2">Notes</h2>
              <p class="text-sm text-navy-700">{{ client.notes }}</p>
            </div>
          }
        }

        <!-- Matters Tab -->
        @if (activeTab() === 'matters') {
          @if (mattersLoading()) {
            <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading matters...</div>
          } @else if (matters().length === 0) {
            <div class="card-section text-center py-12">
              <div class="text-4xl mb-4">📁</div>
              <h3 class="text-lg font-medium text-navy-700">No matters for this client</h3>
              <p class="text-sm text-navy-500 mt-2">Create a new matter to begin legal work for this client.</p>
            </div>
          } @else {
            <div class="card-section overflow-x-auto">
              <table class="table table-sm">
                <thead>
                  <tr class="text-navy-600">
                    <th>Reference</th>
                    <th>Title</th>
                    <th>Type</th>
                    <th>Status</th>
                    <th>Opened</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  @for (m of matters(); track m.id) {
                    <tr class="hover:bg-base-200">
                      <td class="font-mono text-xs">{{ m.referenceNumber }}</td>
                      <td class="font-medium">{{ m.title }}</td>
                      <td class="text-xs text-navy-500">{{ getMatterType(m.matterType) }}</td>
                      <td><span class="badge badge-sm" [class]="getMatterStatusClass(m.status)">{{ getMatterStatusLabel(m.status) }}</span></td>
                      <td class="text-xs text-navy-400">{{ m.openedDate }}</td>
                      <td><a [routerLink]="['/matters', m.id]" class="btn btn-ghost btn-xs">View</a></td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        }
      }
    </div>
  `
})
export class ClientDetailComponent implements OnInit {
  id = input.required<string>();
  activeTab = signal('details');
  matters = signal<MatterDto[]>([]);
  mattersLoading = signal(false);

  constructor(public store: ClientStore, private http: HttpClient) {}

  ngOnInit(): void {
    this.store.loadClient(this.id());
  }

  loadMatters(): void {
    this.activeTab.set('matters');
    if (this.matters().length === 0) {
      this.mattersLoading.set(true);
      this.http.get<PaginatedList<MatterDto>>(`${environment.apiUrl}/matters?clientId=${this.id()}`).subscribe({
        next: (result) => { this.matters.set(result.items); this.mattersLoading.set(false); },
        error: () => this.mattersLoading.set(false)
      });
    }
  }

  getStatusLabel(status: number): string {
    return ['Pending', 'Active', 'On Hold', 'Archived'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-warning', 'badge-success', 'badge-error', 'badge-ghost'][status] ?? '';
  }

  getMatterStatusLabel(status: number): string {
    return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][status] ?? 'Unknown';
  }

  getMatterStatusClass(status: number): string {
    return ['badge-info', 'badge-primary', 'badge-warning', 'badge-warning', 'badge-warning', 'badge-accent', 'badge-ghost', 'badge-ghost'][status] ?? '';
  }

  getMatterType(type: number): string {
    return ['Conveyancing', 'Litigation', 'Family Law', 'Commercial', 'Employment', 'Criminal', 'Immigration', 'Wills & Probate', 'Personal Injury', 'Other'][type] ?? 'Other';
  }
}
