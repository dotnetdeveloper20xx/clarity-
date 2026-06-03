import { Component, OnInit, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatterStore } from '../../../core/stores/matter.store';

@Component({
  selector: 'app-matter-detail',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="page-container">
      <div class="mb-4">
        <a routerLink="/matters" class="text-sm text-primary hover:underline">← Back to Matters</a>
      </div>

      @if (store.loading()) {
        <div class="card-section animate-pulse">
          <div class="h-6 bg-base-300 rounded w-1/3 mb-4"></div>
          <div class="h-4 bg-base-300 rounded w-1/2"></div>
        </div>
      }

      @if (store.selected(); as matter) {
        <div class="flex items-center justify-between mb-6">
          <div>
            <h1 class="page-title">{{ matter.title }}</h1>
            <p class="text-sm text-navy-500 mt-1">{{ matter.referenceNumber }} • {{ matter.clientName }}</p>
          </div>
          <span class="badge badge-lg" [class]="getStatusClass(matter.status)">{{ getStatusLabel(matter.status) }}</span>
        </div>

        <!-- Tabs -->
        <div role="tablist" class="tabs tabs-bordered mb-6">
          <a role="tab" class="tab tab-active">Overview</a>
          <a role="tab" class="tab">Documents</a>
          <a role="tab" class="tab">Time</a>
          <a role="tab" class="tab">Billing</a>
          <a role="tab" class="tab">Tasks</a>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="card-section">
            <h2 class="section-title mb-4">Matter Details</h2>
            <div class="space-y-3">
              <div><div class="data-label">Lead Consultant</div><div class="data-value">{{ matter.leadConsultantName }}</div></div>
              <div><div class="data-label">Fee Arrangement</div><div class="data-value">{{ ['Hourly', 'Fixed Fee', 'Hybrid'][matter.feeArrangement] }}</div></div>
              <div><div class="data-label">Estimated Value</div><div class="data-value">{{ matter.estimatedValue ? '£' + matter.estimatedValue.toLocaleString() : '—' }}</div></div>
              <div><div class="data-label">Opened</div><div class="data-value">{{ matter.openedDate }}</div></div>
              @if (matter.closedDate) {
                <div><div class="data-label">Closed</div><div class="data-value">{{ matter.closedDate }}</div></div>
              }
            </div>
          </div>

          <div class="card-section">
            <h2 class="section-title mb-4">Description</h2>
            <p class="text-sm text-navy-700">{{ matter.description || 'No description provided.' }}</p>
          </div>
        </div>
      }
    </div>
  `
})
export class MatterDetailComponent implements OnInit {
  id = input.required<string>();

  constructor(public store: MatterStore) {}

  ngOnInit(): void {
    this.store.loadMatter(this.id());
  }

  getStatusLabel(status: number): string {
    return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-info', 'badge-primary', 'badge-warning', 'badge-warning', 'badge-warning', 'badge-accent', 'badge-ghost', 'badge-ghost'][status] ?? '';
  }
}
