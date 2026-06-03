# Database Design

## Overview

Clarity uses SQL Server as its primary data store. The database is designed to handle millions of records, support complex reporting, maintain full audit history, and remain performant under concurrent usage by hundreds of users.

This document provides the high-level database design philosophy and decisions.

---

## Design Principles

| Principle | Explanation |
|-----------|-------------|
| Business-first schema | Tables reflect business concepts (Client, Matter, Invoice), not technical abstractions |
| Referential integrity | Foreign keys enforce relationships at the database level |
| Soft delete everywhere | No hard deletes for business data; use IsDeleted flag |
| Audit by default | All significant tables have CreatedAt, CreatedBy, ModifiedAt, ModifiedBy |
| Optimistic concurrency | RowVersion columns on frequently-edited entities |
| Normalised design | 3NF for transactional tables; denormalisation only for reporting |
| UTC timestamps | All DateTime columns store UTC; conversion happens in the application layer |
| Decimal for money | All monetary values use DECIMAL(18,2) — never FLOAT |
| Guid primary keys | All primary keys are UNIQUEIDENTIFIER (Guid) for distributed-friendly design |
| Meaningful indexes | Indexes based on actual query patterns, not guesswork |

---

## Schema Organisation

Tables are logically grouped by domain but reside in a single database (for simplicity in the first phase). Schema prefixes are used for clarity:

| Schema | Domain |
|--------|--------|
| dbo | Default (shared/cross-cutting) |
| client | Client Management |
| matter | Matter Management |
| doc | Document Management |
| time | Time Recording |
| billing | Billing |
| payment | Payment |
| compliance | Compliance |
| security | Security / Identity |
| audit | Audit Logging |
| notification | Notifications |

---

## Key Design Decisions

### 1. GUIDs vs Integer IDs

**Decision:** Use GUIDs (UNIQUEIDENTIFIER) as primary keys.

**Why:**
- Safe for distributed systems (no collisions)
- Can be generated client-side (offline-first future possibility)
- No sequential predictability (security benefit)
- Compatible with Entity Framework Core default

**Trade-off:** Slightly larger than INT, but manageable with sequential GUIDs (NEWSEQUENTIALID() or application-generated sequential GUIDs).

### 2. Soft Delete vs Hard Delete

**Decision:** Soft delete for all business entities. Hard delete only for truly ephemeral data (e.g., expired notification preferences).

**Why:** Legal firms cannot afford to lose data. Deleted records may be needed for:
- Compliance investigations
- Audit trails
- Data recovery
- Historical reporting

### 3. Separate Audit Table vs CDC

**Decision:** Application-level audit table (AuditLog) with before/after JSON.

**Why:**
- Full control over what is audited
- Business-meaningful audit messages
- Searchable and reportable
- No dependency on SQL Server Enterprise features

### 4. Document Storage

**Decision:** Metadata in SQL Server; file bytes in external storage (file system or blob).

**Why:**
- SQL Server is not optimal for large binary storage
- Blob storage scales independently
- Cheaper at scale
- Easier backup/restore of database (no large BLOBs)

### 5. Reporting Strategy

**Decision:** Optimised read queries and SQL views for the first phase. Separate read models if performance requires it later.

**Why:**
- Simpler architecture initially
- Views can be indexed if needed
- Avoids premature optimisation
- Can migrate to dedicated reporting database later

---

## Concurrency Strategy

Entities that are frequently edited by multiple users use optimistic concurrency:

- Matter
- TimeEntry
- Invoice (Draft only)
- Client

Implementation: `RowVersion` column (ROWVERSION type in SQL Server). EF Core checks this value before saving. If another user modified the record, a concurrency exception is thrown.

---

## Multi-Tenancy

**Current design:** Single-tenant (one firm per deployment).

**Future consideration:** If multi-tenancy is required, the schema supports adding a TenantId column to all tables and applying row-level security. The design does not preclude this.
