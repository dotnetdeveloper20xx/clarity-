# Phase 7 — Reporting, Dashboards, Search, Performance, and Diagnostics

Perfect.

Now the platform has real workflows. Matters move. Invoices are issued. Documents are uploaded. Compliance reviews happen. Background jobs run. Notifications are sent. Audit trails are created.

At this point, the legal firm will ask the next natural question:

“How do we see what is happening across the business?”

That is Phase 7.

This phase turns the system from a working application into a management platform.

## Goal of Phase 7

Build enterprise reporting, dashboards, search, diagnostics, and performance visibility.

This phase adds:

business dashboards, operational reporting, financial reporting, compliance reporting, global search, advanced filters, SQL performance tuning, API diagnostics, background job monitoring, audit search, and support-friendly troubleshooting tools.

---

# Step 1 — Define Reporting Personas

Different users need different information.

Consultants want:

their open matters, overdue tasks, time recorded this week, documents awaiting review.

Team Leaders want:

team workload, matter progress, overdue matters, consultant productivity.

Finance wants:

unpaid invoices, paid invoices, aged debt, billing totals, payment allocation.

Compliance wants:

high-risk matters, failed checks, expired reviews, unresolved flags.

Administrators want:

system health, failed jobs, login activity, audit activity.

Do not build one dashboard for everyone. Build role-aware dashboards.

---

# Step 2 — Build Main Dashboard

Create a main dashboard with cards such as:

open matters, new matters this month, overdue tasks, pending documents, unbilled time, outstanding invoices, compliance alerts, failed background jobs.

Each card should support:

loading state, error state, refresh, click-through navigation.

Example:

Click “Outstanding Invoices” and it opens Billing filtered to unpaid invoices.

That is good enterprise UX.

---

# Step 3 — Build Financial Dashboard

Finance dashboard should show:

total billed this month, total paid this month, outstanding balance, aged debt, invoices overdue by 30, 60, and 90 days, top clients by revenue, payment trends.

This is where business awareness becomes visible.

A legal firm does not only need case management.

It needs cash flow visibility.

---

# Step 4 — Build Matter Dashboard

Matter dashboard should show:

matters by status, matters by type, matters by consultant, average matter age, matters awaiting client, matters awaiting third party, matters ready for billing, matters blocked by compliance.

This helps team leaders manage operational flow.

---

# Step 5 — Build Compliance Dashboard

Compliance dashboard should show:

high-risk clients, high-risk matters, pending AML checks, failed compliance checks, expired reviews, unresolved compliance flags, matters blocked by compliance.

This should feel serious and clear.

Use strong visual indicators:

green for passed, amber for warning, red for critical.

---

# Step 6 — Build Consultant Productivity Dashboard

This dashboard should show:

time recorded this week, billable versus non-billable time, active matters, completed tasks, overdue tasks, billing contribution, matter closure rate.

Be careful with wording.

This should support management visibility, not feel like surveillance.

Enterprise software must respect people as well as process.

---

# Step 7 — Add Global Search

A Halo-style platform needs strong search.

Users should search across:

clients, matters, documents, invoices, payments, tasks, notes.

Search examples:

client name, matter number, invoice number, document title, email address, postcode.

Global search should return grouped results.

Example:

Clients
Matters
Documents
Invoices

Search should support permissions.

A user must not see search results for matters they are not allowed to access.

---

# Step 8 — Add Advanced Filters

Every major list should support filtering.

Client filters:

client type, city, status, risk level.

Matter filters:

status, matter type, consultant, date opened, date closed, client, billing status.

Invoice filters:

status, date range, overdue, client, matter.

Audit filters:

user, action, entity, date range, correlation id.

This is what makes enterprise software usable.

---

# Step 9 — Add Exporting

Business users often need exports.

Support:

CSV export, Excel-friendly export, PDF report later if needed.

Export examples:

matter list, invoice list, unpaid invoices, compliance flags, audit log, time entries.

Important rule:

Exports must respect permissions.

Never allow users to export data they cannot view.

---

# Step 10 — Create Reporting Read Models

Do not make every dashboard query load full entities.

Create optimized read models or projection queries.

Examples:

MatterSummaryDto
InvoiceSummaryDto
ComplianceSummaryDto
DashboardKpiDto
ConsultantWorkloadDto

Use SQL projections.

Use `AsNoTracking` for read-only queries.

Avoid loading huge object graphs.

This is where SQL and EF performance awareness matters.

---

# Step 11 — SQL Performance Review

Review all reporting queries.

