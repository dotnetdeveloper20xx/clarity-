import { Component, OnInit, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatterStore } from '../../../core/stores/matter.store';
import { TimeApiService } from '../../../core/services/time-api.service';
import { TimeEntryDto, InvoiceDto, PaginatedList } from '../../../core/models/api.models';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../shared/components/toast/toast.component';
import { environment } from '@env/environment';

interface TaskDto { id: string; title: string; description: string; status: number; priority: number; dueDate: string; completedAt: string | null; }
interface NoteDto { id: string; content: string; isClientVisible: boolean; createdAt: string; }
interface DocumentDto { id: string; fileName: string; contentType: string; fileSizeBytes: number; version: number; category: string; createdAt: string; }

@Component({
  selector: 'app-matter-detail',
  standalone: true,
  imports: [RouterLink, FormsModule, ModalComponent, ConfirmDialogComponent],
  template: `
    <div class="page-container">
      <div class="mb-6">
        <a routerLink="/matters" class="text-sm text-blue-600 hover:text-blue-800 font-medium">← Back to Matters</a>
      </div>

      @if (store.loading()) {
        <div class="card-section animate-pulse"><div class="h-7 bg-slate-200 rounded w-1/3 mb-4"></div><div class="h-4 bg-slate-200 rounded w-1/2"></div></div>
      }

      @if (store.selected(); as matter) {
        <!-- Header -->
        <div class="flex items-start justify-between mb-8">
          <div>
            <h1 class="page-title">{{ matter.title }}</h1>
            <p class="text-sm text-slate-500 mt-1">{{ matter.referenceNumber }} • {{ matter.clientName }} • {{ getMatterType(matter.matterType) }}</p>
          </div>
          <div class="flex items-center gap-2">
            <span class="px-3 py-1.5 text-xs font-semibold rounded-full" [class]="getStatusClass(matter.status)">{{ getStatusLabel(matter.status) }}</span>
          </div>
        </div>

        <!-- Tabs -->
        <div class="border-b border-slate-200 mb-6">
          <nav class="flex gap-6">
            @for (tab of tabs; track tab.key) {
              <button (click)="switchTab(tab.key)" class="pb-3 px-1 text-sm font-medium border-b-2 transition-colors"
                [class]="activeTab() === tab.key ? 'border-blue-600 text-blue-600' : 'border-transparent text-slate-500 hover:text-slate-700'">
                {{ tab.label }}
                @if (tab.key === 'tasks' && tasks().length > 0) { <span class="ml-1 text-[10px] bg-slate-200 text-slate-600 px-1.5 py-0.5 rounded-full">{{ tasks().length }}</span> }
                @if (tab.key === 'time' && timeEntries().length > 0) { <span class="ml-1 text-[10px] bg-slate-200 text-slate-600 px-1.5 py-0.5 rounded-full">{{ timeEntries().length }}</span> }
                @if (tab.key === 'documents' && documents().length > 0) { <span class="ml-1 text-[10px] bg-slate-200 text-slate-600 px-1.5 py-0.5 rounded-full">{{ documents().length }}</span> }
              </button>
            }
          </nav>
        </div>

        <!-- OVERVIEW -->
        @if (activeTab() === 'overview') {
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <div class="card-section lg:col-span-2">
              <h2 class="section-title mb-4">Matter Information</h2>
              <div class="grid grid-cols-2 gap-x-8 gap-y-4">
                <div><div class="data-label">Lead Consultant</div><div class="data-value">{{ matter.leadConsultantName }}</div></div>
                <div><div class="data-label">Fee Arrangement</div><div class="data-value">{{ ['Hourly', 'Fixed Fee', 'Hybrid'][matter.feeArrangement] }}</div></div>
                <div><div class="data-label">Estimated Value</div><div class="data-value">{{ matter.estimatedValue ? '£' + matter.estimatedValue.toLocaleString() : '—' }}</div></div>
                <div><div class="data-label">Status</div><div class="data-value">{{ getStatusLabel(matter.status) }}</div></div>
                <div><div class="data-label">Opened</div><div class="data-value">{{ matter.openedDate }}</div></div>
                @if (matter.closedDate) { <div><div class="data-label">Closed</div><div class="data-value">{{ matter.closedDate }}</div></div> }
              </div>
            </div>
            <div class="card-section">
              <h2 class="section-title mb-4">Description</h2>
              <p class="text-sm text-slate-600 leading-relaxed">{{ matter.description || 'No description provided.' }}</p>
            </div>
          </div>
        }

        <!-- NOTES with Add -->
        @if (activeTab() === 'notes') {
          <div class="flex justify-between items-center mb-4">
            <h2 class="section-title">Case Notes</h2>
            <button (click)="showAddNote = true" class="px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg">+ Add Note</button>
          </div>
          @if (notes().length === 0 && !notesLoading()) {
            <div class="card-section text-center py-12"><p class="text-sm text-slate-500">No notes yet. Add the first note to this matter.</p></div>
          }
          <div class="space-y-3">
            @for (note of notes(); track note.id) {
              <div class="card-section">
                <div class="flex items-center justify-between mb-2">
                  <span class="text-xs text-slate-400">{{ note.createdAt.substring(0, 10) }}</span>
                  <span class="text-[10px] px-2 py-0.5 rounded-full font-medium" [class]="note.isClientVisible ? 'bg-blue-50 text-blue-600 border border-blue-200' : 'bg-slate-100 text-slate-500'">{{ note.isClientVisible ? 'Client Visible' : 'Internal' }}</span>
                </div>
                <p class="text-sm text-slate-700 leading-relaxed whitespace-pre-line">{{ note.content }}</p>
              </div>
            }
          </div>
        }

        <!-- TASKS with Add -->
        @if (activeTab() === 'tasks') {
          <div class="flex justify-between items-center mb-4">
            <h2 class="section-title">Tasks</h2>
            <button (click)="showAddTask = true" class="px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg">+ Add Task</button>
          </div>
          @if (tasks().length === 0 && !tasksLoading()) {
            <div class="card-section text-center py-12"><p class="text-sm text-slate-500">No tasks assigned. Create a task to track work.</p></div>
          }
          @if (tasks().length > 0) {
            <div class="card-section overflow-x-auto">
              <table class="table w-full"><thead><tr><th>Task</th><th>Priority</th><th>Status</th><th>Due Date</th></tr></thead>
                <tbody>@for (task of tasks(); track task.id) {
                  <tr><td class="font-medium text-sm">{{ task.title }}</td>
                    <td><span class="text-xs px-2 py-0.5 rounded-full font-medium" [class]="getPriorityClass(task.priority)">{{ getPriorityLabel(task.priority) }}</span></td>
                    <td><span class="text-xs px-2 py-0.5 rounded-full font-medium" [class]="getTaskStatusClass(task.status)">{{ getTaskStatusLabel(task.status) }}</span></td>
                    <td class="text-xs text-slate-500">{{ task.dueDate }}</td></tr>
                }</tbody>
              </table>
            </div>
          }
        }

        <!-- DOCUMENTS with Upload -->
        @if (activeTab() === 'documents') {
          <div class="flex justify-between items-center mb-4">
            <h2 class="section-title">Documents</h2>
            <button (click)="showUploadDoc = true" class="px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg">+ Upload Document</button>
          </div>
          @if (documents().length === 0 && !docsLoading()) {
            <div class="card-section text-center py-12"><p class="text-sm text-slate-500">No documents. Upload files related to this matter.</p></div>
          }
          @if (documents().length > 0) {
            <div class="card-section overflow-x-auto">
              <table class="table w-full"><thead><tr><th>File Name</th><th>Category</th><th>Size</th><th>Version</th><th>Uploaded</th><th></th></tr></thead>
                <tbody>@for (doc of documents(); track doc.id) {
                  <tr><td class="font-medium text-sm">{{ doc.fileName }}</td>
                    <td><span class="text-xs px-2 py-0.5 bg-slate-100 rounded text-slate-600">{{ doc.category || 'General' }}</span></td>
                    <td class="text-xs text-slate-500">{{ formatFileSize(doc.fileSizeBytes) }}</td>
                    <td class="text-xs">v{{ doc.version }}</td>
                    <td class="text-xs text-slate-400">{{ doc.createdAt.substring(0, 10) }}</td>
                    <td><a href="${environment.apiUrl}/documents/{{ doc.id }}/download" target="_blank" class="text-xs text-blue-600 hover:underline font-medium">Download</a></td></tr>
                }</tbody>
              </table>
            </div>
          }
        }

        <!-- TIME with Record -->
        @if (activeTab() === 'time') {
          <div class="flex justify-between items-center mb-4">
            <h2 class="section-title">Time Entries</h2>
            <button (click)="showRecordTime = true" class="px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg">+ Record Time</button>
          </div>
          @if (timeEntries().length === 0 && !timeLoading()) {
            <div class="card-section text-center py-12"><p class="text-sm text-slate-500">No time recorded against this matter yet.</p></div>
          }
          @if (timeEntries().length > 0) {
            <div class="card-section overflow-x-auto">
              <table class="table w-full"><thead><tr><th>Date</th><th>Fee Earner</th><th>Description</th><th>Duration</th><th>Billable</th><th>Status</th></tr></thead>
                <tbody>@for (entry of timeEntries(); track entry.id) {
                  <tr><td class="text-xs">{{ entry.date }}</td><td class="text-sm">{{ entry.userName }}</td>
                    <td class="text-sm max-w-xs truncate">{{ entry.description }}</td>
                    <td class="text-sm font-medium">{{ formatDuration(entry.durationMinutes) }}</td>
                    <td>@if (entry.isBillable) { <span class="text-xs status-active px-2 py-0.5 rounded-full">Billable</span> } @else { <span class="text-xs status-closed px-2 py-0.5 rounded-full">Non-billable</span> }</td>
                    <td><span class="text-xs px-2 py-0.5 rounded-full font-medium" [class]="getTimeStatusClass(entry.status)">{{ getTimeStatusLabel(entry.status) }}</span></td></tr>
                }</tbody>
              </table>
            </div>
          }
        }

        <!-- BILLING -->
        @if (activeTab() === 'billing') {
          <div class="flex justify-between items-center mb-4">
            <h2 class="section-title">Invoices</h2>
            <button (click)="createInvoice()" [disabled]="creatingInvoice()" class="px-3 py-1.5 text-xs font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg disabled:opacity-50">
              @if (creatingInvoice()) { <span class="inline-block w-3 h-3 border-2 border-white border-t-transparent rounded-full animate-spin mr-1"></span> }
              + Generate Invoice
            </button>
          </div>
          @if (invoices().length === 0 && !invoicesLoading()) {
            <div class="card-section text-center py-12"><p class="text-sm text-slate-500">No invoices generated for this matter yet. Click "Generate Invoice" to create one from approved time entries.</p></div>
          }
          @if (invoices().length > 0) {
            <div class="card-section overflow-x-auto">
              <table class="table w-full"><thead><tr><th>Invoice #</th><th>Status</th><th>Issue Date</th><th>Total</th><th>Paid</th><th>Outstanding</th></tr></thead>
                <tbody>@for (inv of invoices(); track inv.id) {
                  <tr><td class="font-mono text-sm font-semibold">{{ inv.invoiceNumber }}</td>
                    <td><span class="text-xs px-2 py-0.5 rounded-full font-medium" [class]="getInvoiceStatusClass(inv.status)">{{ getInvoiceStatusLabel(inv.status) }}</span></td>
                    <td class="text-xs text-slate-500">{{ inv.issueDate || '—' }}</td>
                    <td class="font-medium text-sm">£{{ inv.totalAmount.toLocaleString() }}</td>
                    <td class="text-sm text-emerald-600">£{{ inv.paidAmount.toLocaleString() }}</td>
                    <td class="text-sm font-medium text-amber-600">£{{ (inv.totalAmount - inv.paidAmount).toLocaleString() }}</td></tr>
                }</tbody>
              </table>
            </div>
          }
        }
      }

      <!-- ADD NOTE MODAL -->
      @if (showAddNote) {
        <app-modal title="Add Note" size="lg" (closed)="showAddNote = false">
          <div class="space-y-4">
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Note Content *</label>
              <textarea [(ngModel)]="newNote.content" rows="5" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500" placeholder="Enter case note..."></textarea>
            </div>
            <div class="flex items-center gap-2">
              <input type="checkbox" [(ngModel)]="newNote.isClientVisible" id="clientVisible" class="rounded border-slate-300 text-blue-600 focus:ring-blue-500">
              <label for="clientVisible" class="text-sm text-slate-700">Visible to client</label>
            </div>
          </div>
          <div footer>
            <button (click)="showAddNote = false" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="saveNote()" [disabled]="!newNote.content" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">Save Note</button>
          </div>
        </app-modal>
      }

      <!-- ADD TASK MODAL -->
      @if (showAddTask) {
        <app-modal title="Add Task" size="lg" (closed)="showAddTask = false">
          <div class="space-y-4">
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Title *</label>
              <input [(ngModel)]="newTask.title" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500" placeholder="Task title">
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div><label class="text-xs font-medium text-slate-600 block mb-1">Priority</label>
                <select [(ngModel)]="newTask.priority" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                  <option [ngValue]="0">Low</option><option [ngValue]="1">Medium</option><option [ngValue]="2">High</option><option [ngValue]="3">Urgent</option>
                </select>
              </div>
              <div><label class="text-xs font-medium text-slate-600 block mb-1">Due Date *</label>
                <input type="date" [(ngModel)]="newTask.dueDate" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
              </div>
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Description</label>
              <textarea [(ngModel)]="newTask.description" rows="3" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="Task description..."></textarea>
            </div>
          </div>
          <div footer>
            <button (click)="showAddTask = false" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="saveTask()" [disabled]="!newTask.title || !newTask.dueDate" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">Create Task</button>
          </div>
        </app-modal>
      }

      <!-- UPLOAD DOCUMENT MODAL -->
      @if (showUploadDoc) {
        <app-modal title="Upload Document" size="md" (closed)="showUploadDoc = false">
          <div class="space-y-4">
            <div><label class="text-xs font-medium text-slate-600 block mb-1">File *</label>
              <input type="file" (change)="onFileSelect($event)" class="w-full text-sm border border-slate-300 rounded-lg px-3 py-2 file:mr-4 file:py-1 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-blue-50 file:text-blue-600 hover:file:bg-blue-100">
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Category</label>
              <select [(ngModel)]="uploadCategory" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
                <option value="">General</option><option value="Contract">Contract</option><option value="Correspondence">Correspondence</option>
                <option value="Evidence">Evidence</option><option value="Court Filing">Court Filing</option><option value="Research">Research</option>
              </select>
            </div>
          </div>
          <div footer>
            <button (click)="showUploadDoc = false" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="uploadDocument()" [disabled]="!selectedFile || uploading()" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">
              @if (uploading()) { <span class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span> }
              Upload
            </button>
          </div>
        </app-modal>
      }

      <!-- RECORD TIME MODAL -->
      @if (showRecordTime) {
        <app-modal title="Record Time Entry" size="lg" (closed)="showRecordTime = false">
          <div class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div><label class="text-xs font-medium text-slate-600 block mb-1">Date *</label>
                <input type="date" [(ngModel)]="newTime.date" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
              </div>
              <div><label class="text-xs font-medium text-slate-600 block mb-1">Duration (minutes) *</label>
                <input type="number" [(ngModel)]="newTime.durationMinutes" min="1" max="720" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm">
              </div>
            </div>
            <div><label class="text-xs font-medium text-slate-600 block mb-1">Description *</label>
              <textarea [(ngModel)]="newTime.description" rows="3" class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm" placeholder="Describe the work performed..."></textarea>
            </div>
            <div class="flex items-center gap-2">
              <input type="checkbox" [(ngModel)]="newTime.isBillable" id="billable" class="rounded border-slate-300 text-blue-600 focus:ring-blue-500">
              <label for="billable" class="text-sm text-slate-700">Billable</label>
            </div>
          </div>
          <div footer>
            <button (click)="showRecordTime = false" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
            <button (click)="saveTimeEntry()" [disabled]="!newTime.date || !newTime.description || !newTime.durationMinutes" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50">Save Time Entry</button>
          </div>
        </app-modal>
      }
    </div>
  `
})
export class MatterDetailComponent implements OnInit {
  id = input.required<string>();
  activeTab = signal('overview');
  timeEntries = signal<TimeEntryDto[]>([]); timeLoading = signal(false);
  invoices = signal<InvoiceDto[]>([]); invoicesLoading = signal(false);
  tasks = signal<TaskDto[]>([]); tasksLoading = signal(false);
  notes = signal<NoteDto[]>([]); notesLoading = signal(false);
  documents = signal<DocumentDto[]>([]); docsLoading = signal(false);
  uploading = signal(false);
  creatingInvoice = signal(false);

