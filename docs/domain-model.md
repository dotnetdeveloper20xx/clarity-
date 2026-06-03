# Domain Model

This document defines all entities, their purpose, relationships, and business rules.

---

## Core Entities

### Client

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| ReferenceNumber | string | System-generated, immutable client reference |
| Name | string | Client name (individual or organisation) |
| ClientType | enum | Individual, Organisation |
| Status | enum | Pending, Active, OnHold, Archived |
| Email | string | Primary email |
| Phone | string | Primary phone |
| Address | ValueObject | Postal address |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created the record |
| ModifiedAt | DateTime | UTC last modification timestamp |
| ModifiedBy | Guid | User who last modified |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Has many Matters
- Has many Documents (general, not matter-specific)
- Has many ComplianceChecks
- Has many Contacts

**Business Rules:**
- Cannot have matters until compliance checks pass
- Reference number is immutable after creation
- Soft-delete only

---

### Matter

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| ReferenceNumber | string | System-generated, immutable matter reference |
| ClientId | Guid | FK to Client |
| Title | string | Short description of the matter |
| Description | string | Detailed description |
| MatterType | enum | Conveyancing, Litigation, FamilyLaw, Commercial, Employment, etc. |
| Status | enum | Open, InProgress, OnHold, Closed |
| FeeArrangement | enum | Hourly, FixedFee, Hybrid |
| EstimatedValue | decimal | Estimated value of the matter |
| OpenedDate | DateTime | Date matter was opened |
| ClosedDate | DateTime? | Date matter was closed (nullable) |
| LeadConsultantId | Guid | FK to User (primary fee earner) |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created |
| ModifiedAt | DateTime | UTC last modified |
| ModifiedBy | Guid | User who modified |
| IsDeleted | bool | Soft delete flag |
| RowVersion | byte[] | Optimistic concurrency token |

**Relationships:**
- Belongs to one Client
- Has many TimeEntries
- Has many Documents
- Has many MatterNotes
- Has many Tasks
- Has many Invoices
- Has many MatterTeamMembers
- Has many ComplianceChecks

**Business Rules:**
- Must belong to exactly one client
- Must have at least one assigned consultant
- Cannot be opened against non-compliant clients
- Closed matters are read-only (except admin)
- Status transitions are validated and audited

---

### TimeEntry

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| MatterId | Guid | FK to Matter |
| UserId | Guid | FK to User (fee earner) |
| Date | DateOnly | Date work was performed |
| DurationMinutes | int | Duration in minutes |
| Description | string | Work description |
| IsBillable | bool | Billable vs non-billable |
| BillingRateId | Guid? | FK to BillingRate (if billable) |
| Status | enum | Draft, Approved, Billed |
| ApprovedById | Guid? | FK to User who approved |
| ApprovedAt | DateTime? | When approved |
| InvoiceId | Guid? | FK to Invoice (when billed) |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created |
| ModifiedAt | DateTime | UTC last modified |
| ModifiedBy | Guid | User who modified |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Belongs to one Matter
- Belongs to one User
- Optionally belongs to one Invoice (when billed)
- Optionally linked to one BillingRate

**Business Rules:**
- Must be linked to a matter
- Cannot be recorded against closed matters
- Date cannot be in the future
- Only Team Leaders can approve
- Approved entries cannot be modified without reverting to Draft
- Billed entries are immutable

---

### Invoice

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| InvoiceNumber | string | Sequential, system-generated |
| MatterId | Guid | FK to Matter |
| ClientId | Guid | FK to Client |
| Status | enum | Draft, Issued, PartiallyPaid, Paid, WrittenOff |
| IssueDate | DateOnly | Date invoice was issued |
| DueDate | DateOnly | Payment due date |
| SubTotal | decimal | Total before tax |
| TaxAmount | decimal | VAT/tax amount |
| TotalAmount | decimal | Final amount |
| PaidAmount | decimal | Amount paid so far |
| Notes | string | Optional notes |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created |
| ModifiedAt | DateTime | UTC last modified |
| ModifiedBy | Guid | User who modified |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Belongs to one Matter
- Belongs to one Client
- Has many InvoiceLineItems
- Has many Payments

**Business Rules:**
- Must have at least one line item
- Invoice number is sequential and immutable
- Draft invoices can be edited; issued invoices are immutable
- Only Finance Users can create and issue
- Once time is billed, it cannot be billed again

---

### Payment

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| InvoiceId | Guid | FK to Invoice |
| Amount | decimal | Payment amount |
| PaymentDate | DateOnly | Date payment received |
| PaymentMethod | enum | BankTransfer, Card, Cheque, Cash, Other |
| Reference | string | External payment reference |
| Notes | string | Optional notes |
| IsReversed | bool | Whether payment was reversed |
| ReversedById | Guid? | Who reversed it |
| ReversedAt | DateTime? | When reversed |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who recorded |

**Relationships:**
- Belongs to one Invoice

**Business Rules:**
- Must be linked to an invoice
- Updates invoice status (Partially Paid / Paid)
- Overpayments create credit on client account
- Reversals are audited
- Only Finance Users can record payments

---

