import { Component, OnInit, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ClientStore } from '../../../core/stores/client.store';

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
            <p class="text-sm text-navy-500 mt-1">{{ client.referenceNumber }}</p>
          </div>
          <span class="badge" [class]="getStatusClass(client.status)">{{ getStatusLabel(client.status) }}</span>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-section">
            <h2 class="section-title mb-4">Contact Details</h2>
            <div class="space-y-3">
              <div>
                <div class="data-label">Email</div>
                <div class="data-value">{{ client.email || '—' }}</div>
              </div>
              <div>
                <div class="data-label">Phone</div>
                <div class="data-value">{{ client.phone || '—' }}</div>
              </div>
              <div>
                <div class="data-label">Address</div>
                <div class="data-value">{{ client.addressLine1 || '—' }}</div>
                @if (client.city || client.postCode) {
                  <div class="data-value">{{ client.city }} {{ client.postCode }}</div>
                }
              </div>
            </div>
          </div>

          <div class="card-section">
            <h2 class="section-title mb-4">Client Information</h2>
            <div class="space-y-3">
              <div>
                <div class="data-label">Type</div>
                <div class="data-value">{{ client.clientType === 0 ? 'Individual' : 'Organisation' }}</div>
              </div>
              @if (client.companyNumber) {
                <div>
                  <div class="data-label">Company Number</div>
                  <div class="data-value">{{ client.companyNumber }}</div>
                </div>
              }
              <div>
                <div class="data-label">Created</div>
                <div class="data-value">{{ client.createdAt | date }}</div>
              </div>
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
    </div>
  `
})
export class ClientDetailComponent implements OnInit {
  id = input.required<string>();

  constructor(public store: ClientStore) {}

  ngOnInit(): void {
    this.store.loadClient(this.id());
  }

  getStatusLabel(status: number): string {
    return ['Pending', 'Active', 'On Hold', 'Archived'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-warning', 'badge-success', 'badge-error', 'badge-ghost'][status] ?? '';
  }
}
