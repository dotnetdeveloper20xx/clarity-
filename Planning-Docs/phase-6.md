# Phase 6 — Workflow Engine, Background Processing, Notifications, and Audit Trail

Perfect.

Now the application has:

business understanding, architecture, database, backend APIs, Angular frontend, and complex state management.

At this stage the system works like a normal enterprise web application.

But a Halo-style legal platform needs more than screens and CRUD.

It needs movement.

It needs workflows.

It needs background jobs.

It needs audit history.

It needs notifications.

It needs operational behaviour.

This is the phase where the platform starts feeling alive.

---

# Goal Of Phase 6

Implement enterprise workflow behaviour across the legal platform.

This phase adds:

matter lifecycle workflows, task workflows, document workflows, billing workflows, approval flows, background processing, notifications, audit trails, activity timelines, scheduled jobs, and real-time or near-real-time operational updates.

The goal is to make the application behave like a real legal business platform, not just a data entry system.

---

# Step 1 — Define Workflow Principles

Before coding workflows, define the principles.

A workflow is not just a status change.

A workflow represents business movement.

Example:

A matter moves from Open to Awaiting Client.

An invoice moves from Draft to Issued.

A time entry moves from Submitted to Approved.

A compliance review moves from Pending to Completed.

Each movement may require:

validation, permission checks, audit logging, notifications, and sometimes background processing.

So every workflow must answer:

Who is allowed to perform this action?
What conditions must be true?
What data changes?
What audit record is created?
Who should be notified?
What background work should happen next?

---

# Step 2 — Create Matter Workflow

Matter workflow is central.

Matter statuses:

```text
Draft
Open
AwaitingClient
AwaitingThirdParty
OnHold
BillingReview
Closing
Closed
Archived
```

Allowed transitions:

Draft → Open
Open → AwaitingClient
Open → AwaitingThirdParty
Open → OnHold
OnHold → Open
AwaitingClient → Open
AwaitingThirdParty → Open
Open → BillingReview
BillingReview → Closing
Closing → Closed
Closed → Archived

Do not allow random transitions.

For example:

Draft should not jump directly to Archived.

Closed should not go back to Open unless Admin reopens with a reason.

Every transition must require a reason where appropriate.

---

# Step 3 — Implement Matter Workflow Service

Create an application service or command handler:

```text
ChangeMatterStatusCommand
```

It should:

load matter, check user permission, validate allowed transition, apply status change, create audit log, create matter timeline event, send notifications if required, return updated matter status.

Business rule examples:

Only TeamLeader or Admin can close a matter.

Only Admin can reopen a closed matter.

Matter cannot close if unpaid invoices exist.

Matter cannot close if required compliance checks are incomplete.

This is where business rules become real.

---

# Step 4 — Create Task Workflow

Legal teams live through tasks.

Tasks might include:

Call client
Review document
Prepare invoice
Complete AML check
Send contract
Follow up payment

Task statuses:

```text
NotStarted
InProgress
Blocked
Completed
Cancelled
```

Task priorities:

```text
Low
Normal
High
Urgent
```

Task rules:

Task must belong to a matter or client.

Task may be assigned to a user.

Urgent tasks should create notification.

Overdue tasks should appear on dashboard.

Completed tasks should create timeline event.

---

# Step 5 — Create Document Workflow

Documents are not just files.

They have lifecycle.

Document statuses:

```text
Uploaded
PendingReview
Approved
Rejected
Archived
Deleted
```

Document workflow:

Upload document
Create metadata
Scan or validate file
Mark pending review
Approve or reject
Version if replaced
Archive when matter closes

Rules:

Rejected documents require reason.

Approved document versions should be preserved.

Clients should only see documents marked client-visible.

Internal documents should remain private.

Every download should optionally create access log for sensitive documents.

---

# Step 6 — Create Time Entry Workflow

Time recording has business importance because it affects billing.

Time entry statuses:

```text
Draft
Submitted
Approved
Rejected
Billed
WrittenOff
```

Allowed transitions:

Draft → Submitted
Submitted → Approved
Submitted → Rejected
Rejected → Draft
Approved → Billed
Approved → WrittenOff

Rules:

Draft time can be edited by owner.

Submitted time cannot be edited unless returned.

Approved time can be billed.

Billed time cannot be edited.

Written-off time requires reason.

All transitions must be audited.

---

# Step 7 — Create Billing Workflow

Billing workflow is finance-sensitive.

Invoice statuses:

```text
Draft
Issued
PartPaid
Paid
Cancelled
WrittenOff
```

Rules:

Draft invoice can be edited.

Issued invoice cannot be edited except by Finance/Admin.

Paid invoice cannot be cancelled without credit note or refund process.

Invoice issue creates notification.

Invoice overdue creates scheduled reminder.

Invoice payment updates balance and status automatically.

---

# Step 8 — Create Compliance Workflow

Compliance workflow protects the firm.

Compliance statuses:

```text
Pending
InReview
Passed
Failed
Escalated
Expired
```

Rules:

High-risk clients require compliance review.

Failed compliance blocks matter progression.

Expired compliance triggers warning.

Compliance decisions must be audited.

Only Compliance users can complete compliance reviews.

---

# Step 9 — Create Audit Service

Audit should not be an afterthought.

Create centralized audit service:

```text
IAuditService
```

It should record:

entity name, entity id, action, old values, new values, changed by, changed date, correlation id, reason, source.

