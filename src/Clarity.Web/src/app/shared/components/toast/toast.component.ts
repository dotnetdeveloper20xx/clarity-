import { Component, Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<Toast[]>([]);
  private counter = 0;

  success(message: string): void { this.add(message, 'success'); }
  error(message: string): void { this.add(message, 'error'); }
  info(message: string): void { this.add(message, 'info'); }

  private add(message: string, type: Toast['type']): void {
    const id = ++this.counter;
    this.toasts.update(t => [...t, { id, message, type }]);
    setTimeout(() => this.remove(id), 4000);
  }

  remove(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }
}

@Component({
  selector: 'app-toasts',
  standalone: true,
  template: `
    <div class="fixed top-4 right-4 z-[100] space-y-2 max-w-sm">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="flex items-center gap-3 px-4 py-3 rounded-lg shadow-lg border text-sm font-medium animate-slide-in"
          [class]="getClass(toast.type)">
          <span class="flex-1">{{ toast.message }}</span>
          <button (click)="toastService.remove(toast.id)" class="opacity-60 hover:opacity-100">×</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .animate-slide-in { animation: slideIn 0.3s ease-out; }
    @keyframes slideIn { from { opacity: 0; transform: translateX(20px); } to { opacity: 1; transform: translateX(0); } }
  `]
})
export class ToastComponent {
  constructor(public toastService: ToastService) {}

  getClass(type: string): string {
    switch (type) {
      case 'success': return 'bg-emerald-50 border-emerald-200 text-emerald-800';
      case 'error': return 'bg-red-50 border-red-200 text-red-800';
      default: return 'bg-blue-50 border-blue-200 text-blue-800';
    }
  }
}
