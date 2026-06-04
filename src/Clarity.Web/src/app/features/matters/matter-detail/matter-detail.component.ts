import { Component, OnInit, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatterStore } from '../../../core/stores/matter.store';
import { TimeApiService } from '../../../core/services/time-api.service';
import { TimeEntryDto } from '../../../core/models/api.models';

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
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'overview'" (click)="activeTab.set('overview')">Overview</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'documents'" (click)="activeTab.set('documents')">Documents</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'time'" (click)="switchToTime()">Time</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'billing'" (click)="activeTab.set('billing')">Billing</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'tasks'" (click)="activeTab.set('tasks')">Tasks</a>
        </div>

        <!-- Overview Tab -->
        @if (activeTab() === 'overview') {
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

        <!-- Documents Tab -->
        @if (activeTab() === 'documents') {
          <div class="card-section text-center py-12">
            <div class="text-4xl mb-4">📄</div>
            <h3 class="text-lg font-medium text-navy-700">No documents yet</h3>
            <p class="text-sm text-navy-500 mt-2">Upload documents to this matter to see them here.</p>
          </div>
        }

        <!-- Time Tab -->
        @if (activeTab() === 'time') {
          @if (timeLoading()) {
            <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading time entries...</div>
          } @else if (timeEntries().length === 0) {
            <div class="card-section text-center py-12">
              <div class="text-4xl mb-4">⏱️</div>
              <h3 class="text-lg font-medium text-navy-700">No time recorded</h3>
              <p class="text-sm text-navy-500 mt-2">Time entries for this matter will appear here.</p>
            </div>
          } @else {
            <div class="card-section overflow-x-auto">
              <table class="table table-sm">
                <thead>
                  <tr class="text-navy-600">
                    <th>Date</th>
                    <th>User</th>
                    <th>Description</th>
                    <th>Duration</th>
                    <th>Billable</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  @for (entry of timeEntries(); track entry.id) {
                    <tr class="hover:bg-base-200">
                      <td class="text-xs">{{ entry.date }}</td>
                      <td>{{ entry.userName }}</td>
                      <td class="max-w-xs truncate">{{ entry.description }}</td>
                      <td>{{ formatDuration(entry.durationMinutes) }}</td>
                      <td>
                        @if (entry.isBillable) { <span class="badge badge-success badge-xs">Yes</span> }
                        @else { <span class="badge badge-ghost badge-xs">No</span> }
                      </td>
                      <td><span class="badge badge-sm" [class]="getTimeStatusClass(entry.status)">{{ getTimeStatusLabel(entry.status) }}</span></td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        }

        <!-- Billing Tab -->
        @if (activeTab() === 'billing') {
          <div class="card-section text-center py-12">
            <div class="text-4xl mb-4">💰</div>
            <h3 class="text-lg font-medium text-navy-700">No invoices yet</h3>
            <p class="text-sm text-navy-500 mt-2">Invoices for this matter will appear here once generated.</p>
          </div>
        }

        <!-- Tasks Tab -->
        @if (activeTab() === 'tasks') {
          <div class="card-section text-center py-12">
            <div class="text-4xl mb-4">✅</div>
            <h3 class="text-lg font-medium text-navy-700">No tasks yet</h3>
            <p class="text-sm text-navy-500 mt-2">Tasks assigned to this matter will appear here.</p>
          </div>
        }
      }
    </div>
  `
})
export class MatterDetailComponent implements OnInit {
  id = input.required<string>();
  activeTab = signal('overview');
  timeEntries = signal<TimeEntryDto[]>([]);
  timeLoading = signal(false);

  constructor(public store: MatterStore, private timeApi: TimeApiService) {}

  ngOnInit(): void {
    this.store.loadMatter(this.id());
  }

  switchToTime(): void {
    this.activeTab.set('time');
    if (this.timeEntries().length === 0) {
      this.timeLoading.set(true);
      this.timeApi.getTimeEntries({ matterId: this.id() }).subscribe({
        next: (result) => { this.timeEntries.set(result.items); this.timeLoading.set(false); },
        error: () => this.timeLoading.set(false)
      });
    }
  }

  formatDuration(mins: number): string {
    const h = Math.floor(mins / 60);
    const m = mins % 60;
    return h > 0 ? `${h}h ${m}m` : `${m}m`;
  }

  getStatusLabel(status: number): string {
    return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][status] ?? 'Unknown';
  }

  getStatusClass(status: number): string {
    return ['badge-info', 'badge-primary', 'badge-warning', 'badge-warning', 'badge-warning', 'badge-accent', 'badge-ghost', 'badge-ghost'][status] ?? '';
  }

  getTimeStatusLabel(status: number): string {
    return ['Draft', 'Submitted', 'Approved', 'Rejected', 'Billed', 'Written Off'][status] ?? 'Unknown';
  }

  getTimeStatusClass(status: number): string {
    return ['badge-ghost', 'badge-info', 'badge-success', 'badge-error', 'badge-primary', 'badge-warning'][status] ?? '';
  }
}
