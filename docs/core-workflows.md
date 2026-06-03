# Core Workflows

This document describes the main user journeys and business workflows supported by the Clarity platform.

---

## 1. New Client Onboarding

**Actors:** Consultant, Legal Assistant, Compliance Officer

**Trigger:** A new client approaches the firm for legal services.

### Steps

1. Consultant or Legal Assistant creates a new client record with basic details (name, contact information, type — individual or organisation).
2. System generates a unique client reference number.
3. Compliance Officer is notified to perform identity verification and anti-money laundering (AML) checks.
4. Compliance check is recorded against the client.
5. If compliance checks pass, client status is set to **Active**.
6. If compliance checks fail, client status is set to **On Hold** and matter cannot proceed.
7. Client portal credentials are generated and sent to the client (if client portal access is enabled).
8. Audit log records the client creation and compliance check outcome.

### Business Rules
- A client cannot have matters opened until compliance checks are passed.
- Client reference numbers are system-generated and immutable.
- All client data changes are audited.

---

## 2. Opening a New Matter

**Actors:** Consultant, Team Leader

**Trigger:** A client instructs the firm on a new legal case or piece of work.

### Steps

1. Consultant creates a new matter, linking it to an existing client.
2. Matter details are captured: type, description, estimated value, fee arrangement (hourly/fixed), assigned team members.
3. System generates a unique matter reference number.
4. Team Leader is optionally notified for workload awareness.
5. Matter status is set to **Open**.
6. Default tasks are created based on matter type (if templates exist).
7. Client is notified that a new matter has been opened (if notifications are enabled).
8. Audit log records the matter creation.

### Business Rules
- A matter must belong to exactly one client.
- A matter must have at least one assigned consultant.
- Matter reference numbers are system-generated and immutable.
- Matters cannot be opened against clients with failed compliance checks.

---

## 3. Uploading Documents

**Actors:** Consultant, Legal Assistant, Client

**Trigger:** A document needs to be stored against a client or matter.

### Steps

1. User selects the target matter (or client, for general documents).
2. User uploads one or more files.
3. System stores the file securely and creates a document record with metadata (name, type, upload date, uploaded by).
4. Document is linked to the matter/client.
5. System scans for viruses/malware (if configured).
6. Document version is set to 1.
7. Other team members on the matter are notified (if configured).
8. Audit log records the upload.

### Business Rules
- Documents must be linked to a client or matter (not orphaned).
- File size limits apply (configurable by administrator).
- Certain file types may be restricted.
- Document deletion is soft-delete only (archived, not destroyed).
- Previous versions are retained when a document is re-uploaded.

---

## 4. Recording Time

**Actors:** Consultant, Legal Assistant (non-billable only)

**Trigger:** Work is performed on a matter that should be recorded.

### Steps

1. User selects a matter.
2. User enters time details: date, duration (hours/minutes), description of work, billable/non-billable flag.
3. System validates the entry (e.g., date not in the future, duration within limits).
4. Time entry is saved with status **Draft**.
5. For billing purposes, time entries must be approved by a Team Leader → status changes to **Approved**.
6. Approved time entries become available for invoicing.
7. Audit log records the time entry creation and any status changes.

### Business Rules
- Time entries must be linked to a matter.
- Billable time entries require a valid billing rate.
- Time cannot be recorded against closed matters.
- Maximum daily hours may be enforced (configurable).
- Time entries for past dates may require approval.
- Only Team Leaders can approve time entries.

---

## 5. Generating an Invoice

**Actors:** Finance User

**Trigger:** Approved time entries and/or disbursements are ready to be billed.

### Steps

1. Finance User selects a matter or client for invoicing.
2. System retrieves all approved, unbilled time entries and disbursements.
3. Finance User reviews the line items and can adjust descriptions or exclude items.
4. Invoice is generated with a unique invoice number, date, due date, and total.
5. Invoice status is set to **Draft**.
6. Finance User reviews and finalises the invoice → status changes to **Issued**.
7. Invoice is sent to the client (email or portal notification).
8. Time entries included on the invoice are marked as **Billed**.
9. Audit log records invoice generation and issuance.

