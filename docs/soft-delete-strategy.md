# Soft Delete Strategy

## Principle

In a legal practice management system, data is business-critical. Hard deletes (physically removing rows) create unacceptable risks:

- Lost audit trails
- Broken referential integrity
- Inability to recover from accidental deletion
- Compliance violations (regulated data removed)
- Broken historical reports

Therefore, **all business entities use soft delete by default**.

---

## How Soft Delete Works

When a user "deletes" a record:

1. The record's `IsDeleted` flag is set to `true`
2. `DeletedAt` is set to the current UTC timestamp
3. `DeletedBy` is set to the current user's ID
4. The record remains in the database
5. All normal queries automatically exclude soft-deleted records
6. An audit entry is created recording the deletion

The record is effectively invisible to normal users but remains available for:
- Audit investigation
- Data recovery
- Compliance review
- Historical reporting

---

## Standard Soft Delete Columns

All soft-deletable entities include:

| Column | Type | Purpose |
|--------|------|---------|
| IsDeleted | BIT NOT NULL DEFAULT 0 | Flag indicating deletion |
| DeletedAt | DATETIME2 NULL | When the deletion occurred |
| DeletedBy | UNIQUEIDENTIFIER NULL | Who deleted the record |

---

## EF Core Implementation

A global query filter is applied so soft-deleted records are excluded automatically:

```csharp
// In entity configuration
builder.HasQueryFilter(e => !e.IsDeleted);
```

To include soft-deleted records (for admin/audit views):

```csharp
// Bypass the filter when needed
context.Clients.IgnoreQueryFilters().Where(c => c.IsDeleted)...
```

A `SaveChanges` interceptor converts `Delete` operations into soft-delete updates:

```csharp
// Instead of DELETE, set IsDeleted = true
entry.State = EntityState.Modified;
entry.Entity.IsDeleted = true;
entry.Entity.DeletedAt = DateTime.UtcNow;
entry.Entity.DeletedBy = currentUserId;
```

---

## Entities That Use Soft Delete

| Entity | Soft Delete | Reason |
|--------|-------------|--------|
| Client | Yes | Client history must be preserved |
| ClientContact | Yes | Contact history |
| Matter | Yes | Legal matters are permanent records |
| MatterNote | Yes | Notes may be relevant for audit |
| MatterTask | Yes | Task history |
| TimeEntry | Yes | Time records are financial data |
| Invoice | Yes | Financial records |
| InvoiceLineItem | Yes | Part of invoice (cascades) |
| Document | Yes | Documents may be legally required |
| Disbursement | Yes | Financial records |

---

## Entities That Are NEVER Deleted

Some records should never be deleted, even soft-deleted:

| Entity | Reason |
|--------|--------|
| AuditEntry | Audit logs are append-only and immutable |
| ComplianceCheck | Compliance history is permanent legal record |
| Payment | Financial records; reversals recorded as new entries |

For these, the application does not expose a delete operation at all.

---

## Entities That Use Hard Delete (Exceptions)

Very few entities may use hard delete:

| Entity | Reason |
|--------|--------|
| Notification (old, read) | Housekeeping; no business value after retention period |
| Session tokens (expired) | Security housekeeping |

These are cleaned up by background jobs after their retention period expires.

---

## Cascading Soft Delete

When a parent entity is soft-deleted, child entities may need to cascade:

| Parent | Children | Cascade? |
|--------|----------|----------|
| Client | ClientContacts | Yes — contacts hidden when client archived |
| Matter | MatterNotes, MatterTasks | No — matter closure is different from deletion |
| Invoice | InvoiceLineItems | Yes — line items hidden with invoice |

**Important:** Cascading soft-delete is applied at the application level, not database trigger level. This gives full control and audit capability.

---

## Recovering Soft-Deleted Records

Recovery (un-delete) is available to System Administrators:

1. Admin navigates to the archived/deleted items view
2. Admin selects the record to restore
3. `IsDeleted` is set back to `false`
4. `DeletedAt` and `DeletedBy` are cleared
5. An audit entry records the recovery action

---

## Reporting Considerations

- Standard reports exclude soft-deleted records by default
- Historical reports may include soft-deleted records for completeness
- Clearly indicate deleted records in any view that includes them
- Dashboard counts never include soft-deleted records

---

## Data Retention and Purging

After the retention period expires (e.g., 7+ years for closed matters):

1. Records may be moved to an archive database
2. Or permanently purged (hard delete) — only with explicit business approval
3. Purging is a scheduled, audited administrative process
4. Never automated without human review

See `data-retention-policy.md` for detailed retention periods.
