# Indexing Strategy

## Philosophy

Indexes are created based on actual query patterns, not speculation. The principle is:

1. Add indexes for known query patterns (listed below)
2. Monitor slow queries in production
3. Add indexes as needed based on real performance data
4. Review index usage periodically and remove unused indexes

## Index Types Used

| Type | When Used |
|------|-----------|
| Clustered Index | Primary key (default on Id columns) |
| Non-Clustered Index | Foreign keys, frequently filtered/sorted columns |
| Unique Index | Business keys (reference numbers, email) |
| Filtered Index | Queries that always filter on a condition (e.g., IsDeleted = 0) |
| Composite Index | Multi-column lookups |
| Covering Index | High-frequency queries that benefit from INCLUDE columns |

---

## Core Indexes

### client.Clients

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Clients | Id | Clustered | Primary key |
| UX_Clients_ReferenceNumber | ReferenceNumber | Unique | Lookup by reference |
| IX_Clients_Email | Email | Non-clustered | Login/search by email |
| IX_Clients_Name | Name | Non-clustered | Search by name |
| IX_Clients_Status | Status | Filtered (IsDeleted=0) | Filter active clients by status |

### matter.Matters

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Matters | Id | Clustered | Primary key |
| UX_Matters_ReferenceNumber | ReferenceNumber | Unique | Lookup by reference |
| IX_Matters_ClientId | ClientId | Non-clustered | Get matters for a client |
| IX_Matters_LeadConsultantId | LeadConsultantId | Non-clustered | Get matters for a consultant |
| IX_Matters_Status | Status | Filtered (IsDeleted=0) | Filter by status |
| IX_Matters_OpenedDate | OpenedDate | Non-clustered | Date range queries |
| IX_Matters_ClientId_Status | ClientId, Status | Composite | Client matters by status |

### time.TimeEntries

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_TimeEntries | Id | Clustered | Primary key |
| IX_TimeEntries_MatterId | MatterId | Non-clustered | Get time for a matter |
| IX_TimeEntries_UserId | UserId | Non-clustered | Get time for a user |
| IX_TimeEntries_Date | Date | Non-clustered | Date range queries |
| IX_TimeEntries_Status | Status | Filtered (IsDeleted=0) | Filter by status |
| IX_TimeEntries_InvoiceId | InvoiceId | Non-clustered | Get time for an invoice |
| IX_TimeEntries_MatterId_Status | MatterId, Status | Composite | Unbilled time per matter |
| IX_TimeEntries_UserId_Date | UserId, Date | Composite | User timesheet view |

### billing.Invoices

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Invoices | Id | Clustered | Primary key |
| UX_Invoices_InvoiceNumber | InvoiceNumber | Unique | Lookup by number |
| IX_Invoices_ClientId | ClientId | Non-clustered | Get invoices for client |
| IX_Invoices_MatterId | MatterId | Non-clustered | Get invoices for matter |
| IX_Invoices_Status | Status | Filtered (IsDeleted=0) | Filter by status |
| IX_Invoices_DueDate | DueDate | Non-clustered | Aged debt queries |
| IX_Invoices_ClientId_Status | ClientId, Status | Composite | Outstanding invoices per client |

### payment.Payments

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Payments | Id | Clustered | Primary key |
| IX_Payments_InvoiceId | InvoiceId | Non-clustered | Get payments for invoice |
| IX_Payments_PaymentDate | PaymentDate | Non-clustered | Date range queries |

### doc.Documents

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Documents | Id | Clustered | Primary key |
| IX_Documents_ClientId | ClientId | Non-clustered | Get documents for client |
| IX_Documents_MatterId | MatterId | Non-clustered | Get documents for matter |
| IX_Documents_UploadedById | UploadedById | Non-clustered | Get documents by uploader |

### audit.AuditEntries

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_AuditEntries | Id | Clustered | Primary key |
| IX_AuditEntries_EntityType_EntityId | EntityType, EntityId | Composite | Audit trail per entity |
| IX_AuditEntries_UserId | UserId | Non-clustered | Activity by user |
| IX_AuditEntries_Timestamp | Timestamp | Non-clustered | Date range queries |
| IX_AuditEntries_CorrelationId | CorrelationId | Non-clustered | Trace a request |

### compliance.ComplianceChecks

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_ComplianceChecks | Id | Clustered | Primary key |
| IX_ComplianceChecks_ClientId | ClientId | Non-clustered | Checks for a client |
| IX_ComplianceChecks_MatterId | MatterId | Non-clustered | Checks for a matter |
| IX_ComplianceChecks_Status | Status | Non-clustered | Pending reviews queue |

### security.Users

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Users | Id | Clustered | Primary key |
| UX_Users_Email | Email | Unique | Login lookup |

### notification.Notifications

| Index Name | Columns | Type | Rationale |
|-----------|---------|------|-----------|
| PK_Notifications | Id | Clustered | Primary key |
| IX_Notifications_UserId_IsRead | UserId, IsRead | Composite | Unread notifications per user |
| IX_Notifications_CreatedAt | CreatedAt | Non-clustered | Recent notifications |

---

## Filtered Index Strategy

Filtered indexes are used for tables with soft-delete to avoid indexing deleted records:

```sql
CREATE INDEX IX_Clients_Status 
ON client.Clients (Status) 
WHERE IsDeleted = 0;
```

This ensures queries filtering active records use smaller, faster indexes.

---

## Index Maintenance

| Activity | Frequency |
|----------|-----------|
| Rebuild fragmented indexes (>30%) | Weekly (off-hours) |
| Reorganize moderately fragmented indexes (10-30%) | Daily (off-hours) |
| Update statistics | Daily |
| Review unused indexes | Monthly |
| Review missing index DMVs | Monthly |

---

## Performance Monitoring Queries

The following DMV queries should be run periodically:

- `sys.dm_db_index_usage_stats` — Identify unused indexes
- `sys.dm_db_missing_index_details` — Identify missing indexes suggested by the query optimizer
- `sys.dm_exec_query_stats` — Identify expensive queries
