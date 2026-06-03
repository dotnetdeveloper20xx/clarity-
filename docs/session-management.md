# Session Management

## Overview

Clarity tracks user sessions to provide security visibility and allow both users and administrators to manage active connections.

## How Sessions Work

1. On login, a `UserSession` record is created alongside the refresh token
2. Sessions track: user, device info, IP address, creation time, last activity
3. Users can view their own active sessions
4. Users can revoke any of their sessions (forces re-login on that device)
5. Admins can view all sessions and revoke any session

## Session Lifecycle

```
Login → Session Created → Active (refresh extends) → Revoked / Expired
```

## API Endpoints

### User Self-Service

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/auth/sessions | List my active sessions |
| POST | /api/auth/sessions/{id}/revoke | Revoke one of my sessions |

### Admin Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/security/active-sessions | All active sessions |
| POST | /api/security/sessions/{id}/revoke | Admin revoke any session |

## Use Cases

### Lost Device

1. User reports lost laptop
2. Admin navigates to Security → Active Sessions
3. Admin finds the session from that device
4. Admin revokes the session
5. The associated refresh token is invalidated
6. Next API call from that device will fail with 401
7. Stolen device can no longer access the platform

### Suspicious Activity

1. Admin notices unusual login patterns in security audit log
2. Admin views active sessions for the user
3. Admin revokes suspicious sessions
4. Optionally locks the account pending investigation

## Security Considerations

- Revoking a session immediately invalidates the refresh token
- The access token remains valid until its 15-minute expiry (acceptable trade-off)
- For immediate lockout, admin should also lock the account
- All session operations are logged in the security audit log
