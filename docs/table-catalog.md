# Table Catalog

This document provides a complete catalog of all database tables, their columns, data types, and constraints.

---

## Client Domain

### client.Clients

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| ReferenceNumber | NVARCHAR(20) | NO | | Unique client reference |
| Name | NVARCHAR(200) | NO | | Client full name or organisation name |
| ClientType | INT | NO | | 0=Individual, 1=Organisation |
| Status | INT | NO | 0 | 0=Pending, 1=Active, 2=OnHold, 3=Archived |
| Email | NVARCHAR(256) | YES | | Primary email address |
| Phone | NVARCHAR(50) | YES | | Primary phone number |
| AddressLine1 | NVARCHAR(200) | YES | | Address line 1 |
| AddressLine2 | NVARCHAR(200) | YES | | Address line 2 |
| City | NVARCHAR(100) | YES | | City |
| County | NVARCHAR(100) | YES | | County/State |
| PostCode | NVARCHAR(20) | YES | | Postal code |
| Country | NVARCHAR(100) | YES | | Country |
| CompanyNumber | NVARCHAR(50) | YES | | Company registration number (organisations only) |
| DateOfBirth | DATE | YES | | Date of birth (individuals only) |
| Notes | NVARCHAR(MAX) | YES | | General notes |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who last modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |
| DeletedAt | DATETIME2 | YES | | When soft deleted |
| DeletedBy | UNIQUEIDENTIFIER | YES | | Who soft deleted |
| RowVersion | ROWVERSION | NO | | Concurrency token |

### client.ClientContacts

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| ClientId | UNIQUEIDENTIFIER | NO | | FK to Clients |
| Name | NVARCHAR(200) | NO | | Contact name |
| Email | NVARCHAR(256) | YES | | Contact email |
| Phone | NVARCHAR(50) | YES | | Contact phone |
| Role | NVARCHAR(100) | YES | | Contact role (e.g., Director, Accounts) |
| IsPrimary | BIT | NO | 0 | Primary contact flag |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| IsDeleted | BIT | NO | 0 | Soft delete flag |

---

## Matter Domain

### matter.Matters

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| ReferenceNumber | NVARCHAR(20) | NO | | Unique matter reference |
| ClientId | UNIQUEIDENTIFIER | NO | | FK to Clients |
| Title | NVARCHAR(300) | NO | | Short title |
| Description | NVARCHAR(MAX) | YES | | Detailed description |
| MatterTypeId | UNIQUEIDENTIFIER | NO | | FK to MatterTypes |
| Status | INT | NO | 0 | 0=Open, 1=InProgress, 2=OnHold, 3=Closed |
| FeeArrangement | INT | NO | 0 | 0=Hourly, 1=FixedFee, 2=Hybrid |
| EstimatedValue | DECIMAL(18,2) | YES | | Estimated matter value |
| FixedFeeAmount | DECIMAL(18,2) | YES | | Fixed fee if applicable |
| OpenedDate | DATE | NO | | Date matter was opened |
| ClosedDate | DATE | YES | | Date matter was closed |
| LeadConsultantId | UNIQUEIDENTIFIER | NO | | FK to Users |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who last modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |
| DeletedAt | DATETIME2 | YES | | When soft deleted |
| DeletedBy | UNIQUEIDENTIFIER | YES | | Who soft deleted |
| RowVersion | ROWVERSION | NO | | Concurrency token |

### matter.MatterTypes

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Name | NVARCHAR(100) | NO | | Type name (e.g., Conveyancing, Litigation) |
| Description | NVARCHAR(500) | YES | | Description |
| IsActive | BIT | NO | 1 | Whether type is currently available |

### matter.MatterNotes

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| Content | NVARCHAR(MAX) | NO | | Note content |
| IsClientVisible | BIT | NO | 0 | Visible to client |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |

### matter.MatterTasks

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| Title | NVARCHAR(300) | NO | | Task title |
| Description | NVARCHAR(MAX) | YES | | Task description |
| AssigneeId | UNIQUEIDENTIFIER | NO | | FK to Users |
| Status | INT | NO | 0 | 0=ToDo, 1=InProgress, 2=Blocked, 3=Complete, 4=Cancelled |
| Priority | INT | NO | 1 | 0=Low, 1=Medium, 2=High, 3=Urgent |
| DueDate | DATE | NO | | Due date |
| CompletedAt | DATETIME2 | YES | | When completed |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |

### matter.MatterTeamMembers

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| UserId | UNIQUEIDENTIFIER | NO | | FK to Users |
| Role | NVARCHAR(100) | YES | | Team role (Lead, Assistant, etc.) |
| AssignedAt | DATETIME2 | NO | GETUTCDATE() | When assigned |
| AssignedBy | UNIQUEIDENTIFIER | NO | | Who assigned |

---

## Document Domain

