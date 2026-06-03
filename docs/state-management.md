# State Management

## Approach

Clarity uses **Angular Signals** for lightweight, reactive state management. Each feature has its own store — no monolithic global store.

## Why Signals (not NgRx/NGXS)

- Simpler mental model for the team
- Less boilerplate
- Built into Angular (no external dependency)
- Sufficient for this application's complexity
- Easy to upgrade to a heavier solution if needed later

## Store Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class FeatureStore {
  // Private writable signals
  private itemsSignal = signal<Item[]>([]);
  private loadingSignal = signal(false);
  private errorSignal = signal<string | null>(null);

  // Public read-only signals
  readonly items = this.itemsSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly isEmpty = computed(() => !this.loading() && this.items().length === 0);

  constructor(private api: FeatureApiService) {}

  load(): void {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);
    this.api.getItems().subscribe({
      next: (items) => { this.itemsSignal.set(items); this.loadingSignal.set(false); },
      error: (err) => { this.errorSignal.set(err.message); this.loadingSignal.set(false); }
    });
  }
}
```

## Data Flow

```
User Action (click, navigate)
  → Component calls store.load()
  → Store sets loading = true
  → Store calls API service
  → API service makes HTTP request
  → On success: store sets items, loading = false
  → On error: store sets error, loading = false
  → Component reads signals reactively
  → Template updates automatically
```

## Component Usage

```typescript
@Component({
  template: `
    @if (store.loading()) { <loading-spinner /> }
    @if (store.error()) { <error-panel [message]="store.error()" /> }
    @if (store.isEmpty()) { <empty-state /> }
    @for (item of store.items(); track item.id) { ... }
  `
})
export class ListComponent implements OnInit {
  constructor(public store: FeatureStore) {}
  ngOnInit() { this.store.load(); }
}
```

## Stores Implemented

| Store | Features |
|-------|----------|
| AuthService | login, logout, token management, role checks |
| ClientStore | load list (paginated, searchable), load single, clear |
| MatterStore | load list (paginated, filterable), load single, clear |

## Adding a New Store

1. Create `src/app/core/stores/your-feature.store.ts`
2. Follow the standard pattern (items, loading, error, isEmpty)
3. Inject the corresponding API service
4. Provide methods for load, create, update as needed
5. Components inject the store and read signals in templates

## State Persistence

- Auth token persists in `localStorage`
- List filters persist in store signals (survive navigation within session)
- State is NOT persisted across page refreshes (except auth)
