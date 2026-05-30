
# Phase 3 — Database Design and Data Architecture

This is a very important phase.

For a Halo-style legal platform, the database is not just a technical storage area.

The database is the memory of the business.

It stores:

clients, matters, documents, time records, invoices, payments, audit history, compliance checks, reporting data, and operational decisions.

So in this phase, we design the data carefully before writing APIs or Angular screens.

---

# Goal Of Phase 3

Design a production-grade SQL Server database for the legal practice management platform.

By the end of this phase, we should know:

what tables exist, how they relate, what indexes are needed, what data must be audited, how soft delete works, how reporting will work, how historical records are preserved, and how performance will be protected.

This phase is still mostly design.

We may define database scripts or migration plans, but we are not yet building the full application.

---

# Step 1 — Identify The Core Data Areas

Start from the domains we identified earlier.

The major data areas are:

Client data
Matter data
Document data
Time recording data
Billing data
Payment data
Compliance data
Security data
Audit data
Reporting data
Notification data

Each area needs careful ownership.

For example, client information belongs to the Client domain.

Matter information belongs to the Matter domain.

Invoice information belongs to the Billing domain.

Payment information belongs to the Payment domain.

This prevents confusion later.

---

# Step 2 — Design The Client Tables

Start with clients because most legal work begins with a client.

Possible tables:

Clients
ClientContacts
ClientAddresses
ClientRelationships
ClientCommunicationPreferences

A client might be an individual or an organisation.

So we need to support both.

A person client may have date of birth.

A company client may have company number.

Both may have phone, email, address, and communication preferences.

Important rule:

Do not design the client table only for today’s needs.

Legal systems grow over time.

Future requirements may include anti-money laundering checks, identity verification, conflict checks, and client risk scores.

---

# Step 3 — Design The Matter Tables

Matter Management is the heart of the legal platform.

A matter represents a legal case, transaction, or instruction.

Possible tables:

Matters
MatterTypes
MatterStatuses
MatterNotes
MatterTasks
MatterParticipants
MatterHistory
MatterMilestones

A matter belongs to a client.

A matter may have one lead consultant or solicitor.

A matter may have assistants.

A matter has status.

Example statuses:

Open
On Hold
Awaiting Client
Awaiting Third Party
Billing Review
Closed
Archived

Matter history is very important.

Every major matter change should be traceable.

---

# Step 4 — Design Document Tables

Legal firms are document-heavy.

Documents are central to legal work.

Possible tables:

Documents
DocumentVersions
DocumentCategories
DocumentMatterLinks
DocumentClientLinks
DocumentAccessLogs

Important design point:

Usually the database should not store large document binary content directly unless there is a strong reason.

Better approach:

Store the file in storage.

Store metadata in SQL.

For local development, storage can be a local folder.

For Azure, storage can become Azure Blob Storage.

The database stores:

file name, storage path, content type, size, uploaded by, uploaded date, matter id, version number, security level.

---

# Step 5 — Design Time Recording Tables

Time recording is one of the most important legal business features.

Possible tables:

TimeEntries
TimeEntryTypes
TimeRates
TimeCategories
TimeApprovalHistory

A time entry should include:

matter id, user id, date, duration, description, billable flag, hourly rate, status.

Example statuses:

Draft
Submitted
Approved
Rejected
Billed
WrittenOff

Important rule:

Once a time entry has been billed, it should not be casually changed.

If changes are needed, they should be audited carefully.

---

# Step 6 — Design Billing Tables

Billing is sensitive because money is involved.

Possible tables:

Invoices
InvoiceLines
InvoiceStatuses
CreditNotes
BillingRuns
BillingAdjustments

An invoice should include:

client id, matter id, invoice number, invoice date, due date, subtotal, VAT, total, status.

Invoice line items may include:

time entries, disbursements, fixed fees, adjustments.

Important rule:

Financial records should be treated as historical truth.

Do not overwrite important financial values without audit.

---

# Step 7 — Design Payment Tables

Payments may come from bank transfer, card payment, cheque, or internal allocation.

Possible tables:

Payments
PaymentAllocations
PaymentMethods
PaymentStatuses
Refunds

Payment allocation is important.

One payment may pay one invoice.

Or one payment may be split across multiple invoices.

So do not simply store Payment.InvoiceId if the business may need flexible allocation.

Use a PaymentAllocations table.

---

# Step 8 — Design Compliance Tables