### doc.Documents

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| ClientId | UNIQUEIDENTIFIER | YES | | FK to Clients (nullable) |
| MatterId | UNIQUEIDENTIFIER | YES | | FK to Matters (nullable) |
| FileName | NVARCHAR(500) | NO | | Original file name |
| ContentType | NVARCHAR(100) | NO | | MIME type |
| FileSizeBytes | BIGINT | NO | | File size in bytes |
| StoragePath | NVARCHAR(1000) | NO | | Path in blob/file storage |
| Version | INT | NO | 1 | Document version |
| ParentDocumentId | UNIQUEIDENTIFIER | YES | | FK to previous version |
| Category | NVARCHAR(100) | YES | | Document category |
| Status | INT | NO | 0 | 0=Active, 1=Archived |
| UploadedById | UNIQUEIDENTIFIER | NO | | FK to Users |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| IsDeleted | BIT | NO | 0 | Soft delete flag |
| DeletedAt | DATETIME2 | YES | | When soft deleted |
| DeletedBy | UNIQUEIDENTIFIER | YES | | Who soft deleted |

---

## Time Recording Domain

### time.TimeEntries

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| UserId | UNIQUEIDENTIFIER | NO | | FK to Users (fee earner) |
| Date | DATE | NO | | Date work performed |
| DurationMinutes | INT | NO | | Duration in minutes |
| Description | NVARCHAR(1000) | NO | | Work description |
| IsBillable | BIT | NO | 1 | Billable flag |
| BillingRateId | UNIQUEIDENTIFIER | YES | | FK to BillingRates |
| RateAmount | DECIMAL(18,2) | YES | | Rate at time of entry (snapshot) |
| Status | INT | NO | 0 | 0=Draft, 1=Approved, 2=Billed |
| ApprovedById | UNIQUEIDENTIFIER | YES | | FK to User who approved |
| ApprovedAt | DATETIME2 | YES | | When approved |
| InvoiceId | UNIQUEIDENTIFIER | YES | | FK to Invoice (when billed) |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |
| RowVersion | ROWVERSION | NO | | Concurrency token |

### time.BillingRates

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Name | NVARCHAR(100) | NO | | Rate name (e.g., Senior Solicitor) |
| HourlyRate | DECIMAL(18,2) | NO | | Amount per hour |
| EffectiveFrom | DATE | NO | | Start date |
| EffectiveTo | DATE | YES | | End date (null = current) |
| IsActive | BIT | NO | 1 | Currently active |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |

---

## Billing Domain

### billing.Invoices

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| InvoiceNumber | NVARCHAR(20) | NO | | Sequential invoice number |
| ClientId | UNIQUEIDENTIFIER | NO | | FK to Clients |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| Status | INT | NO | 0 | 0=Draft, 1=Issued, 2=PartiallyPaid, 3=Paid, 4=WrittenOff |
| IssueDate | DATE | YES | | Date issued |
| DueDate | DATE | YES | | Payment due date |
| SubTotal | DECIMAL(18,2) | NO | 0 | Total before tax |
| TaxRate | DECIMAL(5,2) | NO | 20.00 | Tax rate percentage |
| TaxAmount | DECIMAL(18,2) | NO | 0 | Tax amount |
| TotalAmount | DECIMAL(18,2) | NO | 0 | Total including tax |
| PaidAmount | DECIMAL(18,2) | NO | 0 | Amount paid so far |
| Notes | NVARCHAR(MAX) | YES | | Invoice notes |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |
| ModifiedBy | UNIQUEIDENTIFIER | YES | | User who modified |
| IsDeleted | BIT | NO | 0 | Soft delete flag |
| RowVersion | ROWVERSION | NO | | Concurrency token |

### billing.InvoiceLineItems

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| InvoiceId | UNIQUEIDENTIFIER | NO | | FK to Invoices |
| Description | NVARCHAR(500) | NO | | Line item description |
| Quantity | DECIMAL(10,2) | NO | | Quantity (hours or units) |
| UnitPrice | DECIMAL(18,2) | NO | | Price per unit |
| Amount | DECIMAL(18,2) | NO | | Line total |
| TimeEntryId | UNIQUEIDENTIFIER | YES | | FK to TimeEntry (if from time) |
| LineType | INT | NO | 0 | 0=Time, 1=Disbursement, 2=FixedFee, 3=Adjustment |
| SortOrder | INT | NO | 0 | Display order |

### billing.Disbursements

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| MatterId | UNIQUEIDENTIFIER | NO | | FK to Matters |
| Description | NVARCHAR(500) | NO | | What the cost was for |
| Amount | DECIMAL(18,2) | NO | | Cost amount |
| Date | DATE | NO | | Date incurred |
| IsBilled | BIT | NO | 0 | Whether included on an invoice |
| InvoiceId | UNIQUEIDENTIFIER | YES | | FK to Invoice (when billed) |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |
| IsDeleted | BIT | NO | 0 | Soft delete flag |

---

## Payment Domain

