# Phase 5 — Angular Frontend and Enterprise State Management

Perfect. This phase is where the system stops feeling like “backend APIs” and starts feeling like a real legal SaaS platform.

The frontend must not be a collection of random Angular pages. It must behave like a proper enterprise application: predictable state, clear data flow, loading states, error handling, permissions, caching, refresh behaviour, route guards, dashboards, and reusable UI patterns.

## Goal of Phase 5

Build the Angular frontend using:

Angular latest
TypeScript
Tailwind CSS
DaisyUI
Angular Signals
RxJS
Enterprise state management
Role-based UI
Modern dashboard design
Reusable components
API integration
Error/loading/empty states
Responsive layout

The frontend should feel like a serious corporate legal platform, similar to Microsoft 365, Salesforce, Jira, HubSpot, or Monday.com.

---

# Step 1 — Create Angular Project Structure

Create:

```text
LegalPlatform.Web
```

Suggested structure:

```text
src/app/
  core/
    auth/
    guards/
    interceptors/
    layout/
    services/
    state/
    models/

  shared/
    components/
    ui/
    pipes/
    directives/

  features/
    dashboard/
    clients/
    matters/
    documents/
    time-recording/
    billing/
    payments/
    compliance/
    reporting/
    admin/
    audit/
    notifications/

  shell/
    sidebar/
    topbar/
    breadcrumbs/
    command-palette/
```

Important idea:

`core` is for application-wide services.
`shared` is for reusable UI.
`features` is for business modules.
`shell` is the main enterprise layout.

---

# Step 2 — Install Frontend Styling

Use:

Tailwind CSS
DaisyUI
Angular animations

Create a professional legal colour scheme:

Deep navy
Slate grey
Royal blue
Soft teal
White cards
Amber warning
Red compliance alert
Green paid/success states

The UI should feel:

serious, trustworthy, premium, clean, corporate, modern.

Not cartoonish. Not template-looking.

---

# Step 3 — Build The Application Shell

The shell is the frame around the whole application.

It includes:

Sidebar
Top navigation
Breadcrumbs
User menu
Notification bell
Search / command palette
Theme toggle
Main content area

The sidebar should be role-aware.

Example:

Finance users see Billing and Payments.

Compliance users see Compliance and Audit.

Clients see only their portal pages.

Admins see everything.

---

# Step 4 — Build Authentication State

Create an `AuthStore`.

This should manage:

current user
JWT token
refresh token
roles
permissions
login state
session expiry
logout
token refresh

Use Angular Signals for local reactive state.

Example conceptually:

The app should always know:

Who is logged in?
What role do they have?
What can they access?
Is the token valid?
Should we redirect them?

This state is global.

So it belongs in a core store, not inside random components.

---

# Step 5 — Add HTTP Interceptors

Create interceptors for:

Authentication
Error handling
Correlation ID
Loading indicator
Retry strategy where safe

Auth interceptor:

adds bearer token to API calls.

Error interceptor:

handles 401, 403, 500, validation errors, network failure.

Correlation interceptor:

adds a request tracking ID.

This connects frontend behaviour to backend diagnostics.

Very enterprise.

---

# Step 6 — Design Enterprise State Management Strategy

Use a hybrid pattern:

Angular Signals for UI state and feature stores
RxJS for API streams and async workflows
Route resolvers for critical page data
Local cache for list screens
Optimistic updates only where safe

Avoid putting everything into one giant global store.

That becomes painful.

Instead, use feature stores.

Example:

```text
AuthStore
ClientStore
MatterStore
DocumentStore
BillingStore
NotificationStore
DashboardStore
ComplianceStore
```

Each store owns its own feature state.

---

# Step 7 — Define Standard Feature State Shape

Every feature store should follow a consistent state model:

```text
items
selectedItem
filters
pagination
sort
loading
saving
error
lastLoadedAt
isDirty
```

This consistency helps junior developers.

Once they learn one feature, they understand the others.

---