  showAddNote = false; showAddTask = false; showUploadDoc = false; showRecordTime = false;
  newNote = { content: '', isClientVisible: false };
  newTask = { title: '', description: '', priority: 1, dueDate: '' };
  newTime = { date: '', durationMinutes: 60, description: '', isBillable: true };
  selectedFile: File | null = null;
  uploadCategory = '';

  tabs = [
    { key: 'overview', label: 'Overview' }, { key: 'notes', label: 'Notes' },
    { key: 'tasks', label: 'Tasks' }, { key: 'documents', label: 'Documents' },
    { key: 'time', label: 'Time Entries' }, { key: 'billing', label: 'Billing' },
  ];

  constructor(public store: MatterStore, private timeApi: TimeApiService, private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void {
    this.store.loadMatter(this.id());
    this.newTime.date = new Date().toISOString().split('T')[0];
    this.newTask.dueDate = new Date(Date.now() + 7 * 86400000).toISOString().split('T')[0];
  }

  switchTab(tab: string): void {
    this.activeTab.set(tab);
    if (tab === 'time' && this.timeEntries().length === 0) this.loadTime();
    if (tab === 'billing' && this.invoices().length === 0) this.loadInvoices();
    if (tab === 'tasks' && this.tasks().length === 0) this.loadTasks();
    if (tab === 'notes' && this.notes().length === 0) this.loadNotes();
    if (tab === 'documents' && this.documents().length === 0) this.loadDocuments();
  }

  // --- CRUD Operations ---
  saveNote(): void {
    this.http.post(`${environment.apiUrl}/matters/${this.id()}/notes`, this.newNote).subscribe({
      next: () => { this.showAddNote = false; this.newNote = { content: '', isClientVisible: false }; this.notes.set([]); this.loadNotes(); this.toast.success('Note added successfully'); },
      error: () => this.toast.error('Failed to add note')
    });
  }

  saveTask(): void {
    const payload = { ...this.newTask, matterId: this.id(), assigneeId: '00000000-0000-0000-0000-000000000000' };
    this.http.post(`${environment.apiUrl}/matters/${this.id()}/tasks`, payload).subscribe({
      next: () => { this.showAddTask = false; this.newTask = { title: '', description: '', priority: 1, dueDate: '' }; this.tasks.set([]); this.loadTasks(); this.toast.success('Task created successfully'); },
      error: () => this.toast.error('Failed to create task')
    });
  }

  saveTimeEntry(): void {
    const payload = { ...this.newTime, matterId: this.id() };
    this.timeApi.recordTime(payload).subscribe({
      next: () => { this.showRecordTime = false; this.newTime = { date: new Date().toISOString().split('T')[0], durationMinutes: 60, description: '', isBillable: true }; this.timeEntries.set([]); this.loadTime(); this.toast.success('Time entry recorded'); },
      error: () => this.toast.error('Failed to record time')
    });
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  uploadDocument(): void {
    if (!this.selectedFile) return;
    this.uploading.set(true);
    const formData = new FormData();
    formData.append('file', this.selectedFile);
    formData.append('matterId', this.id());
    if (this.uploadCategory) formData.append('category', this.uploadCategory);

    this.http.post(`${environment.apiUrl}/documents`, formData).subscribe({
      next: () => { this.showUploadDoc = false; this.selectedFile = null; this.uploadCategory = ''; this.uploading.set(false); this.documents.set([]); this.loadDocuments(); this.toast.success('Document uploaded successfully'); },
      error: () => { this.uploading.set(false); this.toast.error('Failed to upload document'); }
    });
  }

  createInvoice(): void {
    const matter = this.store.selected();
    if (!matter) return;
    this.creatingInvoice.set(true);
    this.http.post(`${environment.apiUrl}/invoices`, { matterId: matter.id, clientId: matter.clientId, taxRate: 20 }).subscribe({
      next: () => { this.creatingInvoice.set(false); this.invoices.set([]); this.loadInvoices(); this.toast.success('Invoice generated from approved time entries'); },
      error: (err) => { this.creatingInvoice.set(false); this.toast.error(err?.error?.[0] || err?.error?.title || 'No approved unbilled time entries found'); }
    });
  }

  // --- Data Loading ---
  private loadTime(): void { this.timeLoading.set(true); this.timeApi.getTimeEntries({ matterId: this.id() }).subscribe({ next: r => { this.timeEntries.set(r.items); this.timeLoading.set(false); }, error: () => this.timeLoading.set(false) }); }
  private loadInvoices(): void { this.invoicesLoading.set(true); this.http.get<PaginatedList<InvoiceDto>>(`${environment.apiUrl}/invoices?matterId=${this.id()}`).subscribe({ next: r => { this.invoices.set(r.items); this.invoicesLoading.set(false); }, error: () => this.invoicesLoading.set(false) }); }
  private loadTasks(): void { this.tasksLoading.set(true); this.http.get<TaskDto[]>(`${environment.apiUrl}/matters/${this.id()}/tasks`).subscribe({ next: r => { this.tasks.set(r); this.tasksLoading.set(false); }, error: () => this.tasksLoading.set(false) }); }
  private loadNotes(): void { this.notesLoading.set(true); this.http.get<NoteDto[]>(`${environment.apiUrl}/matters/${this.id()}/notes`).subscribe({ next: r => { this.notes.set(r); this.notesLoading.set(false); }, error: () => this.notesLoading.set(false) }); }
  private loadDocuments(): void { this.docsLoading.set(true); this.http.get<DocumentDto[]>(`${environment.apiUrl}/documents?matterId=${this.id()}`).subscribe({ next: r => { this.documents.set(r); this.docsLoading.set(false); }, error: () => this.docsLoading.set(false) }); }

  // --- Helpers ---
  formatDuration(mins: number): string { const h = Math.floor(mins / 60); const m = mins % 60; return h > 0 ? `${h}h ${m}m` : `${m}m`; }
  formatFileSize(bytes: number): string { if (bytes < 1024) return bytes + ' B'; if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB'; return (bytes / 1048576).toFixed(1) + ' MB'; }
  getStatusLabel(s: number): string { return ['Open', 'In Progress', 'On Hold', 'Awaiting Client', 'Awaiting Third Party', 'Billing Review', 'Closed', 'Archived'][s] ?? ''; }
  getStatusClass(s: number): string { return ['status-info', 'status-active', 'status-warning', 'status-warning', 'status-warning', 'status-pending', 'status-closed', 'status-closed'][s] ?? ''; }
  getMatterType(t: number): string { return ['Conveyancing', 'Litigation', 'Family Law', 'Commercial', 'Employment', 'Criminal', 'Immigration', 'Wills & Probate', 'Personal Injury', 'Other'][t] ?? 'Other'; }
  getTimeStatusLabel(s: number): string { return ['Draft', 'Submitted', 'Approved', 'Rejected', 'Billed', 'Written Off'][s] ?? ''; }
  getTimeStatusClass(s: number): string { return ['status-closed', 'status-info', 'status-active', 'status-error', 'bg-indigo-50 text-indigo-700 border border-indigo-200', 'status-warning'][s] ?? ''; }
  getInvoiceStatusLabel(s: number): string { return ['Draft', 'Issued', 'Partially Paid', 'Paid', 'Cancelled', 'Written Off'][s] ?? ''; }
  getInvoiceStatusClass(s: number): string { return ['status-closed', 'status-info', 'status-warning', 'status-active', 'status-error', 'status-error'][s] ?? ''; }
  getTaskStatusLabel(s: number): string { return ['To Do', 'In Progress', 'Blocked', 'Complete', 'Cancelled'][s] ?? ''; }
  getTaskStatusClass(s: number): string { return ['status-closed', 'status-info', 'status-error', 'status-active', 'status-closed'][s] ?? ''; }
  getPriorityLabel(p: number): string { return ['Low', 'Medium', 'High', 'Urgent'][p] ?? ''; }
  getPriorityClass(p: number): string { return ['status-closed', 'status-info', 'status-warning', 'status-error'][p] ?? ''; }
}
