# Authentication & Authorization

## Authentication

Clarity uses JWT (JSON Web Token) bearer authentication.

### Flow

1. User submits email + password to `POST /api/auth/login`
2. Server validates credentials against the Users table
3. On success, server returns a signed JWT containing user ID, email, name, and roles
4. Client stores token and sends it with every subsequent request as `Authorization: Bearer <token>`
5. Token expires after 8 hours

### Token Claims

| Claim | Description |
|-------|-------------|
| NameIdentifier | User's GUID ID |
| Email | User's email |
| Name | User's full name |
| Role | One claim per assigned role |

### Security Configuration

- Signing key: configured in `appsettings.json` → `Jwt:Key` (in production, use Azure Key Vault)
- Algorithm: HMAC-SHA256
- Token lifetime: 8 hours
- Validation: Issuer, Audience, Lifetime, and Signing Key are all validated

## Authorization

### Role-Based Access Control

Roles are stored in the database and assigned to users via the `UserRoles` join table.

**Seeded Roles:**
- Admin
- Consultant
- LegalAssistant
- TeamLeader
- Finance
- Compliance
- Support
- Client

### Protecting Endpoints

Controllers use `[Authorize]` attribute. Specific role restrictions can be added:

```csharp
[Authorize(Roles = "Finance,Admin")]
public async Task<IActionResult> IssueInvoice(...)
```

### CurrentUserService

The `ICurrentUserService` interface provides access to the authenticated user's identity from any handler:

```csharp
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
}
```

This is injected into command/query handlers to enforce business rules based on the current user.

## Password Security

For development, passwords are hashed using SHA256. In production, this should be replaced with BCrypt or Argon2 via a proper identity library.

## Future Enhancements

- Refresh token rotation
- Account lockout after failed attempts
- Two-factor authentication
- Azure AD / OAuth integration
- Permission-based (not just role-based) authorization policies