Check:

are indexes used?
are queries paginated?
are unnecessary columns loaded?
are joins reasonable?
are N+1 queries avoided?
are date filters indexed?
are dashboard queries too heavy?

Document performance decisions.

A dashboard that works with 100 records may fail with 1 million records.

---

# Step 12 — Add Indexes For Reporting

Likely indexes:

Matter status
Matter consultant id
Matter client id
Matter opened date
Invoice status
Invoice due date
Payment date
Time entry user id
Time entry matter id
Compliance risk level
Audit created date
Audit entity id
Document matter id

Do not add indexes blindly.

Add them based on query patterns.

---

# Step 13 — Add Diagnostics Dashboard

Create an admin/support diagnostics area.

It should show:

recent API errors, slow requests, failed background jobs, dead-letter jobs, health check status, recent login failures, database connection status, file storage status.

This is extremely enterprise.

It tells interviewers:

“I design systems that can be supported in production.”

---

# Step 14 — Add Correlation Search

Support users should be able to search by correlation ID.

Example:

A user reports an error and gives reference:

`ABC-123`

Support searches it and sees:

request path, user id, time, error, related audit entry, related background job.

That is professional production debugging.

---

# Step 15 — Add Audit Search

Audit logs can become huge.

Add advanced audit search:

entity type, entity id, user, action, date range, correlation id.

Audit detail should show:

old values, new values, reason, source, timestamp, user.

This is essential for legal and financial systems.

---

# Step 16 — Add Background Job Monitoring

Create screen:

Background Jobs

Show:

pending, processing, completed, failed, dead-letter.

Allow admin/support to:

view job details, retry failed job, move dead-letter back to pending, mark investigated.

This makes the worker system supportable.

---

# Step 17 — Add Health Checks

Backend health checks should include:

API alive, database available, file storage available, background worker heartbeat, queue/job table status.

Frontend diagnostics page should display friendly statuses.

Example:

Database: Healthy
File Storage: Healthy
Worker: Warning
Failed Jobs: 3

---

# Step 18 — Add Frontend State Support

Update stores:

DashboardStore
ReportingStore
SearchStore
DiagnosticsStore
AuditStore
BackgroundJobStore

Each store should manage:

filters, loading, error, results, pagination, refresh state, last updated time.

For dashboard widgets, allow independent loading.

If one widget fails, the whole dashboard should not collapse.

That is mature UX.

---

# Step 19 — Add UI Polish

Dashboards should feel modern.

Use:

metric cards, charts, tables, badges, timeline, alerts, tabs, filter drawers, refresh buttons, skeleton loading, empty states.

Keep it corporate.

Clean, calm, confident.

Not noisy.

---

# Step 20 — Testing

Test:

dashboard query handlers, reporting filters, permission-filtered search, export permissions, diagnostics endpoints, audit search, background job retry behaviour.

Also test performance with seeded data.

Do not only test happy paths.

---

# Step 21 — Documentation

Create:

```text
docs/reporting-strategy.md
docs/dashboard-guide.md
docs/search-strategy.md
docs/sql-performance-guide.md
docs/diagnostics-guide.md
docs/audit-search-guide.md
docs/background-job-support-guide.md
```

Write for junior developers and support engineers.

Explain how to investigate:

slow dashboard, failed job, missing document, audit query, permission issue.

---

# Phase 7 Deliverables

At the end of Phase 7 we should have:

role-based dashboards, financial dashboard, matter dashboard, compliance dashboard, consultant productivity dashboard, global search, advanced filters, exports, optimized reporting queries, SQL indexes, diagnostics dashboard, correlation ID search, audit search, background job monitoring, health checks, frontend reporting state stores, performance tests, and documentation.

This phase makes the system look and feel like a serious corporate platform.

---

# AI Prompt For Phase 7

Use this:

> Complete Phase 7 only. Implement enterprise reporting, dashboards, global search, diagnostics, SQL performance improvements, and support tooling for the legal practice management platform. Build role-aware dashboards for consultants, team leaders, finance, compliance, administrators, and support users. Implement global search across clients, matters, documents, invoices, payments, tasks, and audit logs with permission-aware results. Add advanced filters, exports, optimized read models, SQL indexes, diagnostics dashboard, correlation ID search, audit search, background job monitoring, health checks, and frontend feature stores for reporting and diagnostics. Ensure dashboards handle loading, error, empty, and partial failure states independently. Generate documentation for reporting, SQL performance, diagnostics, and production support. Do not implement production deployment yet.
