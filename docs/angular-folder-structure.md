# Angular Folder Structure

## Location

```
src/Clarity.Web/
```

## Directory Layout

```
src/
├── app/
│   ├── core/                       # Application-wide singletons
│   │   ├── guards/
│   │   │   └── auth.guard.ts       # Route protection
│   │   ├── interceptors/
│   │   │   ├── auth.interceptor.ts         # JWT token attachment
│   │   │   ├── error.interceptor.ts        # Global error handling
│   │   │   └── correlation.interceptor.ts  # Request tracing
│   │   ├── models/
│   │   │   └── api.models.ts       # All TypeScript interfaces/DTOs
│   │   ├── services/
│   │   │   ├── auth.service.ts     # Authentication + state
│   │   │   ├── client-api.service.ts
│   │   │   ├── matter-api.service.ts
│   │   │   └── time-api.service.ts
│   │   └── stores/
│   │       ├── client.store.ts     # Client feature state
│   │       └── matter.store.ts     # Matter feature state
│   │
│   ├── shared/                     # Reusable components (future)
│   │   └── (components, pipes, directives)
│   │
│   ├── features/                   # Lazy-loaded feature pages
│   │   ├── auth/login/
│   │   ├── dashboard/
│   │   ├── clients/
│   │   │   ├── client-list/
│   │   │   ├── client-detail/
│   │   │   └── client-form/
│   │   ├── matters/
│   │   │   ├── matter-list/
│   │   │   └── matter-detail/
│   │   ├── time-recording/
│   │   │   └── time-list/
│   │   ├── billing/
│   │   │   └── invoice-list/
│   │   └── compliance/
│   │       └── compliance-list/
│   │
│   ├── shell/
│   │   └── shell.component.ts      # Main layout (sidebar + header + outlet)
│   │
│   ├── app.routes.ts               # All route definitions
│   ├── app.config.ts               # Providers (interceptors, router, etc.)
│   └── app.component.ts            # Root component
│
├── environments/
│   ├── environment.ts              # Dev API URL
│   └── environment.prod.ts         # Prod API URL
│
├── styles.css                      # Tailwind imports + custom layers
├── index.html                      # HTML shell with DaisyUI theme
└── main.ts                         # Bootstrap entry point
```

## Conventions

| Convention | Rule |
|-----------|------|
| Components | One per file, standalone, in feature folders |
| Services | In `core/services/`, `providedIn: 'root'` |
| Stores | In `core/stores/`, one per feature domain |
| Models | All in `core/models/api.models.ts` |
| Guards | In `core/guards/`, functional style |
| Interceptors | In `core/interceptors/`, functional style |

## Where to Put New Code

| "I need to..." | Put it in... |
|----------------|-------------|
| Add a new page | `features/<domain>/<page-name>/` |
| Add a new API call | `core/services/<domain>-api.service.ts` |
| Add feature state | `core/stores/<domain>.store.ts` |
| Add a reusable component | `shared/components/` |
| Add a route guard | `core/guards/` |
| Add a DTO interface | `core/models/api.models.ts` |
| Add a new route | `app.routes.ts` (lazy loaded) |
