# Business Rules

This document captures the core business rules that govern how the Clarity platform operates. These rules must be enforced by the application at all times.

---

## Client Rules

| # | Rule |
|---|------|
| C1 | Every client must have a unique, system-generated reference number. |
| C2 | A client cannot have matters opened until compliance checks have passed. |
| C3 | Client records are never physically deleted; they are soft-deleted (archived). |
| C4 | A client can be an individual or an organisation. |
| C5 | Client contact details must include at least one form of contact (email, phone, or address). |
| C6 | Only System Administrators can permanently archive a client. |
| C7 | All changes to client records are audited. |

---

## Matter Rules

| # | Rule |
|---|------|
| M1 | A matter must belong to exactly one client. |
| M2 | A matter must have at least one assigned consultant. |
| M3 | Every matter must have a unique, system-generated reference number. |
| M4 | Matters cannot be opened against clients with failed or pending compliance checks. |
| M5 | Matter status transitions follow a defined lifecycle: Open → In Progress → On Hold → Closed. |
| M6 | Closed matters become read-only except for System Administrators. |
| M7 | Matters cannot be closed with unbilled approved time entries (without explicit write-off). |
| M8 | Matters cannot be closed with unpaid invoices (without explicit write-off). |
| M9 | All matter status changes are audited. |
| M10 | Reopening a closed matter requires administrator approval and is audited. |

---

## Time Recording Rules

| # | Rule |
|---|------|
| T1 | Time entries must be linked to a matter. |
| T2 | Time entries must specify whether they are billable or non-billable. |
| T3 | Billable time entries require a valid billing rate. |
| T4 | Time cannot be recorded against closed matters. |
| T5 | Time entry dates cannot be in the future. |
| T6 | Maximum daily hours may be configured and enforced per user. |
| T7 | Time entries have a lifecycle: Draft → Approved → Billed. |
| T8 | Only Team Leaders can approve time entries. |
| T9 | Approved time entries cannot be modified without reverting to Draft (audited). |
| T10 | Non-billable time entries do not appear on invoices. |

---

## Billing Rules

| # | Rule |
|---|------|
| B1 | Only approved time entries can be included on invoices. |
| B2 | An invoice must have at least one line item. |
| B3 | Invoice numbers are sequential and system-generated. |
| B4 | Draft invoices can be edited; issued invoices are immutable. |
| B5 | VAT/tax is calculated automatically based on configuration. |
| B6 | Only Finance Users can generate and issue invoices. |
| B7 | Once time entries are billed, they cannot be billed again. |
| B8 | Credit notes can be issued against invoices (audited). |
| B9 | Write-offs must be explicitly recorded with a reason (audited). |

---

## Payment Rules

| # | Rule |
|---|------|
| P1 | Payments must be linked to at least one invoice. |
| P2 | Payments update invoice status: Partially Paid or Paid. |
| P3 | Overpayments create a credit balance on the client account. |
| P4 | Payment method must be recorded (bank transfer, card, cheque, etc.). |
| P5 | Payment reversals require Finance User action and are audited. |
| P6 | Only Finance Users can record or reverse payments. |

---

## Document Rules

| # | Rule |
|---|------|
| D1 | Documents must be linked to a client or a matter. |
| D2 | Documents cannot exist as orphans (unlinked). |
| D3 | Document deletion is always soft-delete (archived, never physically destroyed). |
| D4 | Document versioning is maintained; re-uploading creates a new version. |
| D5 | Previous document versions remain accessible. |
| D6 | File size limits are configurable by System Administrators. |
| D7 | Restricted file types can be configured. |
| D8 | All document actions (upload, download, delete) are audited. |

---

## Compliance Rules

| # | Rule |
|---|------|
| CO1 | Compliance checks are mandatory for all new clients. |
| CO2 | Compliance check results cannot be deleted or modified after completion. |
| CO3 | Failed compliance checks block matter creation and progression. |
| CO4 | Periodic re-checks can be configured per client type or matter type. |
| CO5 | Only Compliance Officers can perform and record compliance checks. |
| CO6 | Compliance history is permanently retained. |

---

## Audit Rules

| # | Rule |
|---|------|
| A1 | Every significant action must generate an audit log entry. |
| A2 | Audit logs must record: who, what, when, and the before/after state. |
| A3 | Audit logs are append-only; they cannot be modified or deleted. |
| A4 | Audit logs must be retained for a minimum period defined by policy (e.g., 7 years). |
| A5 | Audit logs must be searchable and filterable. |

---

## Security and Access Rules

| # | Rule |
|---|------|
| S1 | Users can only access data permitted by their assigned roles. |
| S2 | Role assignments follow the principle of least privilege. |
| S3 | Authentication is required for all platform access. |
| S4 | Session timeouts are enforced after a configurable period of inactivity. |
| S5 | Failed login attempts are logged and may trigger account lockout. |
| S6 | Password policies are configurable (minimum length, complexity, expiry). |
| S7 | All API endpoints require authentication and authorisation checks. |
| S8 | Sensitive data (passwords, payment details) must be encrypted at rest and in transit. |
| S9 | User impersonation (for support) must be audited. |

---

## General Rules

| # | Rule |
|---|------|
| G1 | All records use soft-delete by default unless explicitly stated otherwise. |
| G2 | All entities have CreatedAt, CreatedBy, ModifiedAt, ModifiedBy fields. |
| G3 | System-generated reference numbers are immutable after creation. |
| G4 | Concurrent modifications must be handled (optimistic concurrency). |
| G5 | Business-critical operations must be idempotent where possible. |
| G6 | All monetary values are stored with precision (decimal, not floating point). |
| G7 | Date/time values are stored in UTC; displayed in the user's local timezone. |
