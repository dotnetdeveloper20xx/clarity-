import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  template: `
    <div class="fixed inset-0 z-50 flex items-center justify-center" (click)="onBackdropClick($event)">
      <div class="fixed inset-0 bg-black/50 backdrop-blur-sm"></div>
      <div class="relative bg-white rounded-xl shadow-2xl border border-slate-200 w-full mx-4 overflow-hidden animate-in"
           [class]="sizeClass()" (click)="$event.stopPropagation()">
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200 bg-slate-50">
          <h2 class="text-lg font-semibold text-slate-900">{{ title() }}</h2>
          <button (click)="closed.emit()" class="p-1 hover:bg-slate-200 rounded-lg transition-colors">
            <svg class="w-5 h-5 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
          </button>
        </div>
        <!-- Body -->
        <div class="px-6 py-5 max-h-[70vh] overflow-y-auto">
          <ng-content />
        </div>
        <!-- Footer -->
        <div class="px-6 py-4 border-t border-slate-200 bg-slate-50 flex justify-end gap-3">
          <ng-content select="[footer]" />
        </div>
      </div>
    </div>
  `,
  styles: [`
    .animate-in { animation: modalIn 0.2s ease-out; }
    @keyframes modalIn { from { opacity: 0; transform: scale(0.95) translateY(10px); } to { opacity: 1; transform: scale(1) translateY(0); } }
  `]
})
export class ModalComponent {
  title = input('');
  size = input<'sm' | 'md' | 'lg' | 'xl'>('md');
  closed = output();

  sizeClass(): string {
    switch (this.size()) {
      case 'sm': return 'max-w-md';
      case 'md': return 'max-w-lg';
      case 'lg': return 'max-w-2xl';
      case 'xl': return 'max-w-4xl';
    }
  }

  onBackdropClick(event: Event): void {
    this.closed.emit();
  }
}