# Step 8 — Client State Management

ClientStore should manage:

client list
selected client
client search filters
pagination
client matters
client documents
client timeline
loading state
validation errors

Client pages:

Client list
Client details
Create client
Edit client
Client timeline
Client matters tab
Client documents tab

Important:

Search and filters should be stored in state so if the user opens a client and goes back, the list remembers their search.

That feels professional.

---

# Step 9 — Matter State Management

MatterStore is more complex.

It manages:

matter list
selected matter
matter status
matter notes
matter tasks
matter documents
matter time entries
matter invoices
matter timeline
permissions per matter

Matter detail page should use tabs:

Overview
Documents
Tasks
Time
Billing
Compliance
Audit Trail

This is a proper enterprise screen.

Not a simple form.

---

# Step 10 — Document State Management

DocumentStore manages:

uploaded files
upload progress
document metadata
document versions
download state
preview state
access logs
processing status

Support:

drag and drop upload
version history
document category filter
matter-linked documents
client-linked documents

This is very important for legal platforms because documents are central to legal work.

---

# Step 11 — Time Recording State Management

TimeRecordingStore manages:

daily time entries
weekly time sheet
matter time
billable/non-billable filters
draft entries
submitted entries
approval state

Important enterprise behaviour:

A user may start entering time, navigate away, and come back.

State should protect draft work.

Use dirty state warnings.

Example:

“You have unsaved time entries.”

---

# Step 12 — Billing State Management

BillingStore manages:

invoice list
selected invoice
invoice lines
draft invoice
outstanding balances
payment allocations
billing filters
finance dashboard data

Billing needs careful state because financial screens must not behave casually.

Do not use unsafe optimistic updates for money.

When invoice is issued, wait for backend confirmation.

Then refresh invoice state.

---

# Step 13 — Compliance State Management

ComplianceStore manages:

flagged matters
AML checks
risk levels
review notes
compliance status
high-risk alerts
review queue

Compliance UI should feel serious.

Use badges:

Low risk
Medium risk
High risk
Critical

Use clear audit-friendly layouts.

---

# Step 14 — Dashboard State Management

DashboardStore manages:

KPI cards
matter counts
billing summaries
time summaries
compliance warnings
team workload
recent activity
system health

Dashboard should support:

auto refresh
manual refresh
date range filtering
role-specific widgets

Example:

Finance dashboard is different from consultant dashboard.

Compliance dashboard is different from admin dashboard.

---

# Step 15 — Notification State

NotificationStore manages:

unread count
notification list
toast events
real-time updates
mark as read
mark all as read

Later, this can connect to SignalR.

For now, use API polling or local refresh.

---

# Step 16 — Build Shared UI Components

Create reusable enterprise components:

DataTable
FilterBar
StatusBadge
RiskBadge
MoneyDisplay
DateDisplay
PageHeader
EmptyState
ErrorPanel
LoadingSkeleton
ConfirmDialog
DrawerPanel
Timeline
AuditTrailViewer
PermissionGate
MetricCard
SearchBox
PaginationControls

This avoids duplicated frontend code.

It also makes the app feel consistent.

---

# Step 17 — Build Enterprise Data Table Pattern

Almost every legal system has tables.

Clients table.
Matters table.
Invoices table.
Payments table.
Documents table.
Audit table.

So build a strong reusable table pattern.

Must support:

server-side pagination
server-side sorting
server-side filtering
loading skeleton
empty state
row actions
bulk actions
permission-aware buttons

This is a major enterprise skill.

---

# Step 18 — Build Route Guards

Create:

AuthGuard
RoleGuard
PermissionGuard
UnsavedChangesGuard

Examples:

Only Finance can access Billing.

Only Compliance can access Compliance Review.

Only Admin can access User Management.

UnsavedChangesGuard protects forms from accidental navigation.

---

# Step 19 — Form Architecture

Use Reactive Forms.

Each major form should support:

validation
server-side errors
dirty state
save state
cancel
reset
field-level messages

