# Security Model

## Overview

Clarity implements enterprise-grade security with multiple layers of protection:

1. **Authentication** — JWT tokens with refresh token rotation
2. **Authorization** — Role-based and policy-based access control
3. **Session Management** — Trackable sessions with revocation
4. **Account Protection** — Lockout after failed attempts
5. **Matter-Level Security** — Fine-grained access control per matter
6. **Security Auditing** — All security events are logged
7. **Secure Configuration** — Secrets managed outside source code

## Authentication Flow

```
1. User submits email + password → POST /api/auth/login
2. Server validates credentials
3. On success: returns access token (15 min) + refresh token (7 days)
4. Client stores tokens and uses access token for all API calls
5. When access token expires: POST /api/auth/refresh with refresh token
6. Server validates refresh token, rotates it, returns new pair
7. On logout: refresh token is revoked server-side
```

## Token Strategy

| Token | Lifetime | Purpose | Storage |
|-------|----------|---------|---------|
| Access Token (JWT) | 15 minutes | Authenticate API requests | Memory/localStorage |
| Refresh Token | 7 days | Obtain new access token without re-login | Secure storage |

### Refresh Token Rotation

Every time a refresh token is used, it is invalidated and a new one is issued. This limits the damage window if a token is compromised.

## Account Lockout

- **Threshold**: 5 failed login attempts
- **Lockout Duration**: 15 minutes
- **Reset**: Successful login resets the counter
- **Admin Override**: Admins can unlock accounts manually

## Authorization Policies

| Policy | Required Roles |
|--------|---------------|
| CanViewClients | Any authenticated user |
| CanManageClients | Admin, Consultant, TeamLeader, LegalAssistant |
| CanCloseMatters | Admin, TeamLeader |
| CanViewBilling | Admin, Finance, TeamLeader |
| CanIssueInvoices | Admin, Finance |
| CanRecordPayments | Admin, Finance |
| CanViewCompliance | Admin, Compliance |
| CanManageUsers | Admin |
| CanViewDiagnostics | Admin, Support |
| CanViewAudit | Admin, Compliance, Support |

## Matter-Level Security

Beyond role-based access, individual matters can have explicit access grants:

| Level | Permissions |
|-------|-------------|
| Read | View matter details, documents, timeline |
| Contribute | Read + add notes, upload documents, record time |
| Manage | Contribute + change status, assign team, close |
| Restricted | No access (explicitly blocked) |

## Security Audit Log

All security events are recorded in `SecurityAuditLogs`:

| Event Type | When |
|-----------|------|
| LoginSuccess | Successful authentication |
| LoginFailed | Invalid credentials |
| Lockout | Account locked after repeated failures |
| AccountUnlocked | Admin unlocks account |
| PasswordChanged | User changes password |
| SessionRevoked | Session terminated |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | Authenticate and get tokens |
| POST | /api/auth/refresh | Rotate tokens |
| POST | /api/auth/logout | Revoke refresh token |
| GET | /api/auth/me | Get current user info |
| POST | /api/auth/change-password | Change password |
| GET | /api/auth/sessions | List user's active sessions |
| POST | /api/auth/sessions/{id}/revoke | Revoke a session |
| GET | /api/security/audit-log | Security event log (Admin) |
| GET | /api/security/active-sessions | All active sessions (Admin) |
| GET | /api/security/locked-accounts | Locked accounts (Admin) |
| POST | /api/security/users/{id}/unlock | Unlock account (Admin) |

## Security Principles

- Never trust the frontend — all authorization enforced server-side
- Never reveal whether an email exists (generic "Invalid login details")
- Never expose internal details in error messages
- All sensitive actions are audited
- Financial actions require Finance/Admin role
- Compliance actions require Compliance/Admin role
- Session management allows users and admins to revoke access