Legal systems need strong compliance support.

Possible tables:

ComplianceChecks
AmlChecks
ConflictChecks
RiskAssessments
ComplianceReviewNotes
ComplianceFlags

Compliance records should capture:

who reviewed, when reviewed, result, risk level, notes, supporting documents, next review date.

Example risk levels:

Low
Medium
High
Critical

Important rule:

Compliance data must be audit-friendly.

The firm may need to explain decisions later.

---

# Step 9 — Design Security Tables

If using ASP.NET Identity, many identity tables are generated.

But we still need business-level security.

Possible tables:

Users
Roles
Permissions
UserRoles
RolePermissions
UserMatterAccess
UserClientAccess

Matter-level permissions may be important.

Not every user should see every matter.

Especially in legal work, privacy and confidentiality matter.

---

# Step 10 — Design Audit Tables

Audit is non-negotiable in corporate legal software.

Possible tables:

AuditLogs
AuditLogDetails
SecurityAuditLogs
DataAccessLogs

Audit record should capture:

who did it, what they did, when they did it, what entity changed, old value, new value, correlation id, IP address if available.

Examples:

Matter status changed.

Invoice marked as paid.

Document downloaded.

Client details updated.

User permission changed.

Audit logs should be append-only.

Do not update audit records casually.

---

# Step 11 — Soft Delete Strategy

In legal and financial systems, hard delete is dangerous.

Instead, use soft delete.

Common fields:

IsDeleted
DeletedAt
DeletedBy

This means the record is hidden from normal screens but still exists for history, audit, and recovery.

Important rule:

Some records should never be deleted at all, only archived.

Examples:

Invoices, payments, audit logs, compliance decisions.

---

# Step 12 — Indexing Strategy

Indexes help SQL Server find data quickly.

Without indexes, large systems become slow.

Start with indexes on:

Client email
Client name
Matter number
Matter status
Matter client id
Invoice number
Invoice status
Payment date
Document matter id
Time entry matter id
Audit entity id
Audit created date

But remember:

Indexes improve reads.

They can slow writes.

So we design indexes based on real query patterns.

---

# Step 13 — Reporting Strategy

Reporting queries can become heavy.

Dashboards may ask:

open matters by consultant, billed time this month, unpaid invoices, average matter duration, compliance flags, documents uploaded this week.

Do not let reporting destroy transactional performance.

Options:

optimized read queries, reporting views, indexed views where appropriate, summary tables, background aggregation.

For a first version, we can use optimized SQL views or read models.

---

# Step 14 — Data Retention And Archiving

Legal systems hold data for years.

But not all data should stay active forever.

Design archiving strategy:

closed matters older than X years may move to archived status.

old audit logs may move to archive tables.

documents may move to cheaper storage.

But retention must follow legal and business rules.

---

# Step 15 — Migration Strategy

Use EF Core migrations for schema evolution.

But for a Halo-style system, also document SQL changes carefully.

Migration rules:

never casually change production schema, review migrations, test on realistic data, plan rollback, avoid destructive changes, version scripts, keep database backups.

This protects the business.

---

# Step 16 — Seed Data Strategy

Create realistic local demo data.

Seed:

clients, matters, users, roles, documents, time entries, invoices, payments, compliance checks, audit logs.

This helps developers test dashboards and performance locally.

Do not seed tiny unrealistic data only.

Have enough data to expose performance issues.

---

# Step 17 — Data Documentation

Create these documents:

```text
docs/database-design.md
docs/entity-relationship-overview.md
docs/table-catalog.md
docs/indexing-strategy.md
docs/audit-strategy.md
docs/soft-delete-strategy.md
docs/reporting-data-strategy.md
docs/data-retention-policy.md
docs/migration-strategy.md
```

---

# Phase 3 Deliverables

By the end of Phase 3, we should have:

defined tables, relationships, keys, indexes, audit strategy, soft delete strategy, reporting strategy, retention strategy, migration strategy, and seed data plan.

Still no full APIs.

Still no Angular screens.

This phase prepares the data foundation.

---

# AI Prompt For Phase 3

Use this:

> Complete Phase 3 only. Design the SQL Server data architecture for the enterprise legal practice management platform. Define tables, relationships, keys, indexes, audit strategy, soft delete strategy, reporting strategy, retention strategy, migration strategy, and seed data plan. Produce detailed database documentation and explain every decision in plain English for junior developers. Do not build APIs or frontend yet.