### Document

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| ClientId | Guid? | FK to Client (nullable) |
| MatterId | Guid? | FK to Matter (nullable) |
| FileName | string | Original file name |
| ContentType | string | MIME type |
| FileSizeBytes | long | File size |
| StoragePath | string | Path/key in blob storage |
| Version | int | Document version number |
| ParentDocumentId | Guid? | FK to previous version |
| Status | enum | Active, Archived |
| UploadedById | Guid | FK to User who uploaded |
| CreatedAt | DateTime | UTC creation timestamp |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Optionally belongs to a Client
- Optionally belongs to a Matter
- May reference a parent document (previous version)

**Business Rules:**
- Must be linked to a client or matter (not orphaned)
- Soft-delete only
- Versioning maintained on re-upload
- File size limits enforced

---

### MatterNote

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| MatterId | Guid | FK to Matter |
| Content | string | Note content |
| IsClientVisible | bool | Whether client can see this note |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created |
| ModifiedAt | DateTime | UTC last modified |
| ModifiedBy | Guid | User who modified |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Belongs to one Matter

**Business Rules:**
- Internal notes never visible to clients
- Client-visible notes are clearly marked
- Cannot be permanently deleted (soft-delete only)

---

### MatterTask

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| MatterId | Guid | FK to Matter |
| Title | string | Task title |
| Description | string | Task description |
| AssigneeId | Guid | FK to User assigned |
| Status | enum | ToDo, InProgress, Blocked, Complete, Cancelled |
| Priority | enum | Low, Medium, High, Urgent |
| DueDate | DateOnly | Due date |
| CompletedAt | DateTime? | When completed |
| CreatedAt | DateTime | UTC creation timestamp |
| CreatedBy | Guid | User who created |
| ModifiedAt | DateTime | UTC last modified |
| ModifiedBy | Guid | User who modified |
| IsDeleted | bool | Soft delete flag |

**Relationships:**
- Belongs to one Matter
- Assigned to one User

**Business Rules:**
- Must have an assignee and due date
- Overdue tasks are flagged
- Completion may be required before matter closure

---

### ComplianceCheck

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| ClientId | Guid | FK to Client |
| MatterId | Guid? | FK to Matter (optional) |
| CheckType | enum | AML, KYC, ConflictOfInterest, RiskAssessment |
| Status | enum | Pending, Pass, Fail, RequiresInvestigation |
| PerformedById | Guid | FK to Compliance Officer |
| PerformedAt | DateTime | When check was completed |
| Notes | string | Officer's notes |
| CreatedAt | DateTime | UTC creation timestamp |

**Relationships:**
- Belongs to one Client
- Optionally linked to a Matter

**Business Rules:**
- Mandatory for new clients
- Cannot be modified after completion
- Failed checks block matter progression
- Only Compliance Officers can perform checks
- History permanently retained

---

### User

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| Email | string | Login email |
| FirstName | string | First name |
| LastName | string | Last name |
| IsActive | bool | Whether account is active |
| LastLoginAt | DateTime? | Last successful login |
| CreatedAt | DateTime | UTC creation timestamp |
| ModifiedAt | DateTime | UTC last modified |

**Relationships:**
- Has many UserRoles
- Has many TimeEntries
- Assigned to many Matters (via MatterTeamMember)

---

### Role

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| Name | string | Role name |
| Description | string | Role description |

**Relationships:**
- Has many Permissions (via RolePermission)
- Assigned to many Users (via UserRole)

---

### Permission

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| Name | string | Permission name (e.g., matter.create, time.approve) |
| Description | string | What this permission allows |
| Category | string | Grouping (Client, Matter, Billing, etc.) |

---

### AuditEntry

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| Timestamp | DateTime | When the action occurred (UTC) |
| UserId | Guid | Who performed the action |
| Action | string | What was done (Created, Updated, Deleted, StatusChanged, etc.) |
| EntityType | string | Type of entity affected |
| EntityId | Guid | ID of entity affected |
| OldValues | string (JSON) | Previous state |
| NewValues | string (JSON) | New state |
| CorrelationId | string | Request correlation ID |
| IpAddress | string | Client IP address |

**Business Rules:**
- Append-only (never modified or deleted)
- Retained for minimum 7 years
- Searchable by user, entity, date range, action

---

### Notification

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| UserId | Guid | FK to recipient User |
| Title | string | Notification title |
| Message | string | Notification body |
| Type | enum | Info, Warning, Action, Reminder |
| IsRead | bool | Whether user has read it |
| EntityType | string? | Related entity type |
| EntityId | Guid? | Related entity ID |
| CreatedAt | DateTime | UTC creation timestamp |

---

### BillingRate

| Property | Type | Description |
|----------|------|-------------|
| Id | Guid | Unique identifier |
| Name | string | Rate name (e.g., "Senior Solicitor", "Trainee") |
| HourlyRate | decimal | Amount per hour |
| EffectiveFrom | DateOnly | When this rate starts |
| EffectiveTo | DateOnly? | When this rate ends (null = current) |
| IsActive | bool | Whether currently active |

---

## Aggregate Roots

Aggregate roots are the entry points for modifying a cluster of related entities:

| Aggregate Root | Owned Entities |
|----------------|---------------|
| **Client** | Contacts, ClientDocuments |
| **Matter** | MatterNotes, MatterTasks, MatterTeamMembers, MatterDocuments |
| **Invoice** | InvoiceLineItems |
| **User** | UserRoles |

Rules for aggregates:
- External entities reference the aggregate root by ID only
- Modifications to owned entities go through the aggregate root
- Consistency is enforced within the aggregate boundary
