# Audit Trail

## Purpose

The audit trail provides an immutable, searchable record of every significant action performed in Clarity. It serves compliance, investigation, and accountability needs.

## What Gets Audited

| Action | Entities | Detail Captured |
|--------|----------|----------------|
| StatusChanged | Matter, TimeEntry, Invoice | Old status, new status, reason |
| Created | Client, Matter, Invoice | Key creation details |
| Updated | Client, Matter | Changed fields (before/after) |
| Deleted | Any soft-deletable entity | Who deleted, when |
| Approved | TimeEntry | Approver, timestamp |
| Rejected | TimeEntry | Reason, who rejected |
| Issued | Invoice | Issue date, due date |
| PaymentRecorded | Payment | Amount, method, invoice |
| ComplianceCompleted | ComplianceCheck | Result, risk level, notes |

## Audit Entry Structure

| Field | Description |
|-------|-------------|
| Timestamp | When the action occurred (UTC) |
| UserId | Who performed it |
| UserEmail | Denormalized for query readability |
| Action | What was done |
| EntityType | Which entity type |
| EntityId | Which specific entity |
| OldValues | JSON of previous state |
| NewValues | JSON of new state |
| CorrelationId | Request trace ID |
| IpAddress | Client IP |

## Implementation

The `IAuditService` is called explicitly by workflow handlers after successful state changes. This gives full control over what is recorded and when.

```csharp
await _audit.RecordAsync("Matter", matterId, "StatusChanged",
    new { Status = "Open" },
    new { Status = "Closed" },
    "All work completed and invoiced.");
```

## Timeline vs Audit

| Feature | Audit Trail | Activity Timeline |
|---------|-------------|-------------------|
| Purpose | Accountability & compliance | User understanding |
| Audience | Compliance officers, admins | All users on the matter |
| Language | Technical (field names, IDs) | Human-friendly (plain English) |
| Immutable | Yes (never modified) | Yes |
| Searchable | By entity, user, date, action | By entity, chronological |

Both are recorded for the same event, but serve different purposes.

## API Endpoint

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/timeline/matter/{matterId} | Get matter audit timeline |

## Retention

Audit entries are retained for a minimum of 7 years (configurable). They can never be deleted by any user including administrators.