### payment.Payments

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| InvoiceId | UNIQUEIDENTIFIER | NO | | FK to Invoices |
| Amount | DECIMAL(18,2) | NO | | Payment amount |
| PaymentDate | DATE | NO | | Date received |
| PaymentMethod | INT | NO | | 0=BankTransfer, 1=Card, 2=Cheque, 3=Cash, 4=Other |
| Reference | NVARCHAR(200) | YES | | External reference |
| Notes | NVARCHAR(500) | YES | | Payment notes |
| IsReversed | BIT | NO | 0 | Reversed flag |
| ReversedById | UNIQUEIDENTIFIER | YES | | Who reversed |
| ReversedAt | DATETIME2 | YES | | When reversed |
| ReversalReason | NVARCHAR(500) | YES | | Why reversed |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who recorded |

---

## Compliance Domain

### compliance.ComplianceChecks

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| ClientId | UNIQUEIDENTIFIER | NO | | FK to Clients |
| MatterId | UNIQUEIDENTIFIER | YES | | FK to Matters (optional) |
| CheckType | INT | NO | | 0=AML, 1=KYC, 2=ConflictOfInterest, 3=RiskAssessment |
| Status | INT | NO | 0 | 0=Pending, 1=Pass, 2=Fail, 3=RequiresInvestigation |
| RiskLevel | INT | YES | | 0=Low, 1=Medium, 2=High, 3=Critical |
| PerformedById | UNIQUEIDENTIFIER | YES | | FK to User |
| PerformedAt | DATETIME2 | YES | | When completed |
| Notes | NVARCHAR(MAX) | YES | | Officer notes |
| NextReviewDate | DATE | YES | | When next review is due |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| CreatedBy | UNIQUEIDENTIFIER | NO | | User who created |

---

## Security Domain

### security.Users

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Email | NVARCHAR(256) | NO | | Login email (unique) |
| PasswordHash | NVARCHAR(MAX) | NO | | Hashed password |
| FirstName | NVARCHAR(100) | NO | | First name |
| LastName | NVARCHAR(100) | NO | | Last name |
| IsActive | BIT | NO | 1 | Account active |
| IsLockedOut | BIT | NO | 0 | Locked out |
| LockoutEnd | DATETIME2 | YES | | Lockout expiry |
| FailedLoginAttempts | INT | NO | 0 | Failed login counter |
| LastLoginAt | DATETIME2 | YES | | Last successful login |
| PasswordChangedAt | DATETIME2 | YES | | Last password change |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
| ModifiedAt | DATETIME2 | YES | | UTC last modification |

### security.Roles

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Name | NVARCHAR(100) | NO | | Role name (unique) |
| Description | NVARCHAR(500) | YES | | Role description |
| IsSystemRole | BIT | NO | 0 | System-defined (cannot be deleted) |

### security.Permissions

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Name | NVARCHAR(100) | NO | | Permission name (unique) |
| Description | NVARCHAR(500) | YES | | Description |
| Category | NVARCHAR(50) | NO | | Grouping (Client, Matter, etc.) |

### security.UserRoles

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| UserId | UNIQUEIDENTIFIER | NO | | FK to Users |
| RoleId | UNIQUEIDENTIFIER | NO | | FK to Roles |
| AssignedAt | DATETIME2 | NO | GETUTCDATE() | When assigned |
| AssignedBy | UNIQUEIDENTIFIER | NO | | Who assigned |

### security.RolePermissions

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| RoleId | UNIQUEIDENTIFIER | NO | | FK to Roles |
| PermissionId | UNIQUEIDENTIFIER | NO | | FK to Permissions |

---

## Audit Domain

### audit.AuditEntries

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| Timestamp | DATETIME2 | NO | GETUTCDATE() | When action occurred (UTC) |
| UserId | UNIQUEIDENTIFIER | NO | | Who performed action |
| UserEmail | NVARCHAR(256) | NO | | Denormalized for query performance |
| Action | NVARCHAR(50) | NO | | Created, Updated, Deleted, StatusChanged, etc. |
| EntityType | NVARCHAR(100) | NO | | Type of entity |
| EntityId | UNIQUEIDENTIFIER | NO | | ID of affected entity |
| OldValues | NVARCHAR(MAX) | YES | | JSON of previous state |
| NewValues | NVARCHAR(MAX) | YES | | JSON of new state |
| CorrelationId | NVARCHAR(100) | YES | | Request correlation ID |
| IpAddress | NVARCHAR(50) | YES | | Client IP |
| UserAgent | NVARCHAR(500) | YES | | Browser/client info |

---

## Notification Domain

### notification.Notifications

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| Id | UNIQUEIDENTIFIER | NO | NEWSEQUENTIALID() | Primary key |
| UserId | UNIQUEIDENTIFIER | NO | | FK to Users (recipient) |
| Title | NVARCHAR(200) | NO | | Notification title |
| Message | NVARCHAR(1000) | NO | | Notification body |
| Type | INT | NO | 0 | 0=Info, 1=Warning, 2=Action, 3=Reminder |
| IsRead | BIT | NO | 0 | Read flag |
| ReadAt | DATETIME2 | YES | | When read |
| EntityType | NVARCHAR(100) | YES | | Related entity type |
| EntityId | UNIQUEIDENTIFIER | YES | | Related entity ID |
| CreatedAt | DATETIME2 | NO | GETUTCDATE() | UTC creation timestamp |
