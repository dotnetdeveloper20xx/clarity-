# Frontend Architecture

## Overview

The Clarity frontend is an Angular single-page application (SPA) using standalone components, signals-based state management, Tailwind CSS with DaisyUI for styling, and a feature-store pattern for predictable data flow.

## Technology Stack

- **Angular 19** — Latest with standalone components, signals, control flow syntax
- **TypeScript** — Strict mode enabled
- **Tailwind CSS 3** — Utility-first styling
- **DaisyUI 4** — Component library built on Tailwind
- **RxJS** — For HTTP streams and async operations
- **Angular Signals** — For reactive state management in stores

## Architecture Pattern

```
Component → Store (Signal-based) → API Service → HTTP → Backend API
```

Components never call HTTP directly. This separation provides:
- Testable stores independent of UI
- Cacheable state that persists across navigation
- Consistent loading/error handling
- Single source of truth per feature

## Project Structure

```
src/app/
├── core/                    # Singletons: services, guards, interceptors, stores
│   ├── guards/             # Route protection (auth, role)
│   ├── interceptors/       # HTTP interceptors (auth, error, correlation)
│   ├── models/             # TypeScript interfaces for API DTOs
│   ├── services/           # API services (HTTP calls only)
│   └── stores/             # Feature stores (signal-based state)
├── shared/                  # Reusable UI components
├── features/               # Feature pages (lazy-loaded)
│   ├── auth/login/
│   ├── dashboard/
│   ├── clients/
│   ├── matters/
│   ├── time-recording/
│   ├── billing/
│   └── compliance/
├── shell/                   # Application layout (sidebar, header)
├── app.routes.ts           # Route definitions
├── app.config.ts           # Providers and configuration
└── app.component.ts        # Root component
```

## State Management

Each feature has a dedicated store using Angular signals:

| Store | Responsibility |
|-------|---------------|
| AuthService | Token, user, roles, login/logout |
| ClientStore | Client list, selected client, pagination |
| MatterStore | Matter list, selected matter, filters |
| (Future) TimeStore | Time entries, weekly view |
| (Future) BillingStore | Invoices, financials |
| (Future) ComplianceStore | Checks, risk levels |

### Standard Store Shape

Every store follows the same pattern:
- `items` — signal with the list data
- `selected` — signal with the currently viewed item
- `loading` — signal indicating async operation in progress
- `error` — signal with error message or null
- `totalCount` — pagination total
- `isEmpty` — computed signal (not loading AND no items)

## Routing

All feature routes are lazy-loaded using `loadComponent()` for optimal bundle splitting. The shell layout wraps all authenticated routes.

## Interceptors

| Interceptor | Purpose |
|-------------|---------|
| authInterceptor | Attaches JWT bearer token to requests |
| correlationInterceptor | Adds X-Correlation-Id header for tracing |
| errorInterceptor | Handles 401 (logout), 403 (warn) globally |

## Role-Aware UI

The sidebar conditionally shows navigation items based on user roles. The `AuthService.hasRole()` and `hasAnyRole()` methods drive visibility.

Frontend role checks are for UX convenience only — backend authorization is the security boundary.

## Styling

- Custom DaisyUI theme "clarity" with professional navy/blue palette
- Utility classes via `@layer components` for reusable patterns
- Responsive grid layouts
- Loading skeletons for perceived performance
- Status badges with colour-coded severity
