import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { TimeApiService } from '../../../core/services/time-api.service';
import { TimeEntryDto, PaginatedList, MatterDto } from '../../../core/models/api.models';
import { environment } from '@env/environment';

@Component({
  selector: 'app-time-list',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <h1 class="page-title">Time Recording</h1>
        <button class="btn btn-primary btn-sm" (click)="toggleForm()">+ Record Time</button>
      </div>

      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div class="card-section">
          <div class="data-label">Total Entries</div>
          <div class="text-2xl font-bold text-navy-900 mt-1">{{ totalCount() }}</div>
        </div>
        <div class="card-section">
          <div class="data-label">Draft</div>
          <div class="text-2xl font-bold text-navy-500 mt-1">{{ draftCount() }}</div>
        </div>
        <div class="card-section">
          <div class="data-label">Awaiting Approval</div>
          <div class="text-2xl font-bold text-blue-600 mt-1">{{ submittedCount() }}</div>
        </div>
        <div class="card-section">
          <div class="data-label">Approved</div>
          <div class="text-2xl font-bold text-green-600 mt-1">{{ approvedCount() }}</div>
        </div>
      </div>

      @if (showForm) {
        <div class="card-section mb-6">
          <h2 class="section-title mb-4">New Time Entry</h2>
          <form (ngSubmit)="onRecordTime()">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <div class="form-control">
                <label class="label"><span class="label-text">Matter *</span></label>
                <select [(ngModel)]="newEntry.matterId" name="matterId" class="select select-bordered select-sm" required>
                  <option value="">Select a matter...</option>
                  @for (m of matterOptions(); track m.id) {
                    <option [value]="m.id">{{ m.referenceNumber }} - {{ m.title }}</option>
                  }
                </select>
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text">Date *</span></label>
                <input type="date" [(ngModel)]="newEntry.date" name="date" class="input input-bordered input-sm" required />
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text">Duration (minutes) *</span></label>
                <input type="number" [(ngModel)]="newEntry.durationMinutes" name="duration" class="input input-bordered input-sm" min="1" max="720" required />
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text">Billable</span></label>
                <input type="checkbox" [(ngModel)]="newEntry.isBillable" name="billable" class="toggle toggle-primary mt-2" />
              </div>
            </div>
            <div class="form-control mt-4">
              <label class="label"><span class="label-text">Description *</span></label>
              <textarea [(ngModel)]="newEntry.description" name="description" class="textarea textarea-bordered" rows="2" placeholder="Describe the work performed..." required></textarea>
            </div>
            @if (formError()) {
              <div class="alert alert-error mt-3 text-sm">{{ formError() }}</div>
            }
            <div class="flex justify-end gap-2 mt-4">
              <button type="button" class="btn btn-ghost btn-sm" (click)="showForm = false">Cancel</button>
              <button type="submit" class="btn btn-primary btn-sm" [disabled]="saving()">
                @if (saving()) { <span class="loading loading-spinner loading-xs"></span> }
                Save Entry
              </button>
            </div>
          </form>
        </div>
      }

      @if (loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading time entries...</div>
      }

      @if (!loading() && items().length > 0) {
        <div class="card-section overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Date</th>
                <th>Matter</th>
                <th>User</th>
                <th>Description</th>
                <th>Duration</th>
                <th>Billable</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              @for (entry of items(); track entry.id) {
                <tr class="hover:bg-base-200">
                  <td class="text-xs whitespace-nowrap">{{ entry.date }}</td>
                  <td class="font-mono text-xs whitespace-nowrap">{{ entry.matterReference }}</td>
                  <td class="text-sm whitespace-nowrap">{{ entry.userName }}</td>
                  <td class="max-w-xs truncate text-sm">{{ entry.description }}</td>
                  <td class="whitespace-nowrap">{{ formatDuration(entry.durationMinutes) }}</td>
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

      @if (!loading() && items().length === 0) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">⏱️</div>
          <h3 class="text-lg font-medium text-navy-700">No time entries yet</h3>
          <p class="text-sm text-navy-500 mt-2">Record your first time entry to track billable work.</p>
          <button class="btn btn-primary btn-sm mt-4" (click)="toggleForm()">+ Record Time</button>
        </div>
      }
    </div>
  `
})
export class TimeListComponent implements OnInit {
  items = signal<TimeEntryDto[]>([]);
  matterOptions = signal<MatterDto[]>([]);
  loading = signal(false);
  saving = signal(false);
  formError = signal<string | null>(null);
  showForm = false;
  newEntry = { date: '', durationMinutes: 60, description: '', isBillable: true, matterId: '' };

  totalCount = computed(() => this.items().length);
  draftCount = computed(() => this.items().filter(e => e.status === 0).length);
  submittedCount = computed(() => this.items().filter(e => e.status === 1).length);
  approvedCount = computed(() => this.items().filter(e => e.status === 2).length);

  constructor(private api: TimeApiService, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadEntries();
    this.loadMatters();
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (this.showForm && !this.newEntry.date) {
      this.newEntry.date = new Date().toISOString().split('T')[0];
    }
  }

  loadEntries(): void {
    this.loading.set(true);
    this.api.getTimeEntries({ pageSize: 50 }).subscribe({
      next: (result) => { this.items.set(result.items); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  loadMatters(): void {
    this.http.get<PaginatedList<MatterDto>>(`${environment.apiUrl}/matters?pageSize=50`).subscribe({
      next: (result) => this.matterOptions.set(result.items.filter(m => m.status !== 6 && m.status !== 7)) // Exclude closed/archived
    });
  }

  onRecordTime(): void {
    if (!this.newEntry.matterId) {
      this.formError.set('Please select a matter.');
      return;
    }
    this.formError.set(null);
    this.saving.set(true);
    this.api.recordTime(this.newEntry).subscribe({
      next: () => {
        this.showForm = false;
        this.saving.set(false);
        this.newEntry = { date: '', durationMinutes: 60, description: '', isBillable: true, matterId: '' };
        this.loadEntries();
      },
      error: (err) => {
        this.formError.set(err?.error?.title || err?.error?.errors?.[0] || 'Failed to record time.');
        this.saving.set(false);
      }
    });
  }

  formatDuration(mins: number): string {
    const h = Math.floor(mins / 60);
    const m = mins % 60;
    return h > 0 ? `${h}h ${m}m` : `${m}m`;
  }

  getTimeStatusLabel(status: number): string {
    return ['Draft', 'Submitted', 'Approved', 'Rejected', 'Billed', 'Written Off'][status] ?? 'Unknown';
  }

  getTimeStatusClass(status: number): string {
    return ['badge-ghost', 'badge-info', 'badge-success', 'badge-error', 'badge-primary', 'badge-warning'][status] ?? '';
  }
}