Forms:

Create Client
Create Matter
Record Time
Create Invoice
Upload Document Metadata
Compliance Review
Payment Entry

Do not put all form logic in components.

Use form services or feature store methods where appropriate.

---

# Step 20 — API Integration Layer

Create strongly typed API clients:

ClientApiService
MatterApiService
DocumentApiService
BillingApiService
PaymentApiService
ComplianceApiService
AuditApiService

These services only talk HTTP.

They do not own complex UI state.

Feature stores call API services.

Components call feature stores.

That gives clean separation.

Flow:

Component → Store → API Service → Backend

Not:

Component → HTTP directly everywhere

---

# Step 21 — Error Handling UX

Enterprise apps must handle failure politely.

Show:

validation messages
permission messages
network error messages
server error messages
retry options
correlation ID for support

Example:

“Something went wrong while loading this matter. Reference: ABC-123.”

That reference should match backend correlation ID.

Very professional.

---

# Step 22 — Loading And Empty States

Every page should handle:

loading
loaded with data
loaded with no data
error
permission denied

Example empty state:

“No matters found for this client yet. Create a new matter to begin legal work.”

This feels much better than a blank page.

---

# Step 23 — Build Main Pages

Build in this order:

Dashboard
Client list
Client detail
Matter list
Matter detail
Document centre
Time recording
Billing
Payments
Compliance
Audit logs
Admin users
Reports

Do not start with the hardest screen.

Start with Dashboard and Clients to prove the layout and state pattern.

---

# Step 24 — Add Animations

Use subtle animations:

page fade-in
card hover
drawer slide
modal scale
toast slide
sidebar collapse
loading skeletons

Keep it professional.

Legal software should feel polished, not childish.

---

# Step 25 — Build Role-Aware UI

Do not only protect backend.

Frontend must also hide unavailable actions.

Examples:

Finance user sees “Issue Invoice”.

Consultant does not.

Compliance user sees “Mark Review Complete”.

Client does not.

Admin sees user management.

This improves usability.

But remember:

Frontend hiding is convenience.

Backend authorization is security.

---

# Step 26 — Frontend Testing

Add tests for:

AuthStore
MatterStore
BillingStore
route guards
interceptors
critical forms
shared components

Focus on behaviour, not meaningless implementation.

---

# Step 27 — Frontend Documentation

Create:

```text
docs/frontend-architecture.md
docs/state-management.md
docs/angular-folder-structure.md
docs/ui-component-guide.md
docs/frontend-debugging-guide.md
docs/role-based-ui.md
```

Explain it so a junior developer can understand:

where to add a new page
where state belongs
where API calls belong
how permissions work
how loading/errors are handled

---

# Phase 5 Deliverables

At the end of Phase 5 we should have:

Angular app
Tailwind and DaisyUI setup
enterprise layout
authentication integration
role-aware navigation
feature stores
complex state management
API integration
dashboard
clients UI
matters UI
documents UI
time recording UI
billing UI
compliance UI
audit UI
admin UI
shared UI component library
loading/error/empty states
frontend tests
frontend documentation

---

# AI Prompt For Phase 5

Use this:

> Complete Phase 5 only. Build the Angular frontend for the enterprise legal practice management platform using Angular latest, TypeScript, Tailwind CSS, DaisyUI, Angular Signals, RxJS, route guards, interceptors, reusable UI components, and enterprise feature-store state management. Implement complex frontend state using feature stores such as AuthStore, ClientStore, MatterStore, DocumentStore, TimeRecordingStore, BillingStore, ComplianceStore, DashboardStore, NotificationStore, and AuditStore. Components must not call HTTP directly. Components must interact with stores, stores call API services, and API services call the backend. Build professional legal SaaS UI screens with loading states, error states, empty states, role-aware navigation, responsive layout, dark mode, animations, and reusable enterprise components. Generate frontend documentation for junior developers. Do not implement advanced production deployment yet.
