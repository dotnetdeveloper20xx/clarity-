# Role-Based UI

## Principle

The frontend shows or hides UI elements based on the authenticated user's roles. This is a UX convenience — **backend authorization is the actual security boundary**.

## How It Works

1. User logs in → backend returns JWT with roles
2. `AuthService` stores roles in a signal
3. Components and the shell read `auth.hasRole()` / `auth.hasAnyRole()`
4. Navigation items, buttons, and sections are conditionally rendered

## AuthService API

```typescript
auth.roles()              // Signal<string[]> — current user's roles
auth.hasRole('Finance')   // boolean — check single role
auth.hasAnyRole(['Finance', 'Admin'])  // boolean — check any of multiple roles
```

## Shell Sidebar Example

```html
@if (auth.hasAnyRole(['Finance', 'Admin'])) {
  <a routerLink="/billing">Billing</a>
}
@if (auth.hasAnyRole(['Compliance', 'Admin'])) {
  <a routerLink="/compliance">Compliance</a>
}
```

## Role Visibility Matrix

| Feature | Admin | Consultant | TeamLeader | Finance | Compliance | Client |
|---------|-------|-----------|-----------|---------|-----------|--------|
| Dashboard | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| Clients | ✓ | ✓ | ✓ | ✓ | ✓ | Own only |
| Matters | ✓ | ✓ | ✓ | Read | Read | Own only |
| Time Recording | ✓ | ✓ | ✓ | Read | — | — |
| Billing | ✓ | — | — | ✓ | — | Invoices |
| Compliance | ✓ | — | — | — | ✓ | — |
| Admin | ✓ | — | — | — | — | — |

## Future: Permission-Based

As the platform matures, the role-based approach can be extended to permission-based:

```typescript
auth.hasPermission('matter.close')
auth.hasPermission('invoice.issue')
```

This provides finer-grained control without changing the pattern.
