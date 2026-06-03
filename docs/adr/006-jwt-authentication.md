# ADR-006: JWT Authentication with Refresh Token Rotation

## Status
Accepted

## Context
The SPA frontend needs stateless API authentication. Options: cookie-based sessions, JWT, OAuth2/OpenID Connect.

## Decision
Use JWT bearer tokens (15-min expiry) with refresh token rotation (7-day expiry) stored server-side.

## Consequences
- Stateless API authentication (scalable)
- Short-lived access tokens limit damage window
- Refresh rotation prevents token reuse after theft
- Server-side refresh token storage enables revocation
- Sessions are trackable and revocable
- More complex than simple cookies (accepted for security benefits)
- Future: can integrate with Azure AD/OAuth2 for SSO
