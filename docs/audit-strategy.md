# Audit Strategy

## Purpose

The audit system provides a complete, tamper-proof record of every significant action performed in Clarity. This serves:

- **Compliance** — Regulatory bodies may require proof of actions taken
- **Investigation** — Trace what happened, when, and by whom
- **Accountability** — Users know their actions are recorded
- **Recovery** — Understand what changed if data needs to be restored
- **Reporting** — Activity reports for management

---

## What Gets Audited

### Always Audited (Mandatory)

| Category | Actions |
|----------|---------|
| Client | Created, Updated, Status Changed, Archived |
| Matter | Created, Updated, Status Changed, Closed, Reopened |
| Time Entry | Created, Updated, Approved, Reverted, Deleted |
| Invoice | Created, Issued, Paid, Written Off |
| Payment | Recorded, Reversed |
| Document | Uploaded, Archived, Downloaded |
| Compliance | Check Created, Check Completed |
| User | Created, Updated, Disabled, Role Assigned, Role Revoked |
| Security | Login Success, Login Failed, Lockout, Password Changed |

### Not Audited (Performance)

| Category | Reason |
|----------|--------|
| Reading/viewing data | Too high volume; use access logs if needed |
| Notification read status | Low business value |
| Dashboard queries | No state change |

---

## Audit Record Structure

Each audit entry captures:

| Field | Description | Example |
|-------|-------------|---------|
| Timestamp | UTC time of action | 2025-03-15T14:22:03Z |
| UserId | Who performed the action | 3fa85f64-5717-4562-b3fc-2c963f66afa6 |
| UserEmail | Denormalized email (for readability in logs) | john.smith@firm.com |
| Action | What was done | StatusChanged |
| EntityType | Type of entity affected | Matter |
| EntityId | ID of entity affected | 8b2e1a4c-... |
| OldValues | JSON of previous state | {"Status": "Open"} |
| NewValues | JSON of new state | {"Status": "Closed"} |
| CorrelationId | Request trace ID | req-abc123 |
| IpAddress | Client IP (if available) | 192.168.1.50 |

---

## Implementation

### Application-Level Auditing

Audit entries are created by an EF Core `SaveChanges` interceptor:

1. Before `SaveChanges`, the interceptor detects entities marked as Added, Modified, or Deleted
2. For modified entities, it captures the original values (OldValues) and current values (NewValues)
3. After `SaveChanges` succeeds, audit entries are written to the `audit.AuditEntries` table
4. The current user and correlation ID are injected from the HTTP context

### MediatR Pipeline Auditing

For command-level auditing (business actions like "Approve Time Entry"), a MediatR pipeline behaviour records the command details and outcome.

---

## Audit Log Rules

| Rule | Detail |
|------|--------|
| Append-only | Audit entries are never updated or deleted |
| No user can delete | Even administrators cannot delete audit logs |
| Minimum retention | 7 years (configurable per regulation) |
| Searchable | By user, entity, date range, action type, correlation ID |
| Performant | Writes are async where possible; reads use indexed queries |
| Tamper-evident | Consider checksums or sequential IDs for integrity (future) |

---

## Querying Audit Logs

Common audit queries:

| Query | Use Case |
|-------|----------|
| All actions by a specific user | Investigate user activity |
| All changes to a specific entity | Entity change history |
| All actions in a date range | Period review |
| All actions of a specific type | Pattern detection (e.g., all deletions) |
| Trace by correlation ID | Follow a single request through the system |

---

## Security Audit (Separate)

Security-sensitive actions get additional logging:

- Login attempts (success and failure)
- Password changes
- Role assignments
- Permission changes
- Account lockouts
- Session creation/expiry

These may feed into a SIEM (Security Information and Event Management) system in production.

---

## Access to Audit Logs

| Role | Access Level |
|------|-------------|
| Compliance Officer | Full read access to all audit logs |
| System Administrator | Full read access |
| Team Leader | Read access to their team's activity |
| Support User | Read access (with audit trail of their own access) |
| Other roles | No direct access to audit logs |

---

## Future Enhancements

- **Event sourcing** for financial domain (complete reconstruction from events)
- **Blockchain-style hashing** for tamper evidence
- **Export to external SIEM** for security monitoring
- **Automated anomaly detection** (unusual access patterns)
