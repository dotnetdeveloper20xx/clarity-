import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';

interface NotificationDto {
  id: string;
  title: string;
  message: string;
  type: string;
  isRead: boolean;
  entityType: string | null;
  entityId: string | null;
  createdAt: string;
}

@Component({
  selector: 'app-notification-list',
  standalone: true,
  template: `
    <div class="page-container">
      <div class="flex items-center justify-between mb-6">
        <h1 class="page-title">Notifications</h1>
        @if (items().length > 0) {
          <button class="btn btn-sm btn-outline" (click)="markAllRead()">Mark All Read</button>
        }
      </div>

      @if (loading()) {
        <div class="card-section"><span class="loading loading-spinner loading-md text-primary"></span> Loading...</div>
      }

      @if (!loading() && items().length === 0) {
        <div class="card-section text-center py-12">
          <div class="text-4xl mb-4">🔔</div>
          <h3 class="text-lg font-medium text-navy-700">No notifications</h3>
          <p class="text-sm text-navy-500 mt-2">You're all caught up.</p>
        </div>
      }

      @if (!loading() && items().length > 0) {
        <div class="space-y-3">
          @for (n of items(); track n.id) {
            <div class="card-section flex items-start gap-4" [class.opacity-60]="n.isRead">
              <div class="text-2xl">{{ getIcon(n.type) }}</div>
              <div class="flex-1">
                <div class="flex items-center justify-between">
                  <h3 class="font-medium text-navy-900 text-sm">{{ n.title }}</h3>
                  <span class="text-xs text-navy-400">{{ formatDate(n.createdAt) }}</span>
                </div>
                <p class="text-sm text-navy-600 mt-1">{{ n.message }}</p>
              </div>
              @if (!n.isRead) {
                <button class="btn btn-ghost btn-xs" (click)="markRead(n.id)">✓</button>
              }
            </div>
          }
        </div>
      }
    </div>
  `
})
export class NotificationListComponent implements OnInit {
  items = signal<NotificationDto[]>([]);
  loading = signal(false);

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<NotificationDto[]>(`${environment.apiUrl}/notifications`).subscribe({
      next: (data) => { this.items.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  markRead(id: string): void {
    this.http.put(`${environment.apiUrl}/notifications/${id}/read`, {}).subscribe({
      next: () => this.items.update(items => items.map(n => n.id === id ? { ...n, isRead: true } : n))
    });
  }

  markAllRead(): void {
    this.http.put(`${environment.apiUrl}/notifications/mark-all-read`, {}).subscribe({
      next: () => this.items.update(items => items.map(n => ({ ...n, isRead: true })))
    });
  }

  getIcon(type: string): string {
    switch (type) {
      case 'Warning': return '⚠️';
      case 'Action': return '🔴';
      case 'Reminder': return '📌';
      default: return 'ℹ️';
    }
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    if (hours < 1) return 'Just now';
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}