### Business Rules
- Only approved time entries can appear on invoices.
- An invoice must have at least one line item.
- Invoice numbers are sequential and system-generated.
- Draft invoices can be edited; issued invoices are immutable.
- VAT/tax calculations apply based on configuration.

---

## 6. Receiving Payment

**Actors:** Finance User

**Trigger:** A client makes a payment against an outstanding invoice.

### Steps

1. Finance User records a payment: amount, date, payment method, reference, linked invoice(s).
2. System allocates payment to the specified invoice(s).
3. If payment equals the invoice total, invoice status changes to **Paid**.
4. If payment is less than the total, invoice status changes to **Partially Paid**.
5. Client account balance is updated.
6. Client is notified of payment receipt (if configured).
7. Audit log records the payment.

### Business Rules
- Payments must be linked to at least one invoice.
- Overpayments create a credit balance on the client account.
- Payment reversal requires Finance User action and is audited.
- Payment method must be recorded for reconciliation.

---

## 7. Compliance Review

**Actors:** Compliance Officer

**Trigger:** A new client is onboarded, a matter is opened, or suspicious activity is flagged.

### Steps

1. Compliance Officer receives a notification or reviews their compliance queue.
2. Officer opens the relevant client or matter.
3. Officer performs required checks (identity verification, AML, conflict of interest).
4. Officer records the outcome: **Pass**, **Fail**, or **Requires Further Investigation**.
5. If the check fails, the client/matter is placed on hold and relevant parties are notified.
6. If the check passes, normal operations continue.
7. Compliance check history is permanently recorded.
8. Audit log records all compliance activities.

### Business Rules
- Compliance checks are mandatory for new clients.
- Compliance checks cannot be deleted or modified after completion.
- Failed compliance checks block matter progression.
- Periodic re-checks may be required (configurable frequency).

---

## 8. Matter Closure

**Actors:** Consultant, Team Leader, Finance User

**Trigger:** Legal work on a matter is complete.

### Steps

1. Consultant requests matter closure.
2. System checks preconditions:
   - All tasks are complete or cancelled.
   - All time entries are approved and billed (or written off).
   - All invoices are paid (or written off).
   - No outstanding compliance issues.
3. If preconditions are met, Team Leader approves closure.
4. Matter status changes to **Closed**.
5. Closed matters become read-only (except for administrators).
6. Client is notified of matter closure.
7. Documents and history remain accessible for reference.
8. Audit log records the closure.

### Business Rules
- Matters cannot be closed with unbilled time or unpaid invoices (without write-off).
- Only Team Leaders or above can approve closure.
- Closed matters can be reopened by administrators if needed (audited).
- Data retention policies apply to closed matters.

---

## 9. Task Assignment and Completion

**Actors:** Consultant, Legal Assistant, Team Leader

**Trigger:** Work needs to be delegated or tracked.

### Steps

1. Consultant or Team Leader creates a task linked to a matter.
2. Task details include: description, assignee, due date, priority.
3. Assignee is notified of the new task.
4. Assignee works on the task and updates status (In Progress, Blocked, Complete).
5. Completed tasks may trigger the next step in a workflow.
6. Overdue tasks generate reminders/notifications.
7. Audit log records task creation and status changes.

### Business Rules
- Tasks must be linked to a matter.
- Tasks must have an assignee and due date.
- Overdue tasks are flagged in dashboards.
- Task completion may be required before matter closure.

---

## 10. Client Communication

**Actors:** Consultant, Legal Assistant, Client

**Trigger:** Information needs to be shared between the firm and the client.

### Steps

1. Firm user or client initiates a message within the matter context.
2. Message is recorded against the matter.
3. Recipient is notified (in-app and/or email).
4. Recipient reads and optionally responds.
5. Full message history is retained for the matter.
6. Audit log records all communications.

### Business Rules
- Communications are linked to a specific matter.
- Internal notes are never visible to clients.
- Client-facing messages are clearly distinguished from internal notes.
- Communication history cannot be deleted.
