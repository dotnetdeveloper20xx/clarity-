import { Injectable, signal, computed } from '@angular/core';
import { ClientApiService } from '../services/client-api.service';
import { ClientDto, PaginatedList } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ClientStore {
  private itemsSignal = signal<ClientDto[]>([]);
  private selectedSignal = signal<ClientDto | null>(null);
  private loadingSignal = signal(false);
  private errorSignal = signal<string | null>(null);
  private totalCountSignal = signal(0);
  private pageNumberSignal = signal(1);
  private searchTermSignal = signal('');

  readonly items = this.itemsSignal.asReadonly();
  readonly selected = this.selectedSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly totalCount = this.totalCountSignal.asReadonly();
  readonly pageNumber = this.pageNumberSignal.asReadonly();
  readonly searchTerm = this.searchTermSignal.asReadonly();
  readonly isEmpty = computed(() => !this.loadingSignal() && this.itemsSignal().length === 0);

  constructor(private api: ClientApiService) {}

  loadClients(params: { searchTerm?: string; status?: number; pageNumber?: number; pageSize?: number } = {}): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);
    if (params.searchTerm !== undefined) this.searchTermSignal.set(params.searchTerm);
    if (params.pageNumber) this.pageNumberSignal.set(params.pageNumber);

    this.api.getClients(params).subscribe({
      next: (result) => {
        this.itemsSignal.set(result.items);
        this.totalCountSignal.set(result.totalCount);
        this.loadingSignal.set(false);
      },
      error: (err) => {
        this.errorSignal.set(err?.error?.title ?? 'Failed to load clients.');
        this.loadingSignal.set(false);
      }
    });
  }

  loadClient(id: string): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    this.api.getClient(id).subscribe({
      next: (client) => {
        this.selectedSignal.set(client);
        this.loadingSignal.set(false);
      },
      error: (err) => {
        this.errorSignal.set(err?.error?.title ?? 'Failed to load client.');
        this.loadingSignal.set(false);
      }
    });
  }

  clearSelected(): void {
    this.selectedSignal.set(null);
  }
}
