# CQRS Strategy

## Overview

Clarity uses **Command Query Responsibility Segregation (CQRS)** to separate write operations (commands) from read operations (queries). This provides:

- Clear separation of intent (reading vs writing)
- Simplified handlers with single responsibility
- Easy addition of cross-cutting concerns (validation, logging, auditing)
- Performance optimisation opportunities (read-optimised queries)
- Better testability (each handler tested in isolation)

## Implementation

CQRS is implemented using **MediatR** with pipeline behaviours for cross-cutting concerns.

### Pipeline

```
Request → Logging Behaviour → Validation Behaviour → Authorization Behaviour → Handler → Response
```

### Pipeline Behaviours (in order)

1. **LoggingBehaviour** — Logs every request and response time
2. **ValidationBehaviour** — Runs FluentValidation rules; rejects invalid requests
3. **AuthorisationBehaviour** — Checks user permissions for the operation
4. **AuditBehaviour** — Records audit entries for commands (writes)
5. **PerformanceBehaviour** — Logs slow-running requests for investigation

---

## Commands (Write Operations)

Commands modify state. They return either a result or nothing.

### Naming Convention
`{Verb}{Entity}Command` → e.g., `CreateMatterCommand`, `ApproveTimeEntryCommand`

### Client Domain Commands

| Command | Description |
|---------|-------------|
| CreateClientCommand | Create a new client record |
| UpdateClientCommand | Update client details |
| ArchiveClientCommand | Soft-delete a client |
| ActivateClientCommand | Set client status to Active |
| PlaceClientOnHoldCommand | Place client on hold |

### Matter Domain Commands

| Command | Description |
|---------|-------------|
| CreateMatterCommand | Open a new matter |
| UpdateMatterCommand | Update matter details |
| CloseMatterCommand | Close a completed matter |
| ReopenMatterCommand | Reopen a closed matter (admin) |
| AssignTeamMemberCommand | Add team member to matter |
| RemoveTeamMemberCommand | Remove team member from matter |
| CreateMatterNoteCommand | Add a note to a matter |
| CreateMatterTaskCommand | Create a task on a matter |
| UpdateTaskStatusCommand | Change task status |

### Time Recording Commands

| Command | Description |
|---------|-------------|
| RecordTimeCommand | Create a new time entry |
| UpdateTimeEntryCommand | Modify a draft time entry |
| ApproveTimeEntryCommand | Approve a time entry (Team Leader) |
| RevertTimeEntryCommand | Revert approved entry to draft |
| DeleteTimeEntryCommand | Soft-delete a time entry |

### Billing Commands

| Command | Description |
|---------|-------------|
| GenerateInvoiceCommand | Create invoice from approved time |
| UpdateDraftInvoiceCommand | Modify a draft invoice |
| IssueInvoiceCommand | Finalise and issue an invoice |
| CreateCreditNoteCommand | Issue credit against an invoice |
| WriteOffInvoiceCommand | Write off an unpaid invoice |

### Payment Commands

| Command | Description |
|---------|-------------|
| RecordPaymentCommand | Record a payment received |
| ReversePaymentCommand | Reverse a recorded payment |
| AllocatePaymentCommand | Allocate payment to invoices |

### Document Commands

| Command | Description |
|---------|-------------|
| UploadDocumentCommand | Upload a new document |
| ArchiveDocumentCommand | Soft-delete a document |
| UpdateDocumentMetadataCommand | Update document metadata |

### Compliance Commands

| Command | Description |
|---------|-------------|
| CreateComplianceCheckCommand | Record a compliance check |
| CompleteComplianceCheckCommand | Complete check with result |

### Security Commands

| Command | Description |
|---------|-------------|
| CreateUserCommand | Create a new user account |
| UpdateUserCommand | Update user details |
| DisableUserCommand | Disable a user account |
| AssignRoleCommand | Assign role to user |
| RevokeRoleCommand | Remove role from user |

---

## Queries (Read Operations)

Queries retrieve data without modifying state. They always return a result.

### Naming Convention
`Get{Entity/Entities}Query` → e.g., `GetMatterQuery`, `GetClientMattersQuery`

### Client Domain Queries

| Query | Description |
|-------|-------------|
| GetClientQuery | Get single client by ID |
| GetClientsQuery | Get paginated list of clients |
| SearchClientsQuery | Search clients by criteria |
| GetClientMattersQuery | Get matters for a client |
| GetClientDocumentsQuery | Get documents for a client |

### Matter Domain Queries

| Query | Description |
|-------|-------------|
| GetMatterQuery | Get single matter by ID |
| GetMattersQuery | Get paginated list of matters |
| GetMatterNotesQuery | Get notes for a matter |
| GetMatterTasksQuery | Get tasks for a matter |
| GetMatterTeamQuery | Get team members on a matter |
| GetMatterTimelineQuery | Get matter activity timeline |
| GetMyMattersQuery | Get current user's assigned matters |

### Time Recording Queries

| Query | Description |
|-------|-------------|
| GetTimeEntriesQuery | Get time entries (with filters) |
| GetMyTimeEntriesQuery | Get current user's time entries |
| GetUnbilledTimeQuery | Get approved unbilled time for a matter |
| GetTimeSummaryQuery | Get time summary for reporting |

### Billing Queries

| Query | Description |
|-------|-------------|
| GetInvoiceQuery | Get single invoice by ID |
| GetInvoicesQuery | Get paginated invoices |
| GetClientInvoicesQuery | Get invoices for a client |
| GetMatterInvoicesQuery | Get invoices for a matter |
| GetOutstandingInvoicesQuery | Get unpaid invoices |
| GetBillingRatesQuery | Get active billing rates |

### Payment Queries

| Query | Description |
|-------|-------------|
| GetPaymentsQuery | Get paginated payments |
| GetInvoicePaymentsQuery | Get payments for an invoice |
| GetClientBalanceQuery | Get client account balance |

### Document Queries

| Query | Description |
|-------|-------------|
| GetDocumentQuery | Get document metadata |
| GetMatterDocumentsQuery | Get documents for a matter |
| DownloadDocumentQuery | Get document download URL |

### Compliance Queries

| Query | Description |
|-------|-------------|
| GetComplianceChecksQuery | Get compliance checks for client |
| GetPendingComplianceQuery | Get pending compliance reviews |
| GetComplianceReportQuery | Compliance summary report |

### Reporting Queries

| Query | Description |
|-------|-------------|
| GetDashboardQuery | Get user-specific dashboard data |
| GetFinancialSummaryQuery | Financial overview |
| GetTeamWorkloadQuery | Team workload report |
| GetAgedDebtQuery | Aged debt report |
| GetWipReportQuery | Work in progress report |
| GetProductivityReportQuery | Fee earner productivity |

### Audit Queries

| Query | Description |
|-------|-------------|
| GetAuditHistoryQuery | Get audit entries with filters |
| GetEntityAuditQuery | Get audit trail for a specific entity |

---

## Validation Strategy

Every command has a corresponding validator:

```
CreateMatterCommand → CreateMatterCommandValidator
```

Validators run automatically via the ValidationBehaviour pipeline. If validation fails, the request is rejected before reaching the handler.

---

## Response Pattern

All handlers return a consistent result type:

- **Commands**: Return `Result<TResponse>` or `Result` (success/failure with error details)
- **Queries**: Return `Result<TResponse>` with the requested data or error

This provides uniform error handling across the application.
