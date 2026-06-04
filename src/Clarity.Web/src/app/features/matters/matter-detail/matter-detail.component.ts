import { Component, OnInit, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatterStore } from '../../../core/stores/matter.store';
import { TimeApiService } from '../../../core/services/time-api.service';
import { TimeEntryDto, InvoiceDto, PaginatedList } from '../../../core/models/api.models';
import { environment } from '@env/environment';

interface TaskDto { id: string; title: string; description: string; status: number; priority: number; dueDate: string; completedAt: string | null; }
interface NoteDto { id: string; content: string; isClientVisible: boolean; createdAt: string; }

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
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'notes'" (click)="switchToNotes()">Notes</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'tasks'" (click)="switchToTasks()">Tasks</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'time'" (click)="switchToTime()">Time</a>
          <a role="tab" class="tab" [class.tab-active]="activeTab() === 'billing'" (click)="switchToBilling()">Billing</a>
        </div>

        <!-- Overview Tab -->
        @if (activeTab() === 'overview') {
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div class="card-section">
              <h2 class="section-title mb-4">Matter Details</h2>
              <div class="space-y-3">
                <div><div class="data-label">Lead Consultant</div><div class="data-value">{{ matter.leadConsultantName }}</div></div>
                <div><div class="data-label">Matter Type</div><div class="data-value">{{ getMatterType(matter.matterType) }}</div></div>
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
              <p class="text-sm text-navy-700 whitespace-pre-line">{{ matter.description || 'No description provided.' }}</p>
            </div>
          </div>
        }

        <!-- Notes Tab -->
        @if (activeTab() === 'notes') {
          @if (notesLoading()) {
            <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading notes...</div>
          } @else if (notes().length === 0) {
            <div class="card-section text-center py-12">
              <div class="text-4xl mb-4">📝</div>
              <h3 class="text-lg font-medium text-navy-700">No notes yet</h3>
              <p class="text-sm text-navy-500 mt-2">Notes and observations for this matter will appear here.</p>
            </div>
          } @else {
            <div class="space-y-3">
              @for (note of notes(); track note.id) {
                <div class="card-section">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-xs text-navy-400">{{ note.createdAt.substring(0, 10) }}</span>
                    @if (note.isClientVisible) {
                      <span class="badge badge-xs badge-info">Client Visible</span>
                    } @else {
                      <span class="badge badge-xs badge-ghost">Internal</span>
                    }
                  </div>
                  <p class="text-sm text-navy-700">{{ note.content }}</p>
                </div>
              }
            </div>
          }
        }

        <!-- Tasks Tab -->
        @if (activeTab() === 'tasks') {
          @if (tasksLoading()) {
            <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading tasks...</div>
          } @else if (tasks().length === 0) {
            <div class="card-section text-center py-12">
              <div class="text-4xl mb-4">✅</div>
              <h3 class="text-lg font-medium text-navy-700">No tasks yet</h3>
              <p class="text-sm text-navy-500 mt-2">Tasks assigned to this matter will appear here.</p>
            </div>
          } @else {
            <div class="card-section overflow-x-auto">
              <table class="table table-sm">
                <thead>
                  <tr class="text-navy-600"><th>Task</th><th>Priority</th><th>Status</th><th>Due Date</th></tr>
                </thead>
                <tbody>
                  @for (task of tasks(); track task.id) {
                    <tr class="hover:bg-base-200">
                      <td class="font-medium">{{ task.title }}</td>
                      <td><span class="badge badge-xs" [class]="getPriorityClass(task.priority)">{{ getPriorityLabel(task.priority) }}</span></td>
                      <td><span class="badge badge-sm" [class]="getTaskStatusClass(task.status)">{{ getTaskStatusLabel(task.status) }}</span></td>
                      <td class="text-xs text-navy-400">{{ task.dueDate }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
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
                  <tr class="text-navy-600"><th>Date</th><th>User</th><th>Description</th><th>Duration</th><th>Billable</th><th>Status</th></tr>
                </thead>
                <tbody>
                  @for (entry of timeEntries(); track entry.id) {
                    <tr class="hover:bg-base-200">
                      <td class="text-xs">{{ entry.date }}</td>
                      <td class="text-sm">{{ entry.userName }}</td>
                      <td class="max-w-xs truncate text-sm">{{ entry.description }}</td>
                      <td>{{ formatDuration(entry.durationMinutes) }}</td>
                      <td>@if (entry.isBillable) { <span class="badge badge-success badge-xs">Yes</span> } @else { <span class="badge badge-ghost badge-xs">No</span> }</td>
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
          @if (invoicesLoading()) {
            <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading invoices...</div>
          } @else if (invoices().length === 0) {
            <div class="card-section text-center py-12">
              <div class="text-4xl mb-4">💰</div>
              <h3 class="text-lg font-medium text-navy-700">No invoices yet</h3>
              <p class="text-sm text-navy-500 mt-2">Invoices for this matter will appear here once generated.</p>
            </div>
          } @else {
            <div class="card-section overflow-x-auto">
              <table class="table table-sm">
                <thead>
                  <tr class="text-navy-600"><th>Invoice #</th><th>Status</th><th>Total</th><th>Paid</th><th>Outstanding</th></tr>
                </thead>
                <tbody>
                  @for (inv of invoices(); track inv.id) {
                    <tr class="hover:bg-base-200">
                      <td class="font-mono text-xs font-medium">{{ inv.invoiceNumber }}</td>
                      <td><span class="badge badge-sm" [class]="getInvoiceStatusClass(inv.status)">{{ getInvoiceStatusLabel(inv.status) }}</span></td>
                      <td class="font-medium">£{{ inv.totalAmount.toLocaleString() }}</td>
                      <td class="text-green-600">£{{ inv.paidAmount.toLocaleString() }}</td>
                      <td class="text-amber-600 font-medium">£{{ (inv.totalAmount - inv.paidAmount).toLocaleString() }}</td>
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
export class MatterDetailComponent implements OnInit {
  id = input.required<string>();
  activeTab = signal('overview');
  timeEntries = signal<TimeEntryDto[]>([]);
  timeLoading = signal(false);
  invoices = signal<InvoiceDto[]>([]);
  invoicesLoading = signal(false);
  tasks = signal<TaskDto[]>([]);
  tasksLoading = signal(false);
  notes = signal<NoteDto[]>([]);
  notesLoading = signal(false);

  constructor(public store: MatterStore, private timeApi: TimeApiService, private http: HttpClient) {}

  ngOnInit(): void { this.store.loadMatter(this.id()); }

  switchToTime(): void {
    this.activeTab.set('time');
    if (this.timeEntries().length === 0) {
      this.timeLoading.set(true);
      this.timeApi.getTimeEntries({ matterId: this.id() }).subscribe({
        next: (r) => { this.timeEntries.set(r.items); this.timeLoading.set(false); },
        error: () => this.timeLoading.set(false)
      });
    }
  }

  switchToBilling(): void {
    this.activeTab.set('billing');
    if (this.invoices().length === 0) {
      this.invoicesLoading.set(true);
      this.http.get<PaginatedList<InvoiceDto>>(`${environment.apiUrl}/invoices?matterId=${this.id()}`).subscribe({
        next: (r) => { this.invoices.set(r.items); this.invoicesLoading.set(false); },
        error: () => this.invoicesLoading.set(false)
      });
    }
  }

  switchToTasks(): void {
    this.activeTab.set('tasks');
    if (this.tasks().length === 0) {
      this.tasksLoading.set(true);
      this.http.get<any[]>(`${environment.apiUrl}/matters/${this.id()}/tasks`).subscribe({
        next: (r) => { this.tasks.set(r); this.tasksLoading.set(false); },
        error: () => this.tasksLoading.set(false)
      });
    }
  }

  switchToNotes(): void {
    this.activeTab.set('notes');
    if (this.notes().length === 0) {
      this.notesLoading.set(true);
      this.http.get<any[]>(`${environment.apiUrl}/matters/${this.id()}/notes`).subscribe({
        next: (r) => { this.notes.set(r); this.notesLoading.set(false); },
        error: () => this.notesLoading.set(false)
      });
    }
  }

  formatDuration(mins: number): string { const h = Math.floor(mins / 60); const m = mins % 60; return h > 0 ? `${h}h ${m}m` : `${m}m`; }
  getStatusLabel(s: number): string { return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting 3rd Party', 'Billing Review', 'Closed', 'Archived'][s] ?? 'Unknown'; }
  getStatusClass(s: number): string { return ['badge-info', 'badge-primary', 'badge-warning', 'badge-warning', 'badge-warning', 'badge-accent', 'badge-ghost', 'badge-ghost'][s] ?? ''; }
  getMatterType(t: number): string { return ['Conveyancing', 'Litigation', 'Family Law', 'Commercial', 'Employment', 'Criminal', 'Immigration', 'Wills & Probate', 'Personal Injury', 'Other'][t] ?? 'Other'; }
  getTimeStatusLabel(s: number): string { return ['Draft', 'Submitted', 'Approved', 'Rejected', 'Billed', 'Written Off'][s] ?? 'Unknown'; }
  getTimeStatusClass(s: number): string { return ['badge-ghost', 'badge-info', 'badge-success', 'badge-error', 'badge-primary', 'badge-warning'][s] ?? ''; }
  getInvoiceStatusLabel(s: number): string { return ['Draft', 'Issued', 'Partially Paid', 'Paid', 'Cancelled', 'Written Off'][s] ?? 'Unknown'; }
  getInvoiceStatusClass(s: number): string { return ['badge-ghost', 'badge-info', 'badge-warning', 'badge-success', 'badge-error', 'badge-error'][s] ?? ''; }
  getTaskStatusLabel(s: number): string { return ['To Do', 'In Progress', 'Blocked', 'Complete', 'Cancelled'][s] ?? 'Unknown'; }
  getTaskStatusClass(s: number): string { return ['badge-ghost', 'badge-info', 'badge-warning', 'badge-success', 'badge-error'][s] ?? ''; }
  getPriorityLabel(p: number): string { return ['Low', 'Medium', 'High', 'Urgent'][p] ?? 'Unknown'; }
  getPriorityClass(p: number): string { return ['badge-ghost', 'badge-info', 'badge-warning', 'badge-error'][p] ?? ''; }
}
