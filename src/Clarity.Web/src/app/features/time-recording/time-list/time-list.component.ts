import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TimeApiService } from '../../../core/services/time-api.service';
import { TimeEntryDto, PaginatedList } from '../../../core/models/api.models';

@Component({
  selector: 'app-time-list',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <h1 class="page-title">Time Recording</h1>
        <button class="btn btn-primary btn-sm" (click)="showForm = true">+ Record Time</button>
      </div>

      @if (showForm) {
        <div class="card-section mb-6">
          <h2 class="section-title mb-4">New Time Entry</h2>
          <form (ngSubmit)="onRecordTime()">
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div class="form-control">
                <label class="label"><span class="label-text">Date</span></label>
                <input type="date" [(ngModel)]="newEntry.date" name="date" class="input input-bordered input-sm" required />
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text">Duration (minutes)</span></label>
                <input type="number" [(ngModel)]="newEntry.durationMinutes" name="duration" class="input input-bordered input-sm" min="1" required />
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text">Billable</span></label>
                <input type="checkbox" [(ngModel)]="newEntry.isBillable" name="billable" class="toggle toggle-primary mt-2" />
              </div>
            </div>
            <div class="form-control mt-4">
              <label class="label"><span class="label-text">Description</span></label>
              <textarea [(ngModel)]="newEntry.description" name="description" class="textarea textarea-bordered" rows="2" required></textarea>
            </div>
            <div class="flex justify-end gap-2 mt-4">
              <button type="button" class="btn btn-ghost btn-sm" (click)="showForm = false">Cancel</button>
              <button type="submit" class="btn btn-primary btn-sm">Save Entry</button>
            </div>
          </form>
        </div>
      }

      @if (loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading...</div>
      }

      @if (!loading() && items().length > 0) {
        <div class="card-section overflow-x-auto">
          <table class="table table-sm">
            <thead>
              <tr class="text-navy-600">
                <th>Date</th>
                <th>Matter</th>
                <th>Description</th>
                <th>Duration</th>
                <th>Billable</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              @for (entry of items(); track entry.id) {
                <tr class="hover:bg-base-200">
                  <td class="text-xs">{{ entry.date }}</td>
                  <td class="font-mono text-xs">{{ entry.matterReference }}</td>
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

      @if (!loading() && items().length === 0) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">⏱️</div>
          <h3 class="text-lg font-medium text-navy-700">No time entries yet</h3>
          <p class="text-sm text-navy-500 mt-2">Record your first time entry to track billable work.</p>
        </div>
      }
    </div>
  `
})
export class TimeListComponent implements OnInit {
  items = signal<TimeEntryDto[]>([]);
  loading = signal(false);
  showForm = false;
  newEntry = { date: '', durationMinutes: 60, description: '', isBillable: true, matterId: '' };

  constructor(private api: TimeApiService) {}

  ngOnInit(): void {
    this.loadEntries();
  }

  loadEntries(): void {
    this.loading.set(true);
    this.api.getTimeEntries({}).subscribe({
      next: (result) => { this.items.set(result.items); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onRecordTime(): void {
    this.api.recordTime(this.newEntry).subscribe({
      next: () => { this.showForm = false; this.loadEntries(); },
      error: (err) => console.error('Failed to record time', err)
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