Examples:

Matter status changed from Open to Closed.

Invoice issued.

Payment recorded.

Document downloaded.

Permission changed.

Compliance review completed.

Audit must be consistent across all features.

---

# Step 10 — Create Timeline System

Audit is for accountability.

Timeline is for user understanding.

A matter timeline should show human-friendly events:

Matter opened by Sarah.

Document uploaded.

Time entry approved.

Invoice issued.

Payment received.

Compliance review completed.

Matter closed.

This timeline helps users understand the story of a matter.

Create:

```text
MatterTimelineEvent
ClientTimelineEvent
```

or a generic:

```text
ActivityEvent
```

---

# Step 11 — Create Notification System

Notifications should support:

in-app notifications, email simulation, future SMS or Teams integration.

Notification types:

```text
Info
Warning
Success
Error
TaskAssigned
MatterUpdated
InvoiceOverdue
ComplianceAlert
DocumentReviewRequired
```

Each notification should include:

recipient user id, title, message, type, link, read status, created date.

Angular should show:

notification bell, unread count, notification drawer, toast for new events.

---

# Step 12 — Add Background Jobs

Some work should not block user requests.

Create background job table:

```text
BackgroundJobs
```

Fields:

job type, payload JSON, status, retry count, max retries, error message, created date, started date, completed date, correlation id.

Statuses:

```text
Pending
Processing
Completed
Failed
DeadLetter
```

Job types:

```text
SendNotification
ProcessDocument
GenerateInvoicePdf
CheckOverdueTasks
CheckOverdueInvoices
ArchiveClosedMatters
GenerateReports
```

---

# Step 13 — Create Worker Service

Create:

```text
LegalPlatform.Worker
```

The worker should:

poll pending jobs, mark job processing, execute handler, mark completed, retry failures, dead-letter after max retries.

Do not let failed jobs disappear silently.

Failed jobs must be visible in diagnostics later.

---

# Step 14 — Add Retry And Dead Letter Behaviour

Enterprise systems must expect failure.

If email sending fails:

retry.

If document processing fails:

retry.

If it still fails:

move to dead-letter.

Dead-letter means:

the system could not process it automatically and a human may need to investigate.

This is very realistic enterprise behaviour.

---

# Step 15 — Add Scheduled Jobs

Some jobs happen on schedule.

Examples:

check overdue tasks every hour, check overdue invoices daily, archive closed matters nightly, send compliance expiry reminders daily, generate daily operations summary.

For local development, the worker can run these using timers.

Later this could become Azure Functions or scheduled jobs.

---

# Step 16 — Add Real-Time Updates

Add SignalR if desired.

Use it for:

notification updates, dashboard updates, matter timeline updates, task updates, document processing updates.

This makes the application feel modern.

Flow:

backend event occurs, SignalR broadcasts, Angular NotificationStore or MatterStore receives update, UI refreshes.

---

# Step 17 — Update Angular State Stores

Extend frontend stores.

MatterStore should handle timeline updates.

NotificationStore should handle real-time notifications.

TaskStore should track assigned and overdue tasks.

DocumentStore should track processing status.

BillingStore should track invoice status changes.

ComplianceStore should track review alerts.

DashboardStore should refresh KPI widgets when important events happen.

---

# Step 18 — Add Workflow UI

Create UI patterns:

status change modal, reason required textarea, approval buttons, rejection buttons, timeline panel, notification drawer, task board, overdue indicators, compliance alert cards.

Important:

Do not allow users to click invalid actions.

If matter cannot close, show why.

Example:

“This matter cannot be closed because there are unpaid invoices and incomplete compliance checks.”

That is enterprise UX.

---

# Step 19 — Add Workflow Tests

Test workflow rules.

Examples:

matter cannot close with unpaid invoice.

closed matter cannot be edited by consultant.

billed time cannot be changed.

failed compliance blocks matter closure.

invoice paid status updates after full payment.

document rejection requires reason.

These tests protect business behaviour.

---

# Step 20 — Create Workflow Documentation

Create:

```text
docs/workflow-engine.md
docs/matter-lifecycle.md
docs/task-workflow.md
docs/document-workflow.md
docs/time-recording-workflow.md
docs/billing-workflow.md
docs/compliance-workflow.md
docs/background-jobs.md
docs/notifications.md
docs/audit-trail.md
```

Explain workflows in plain English.

This helps junior developers and business stakeholders.

---

# Phase 6 Deliverables

At the end of Phase 6 we should have:

matter lifecycle workflows, task workflows, document workflows, time recording workflows, billing workflows, compliance workflows, centralized audit service, matter timeline, notifications, background jobs, worker service, retry/dead-letter handling, scheduled jobs, optional SignalR real-time updates, Angular workflow UI, workflow tests, and workflow documentation.

This is where the platform starts to feel like a real business system.

---

# AI Prompt For Phase 6

Use this:

> Complete Phase 6 only. Implement enterprise workflows, background processing, notifications, audit trails, timelines, and operational business movement for the legal practice management platform. Add matter lifecycle workflows, task workflows, document workflows, time recording workflows, billing workflows, and compliance workflows. Create centralized audit logging, user-friendly matter timelines, in-app notifications, background job processing, retry and dead-letter behaviour, scheduled jobs, optional SignalR real-time updates, and Angular workflow UI. Update frontend feature stores to support workflow state changes. Generate workflow tests and detailed documentation. Do not implement production deployment yet.
