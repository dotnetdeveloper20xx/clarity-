import { Injectable, signal, computed } from '@angular/core';
import { MatterApiService } from '../services/matter-api.service';
import { MatterDto } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class MatterStore {
  private itemsSignal = signal<MatterDto[]>([]);
  private selectedSignal = signal<MatterDto | null>(null);
  private loadingSignal = signal(false);
  private errorSignal = signal<string | null>(null);
  private totalCountSignal = signal(0);
  private pageNumberSignal = signal(1);

  readonly items = this.itemsSignal.asReadonly();
  readonly selected = this.selectedSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly totalCount = this.totalCountSignal.asReadonly();
  readonly pageNumber = this.pageNumberSignal.asReadonly();
  readonly isEmpty = computed(() => !this.loadingSignal() && this.itemsSignal().length === 0);

  constructor(private api: MatterApiService) {}

  loadMatters(params: { searchTerm?: string; status?: number; clientId?: string; pageNumber?: number; pageSize?: number } = {}): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.api.getMatters(params).subscribe({
      next: (result) => {
        this.itemsSignal.set(result.items);
        this.totalCountSignal.set(result.totalCount);
        this.pageNumberSignal.set(params.pageNumber ?? 1);
        this.loadingSignal.set(false);
      },
      error: (err) => {
        this.errorSignal.set(err?.error?.title ?? 'Failed to load matters.');
        this.loadingSignal.set(false);
      }
    });
  }

  loadMatter(id: string): void {
    this.loadingSignal.set(true);
    this.api.getMatter(id).subscribe({
      next: (matter) => {
        this.selectedSignal.set(matter);
        this.loadingSignal.set(false);
      },
      error: (err) => {
        this.errorSignal.set(err?.error?.title ?? 'Failed to load matter.');
        this.loadingSignal.set(false);
      }
    });
  }

  clearSelected(): void {
    this.selectedSignal.set(null);
  }
}
