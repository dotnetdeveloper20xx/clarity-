import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  template: `
    <div class="fixed inset-0 z-50 flex items-center justify-center">
      <div class="fixed inset-0 bg-black/50 backdrop-blur-sm" (click)="cancelled.emit()"></div>
      <div class="relative bg-white rounded-xl shadow-2xl border border-slate-200 w-full max-w-sm mx-4 p-6">
        <h3 class="text-base font-semibold text-slate-900 mb-2">{{ title() }}</h3>
        <p class="text-sm text-slate-600 mb-6">{{ message() }}</p>
        <div class="flex justify-end gap-3">
          <button (click)="cancelled.emit()" class="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50">Cancel</button>
          <button (click)="confirmed.emit()" class="px-4 py-2 text-sm font-medium text-white rounded-lg" [class]="variant() === 'danger' ? 'bg-red-600 hover:bg-red-700' : 'bg-blue-600 hover:bg-blue-700'">{{ confirmText() }}</button>
        </div>
      </div>
    </div>
  `
})
export class ConfirmDialogComponent {
  title = input('Confirm');
  message = input('Are you sure?');
  confirmText = input('Confirm');
  variant = input<'primary' | 'danger'>('primary');
  confirmed = output();
  cancelled = output();
}
