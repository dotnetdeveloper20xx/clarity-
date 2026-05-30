
# Phase 4 — Backend API and Application Layer Implementation

This is where we start turning our legal practice management design into working software.

Before this phase, we understood:

the business, the domains, the architecture, and the database.

Now we build:

APIs, application services, commands, queries, validation, security, logging, and workflow behaviour.

This is the phase where a junior developer first starts seeing the connection between business requirements and actual code.

---

# Goal Of Phase 4

Build the backend foundation using ASP.NET Core, Clean Architecture, CQRS, MediatR, Entity Framework Core, SQL Server, FluentValidation, Serilog, Swagger, authentication, authorization, and structured error handling.

By the end of this phase, the backend should support the first real business workflows.

We are not building the Angular frontend yet.

We are making sure the backend is strong, clean, testable, and ready for UI integration.

---

# Step 1 — Create The Backend Solution Structure

Create the solution:

```text
LegalPlatform.sln
```

Create backend projects:

```text
LegalPlatform.Domain
LegalPlatform.Application
LegalPlatform.Infrastructure
LegalPlatform.Api
LegalPlatform.Tests
```

The project responsibility is clear:

Domain contains business rules.

Application contains use cases.

Infrastructure contains database and external systems.

API exposes endpoints.

Tests protect behaviour.

This structure teaches junior developers separation of concerns from day one.

---

# Step 2 — Add Project References

Reference direction matters.

The Domain project should not depend on anything.

Application depends on Domain.

Infrastructure depends on Application and Domain.

API depends on Application and Infrastructure.

Tests can reference all required layers.

The rule is simple:

Business logic must not depend on technical details.

This is the heart of Clean Architecture.

---

# Step 3 — Install Required Backend Packages

Install packages for:

Entity Framework Core
SQL Server
MediatR
FluentValidation
Serilog
Swagger
JWT Authentication
AutoMapper if needed
Health Checks
OpenTelemetry later if required

Do not install packages randomly.

Every package should have a reason.

A senior developer should be able to explain why each dependency exists.

---

# Step 4 — Create Domain Entities

Start with core entities:

Client
Matter
Document
TimeEntry
Invoice
InvoiceLine
Payment
PaymentAllocation
ComplianceCheck
AuditLog
TaskItem
Notification
UserProfile

For each entity, define:

identity, required fields, status fields, timestamps, soft delete fields where appropriate.

Examples:

Client has many Matters.

Matter has many Documents, TimeEntries, Tasks, Notes, and AuditLogs.

Invoice has InvoiceLines.

Payment has PaymentAllocations.

This models the business.

---

# Step 5 — Create Domain Enums

Create enums for business status values.

Examples:

MatterStatus:

Open
OnHold
AwaitingClient
AwaitingThirdParty
BillingReview
Closed
Archived

InvoiceStatus:

Draft
Issued
PartPaid
Paid
Cancelled
WrittenOff

TimeEntryStatus:

Draft
Submitted
Approved
Rejected
Billed
WrittenOff

ComplianceRiskLevel:

Low
Medium
High
Critical

Enums help keep business language consistent.

---

# Step 6 — Create Value Objects

Use value objects for concepts that need structure.

Examples:

Money
EmailAddress
MatterNumber
InvoiceNumber
Address
PhoneNumber

This is not mandatory for every field, but it helps show domain maturity.

For example, Money should include amount and currency.

In legal finance, currency and precision matter.

---

# Step 7 — Create The DbContext

In Infrastructure, create the EF Core DbContext.

Example name:

```text
LegalPlatformDbContext
```

Add DbSet properties for all entities.

Configure relationships using Fluent API.

Avoid relying only on automatic conventions for important relationships.

For enterprise systems, be explicit.

Configure:

table names, keys, required fields, max lengths, indexes, relationships, delete behaviour.

---

# Step 8 — Configure Entity Mappings

Create separate configuration files.

Example:

ClientConfiguration
MatterConfiguration
DocumentConfiguration
InvoiceConfiguration
PaymentConfiguration
TimeEntryConfiguration
AuditLogConfiguration

This keeps DbContext clean.

A clean DbContext is easier for juniors to understand.

---

# Step 9 — Add Database Migrations

Create initial migration:

```text
InitialCreate
```

Apply migration to local SQL Server.

Then verify tables were created properly.

At this stage, do not rush.

Review generated migration.

Review database schema.

Make sure relationships are correct.

A bad database foundation causes years of pain.

---

# Step 10 — Add Seed Data

Seed minimum enterprise demo data:

roles, users, clients, matters, documents, time entries, invoices, payments, compliance checks, audit logs.

Seed enough records to make dashboards and searches meaningful.

Example:

50 clients
200 matters
500 time entries
100 invoices
200 payments
1000 audit logs

Later we can increase this for performance testing.

---

# Step 11 — Create Application Interfaces

In Application, define interfaces that Infrastructure will implement.

Examples:

IApplicationDbContext
ICurrentUserService
IAuditService
IDateTimeProvider
IFileStorageService
INotificationService

This keeps Application independent.

Application says what it needs.

Infrastructure decides how to provide it.

---

# Step 12 — Add CQRS Structure

Organize by feature.

Example:

```text
Application/Clients/Commands/CreateClient
Application/Clients/Queries/GetClientById
Application/Matters/Commands/CreateMatter
Application/Matters/Queries/GetMatterDetails
Application/TimeEntries/Commands/RecordTimeEntry
Application/Invoices/Commands/CreateInvoice
```

Each feature should contain:

Command or Query
Handler
Validator
DTO
Mapping if required

This keeps features easy to find.

---

# Step 13 — Build Client APIs First

Start with Client Management because most other workflows depend on clients.

Implement:

Create Client
Update Client
Get Client By Id
Search Clients
Soft Delete Client
Get Client Matters

Apply validation:

name required, email valid, address optional but structured, client type required.

Add audit logging for create, update, delete.

---

# Step 14 — Build Matter APIs

Implement:

Create Matter
Update Matter
Get Matter By Id
Search Matters
Change Matter Status
Assign Consultant
Close Matter
Archive Matter

Important business rule:

A matter must belong to a valid client.

Matter number should be unique.

Closed matters should become read-only except for admin users.

Every status change must be audited.

---

# Step 15 — Build Document APIs

Implement:

Upload Document
Get Matter Documents
Download Document
Add Document Version
Soft Delete Document
View Document Metadata

For local development, store files in:

```text
storage/documents
```

Store metadata in SQL.

This allows future move to Azure Blob Storage.

---

# Step 16 — Build Time Recording APIs

Implement:

Create Time Entry
Update Time Entry
Submit Time Entry
Approve Time Entry
Reject Time Entry
Get Time Entries By Matter
Get Time Entries By User

Business rules:

Time must belong to an open matter.

Billed time cannot be edited casually.

Rejected time requires reason.

Submitted time should be approved before billing.

---

# Step 17 — Build Billing APIs

Implement:

Create Invoice
Add Invoice Line
Issue Invoice
Cancel Invoice
Get Invoice By Id
Search Invoices
Get Outstanding Invoices

Business rules:

Only approved billable time can be billed.

Issued invoice cannot be edited freely.

Paid invoice cannot be deleted.

Financial changes must be audited.

---

# Step 18 — Build Payment APIs

Implement:

Record Payment
Allocate Payment To Invoice
Refund Payment
Get Payments By Client
Get Payments By Invoice

Business rules:

Payment cannot exceed invoice balance unless overpayment support is added.

A payment can be allocated across multiple invoices.

Paid status should update automatically when balance reaches zero.

---

# Step 19 — Build Compliance APIs

Implement:

Create Compliance Check
Update Compliance Result
Flag Matter
Review Compliance Flag
Get High Risk Matters
Get Compliance History

Business rules:

High-risk matters require review.

Compliance results must be audited.

Compliance records should not be hard deleted.

---

# Step 20 — Add Authentication And Authorization

Implement JWT authentication or ASP.NET Core Identity.

Seed roles:

Admin
Consultant
LegalAssistant
TeamLeader
Finance
Compliance
Support
Client

Protect endpoints using policies.

Examples:

Only Finance can issue invoices.

Only Compliance can complete compliance review.

Only Admin can manage users.

Clients can only see their own matters.

Security must be designed, not sprinkled randomly.

---

# Step 21 — Add Global Exception Handling

Create middleware that catches exceptions and returns a consistent error response.

Do not leak technical details to users.

Log full details internally.

Return safe messages externally.

This is important for security and professionalism.

---

# Step 22 — Add Correlation ID Middleware

Every request should have a correlation ID.

The ID should appear in:

request headers, logs, error responses, audit records where relevant.

This makes production debugging much easier.

---

# Step 23 — Add Request Logging

Use Serilog structured logging.

Log:

request path, method, user id, correlation id, duration, status code.

Log slow requests.

This helps support teams investigate issues.

---

# Step 24 — Add Swagger Documentation

Enable Swagger for local development.

Group endpoints by domain:

Clients
Matters
Documents
Time Recording
Billing
Payments
Compliance
Admin

Swagger helps frontend developers and testers understand the API.

---

# Step 25 — Add Health Checks

Add health endpoints for:

API status, database connectivity, file storage availability.

Later this can connect to Azure App Service health checks.

---

# Step 26 — Add Backend Tests

Start testing critical logic.

Tests should cover:

client validation, matter creation, matter status transitions, time entry approval, invoice generation, payment allocation, compliance flagging, authorization rules.

Do not test meaningless things.

Test business behaviour.

---

# Step 27 — Backend Documentation

Create:

```text
docs/api-guide.md
docs/backend-architecture.md
docs/authentication-authorization.md
docs/backend-debugging-guide.md
docs/error-handling.md
docs/audit-logging.md
```

Explain everything in plain English for junior developers.

---

# Phase 4 Deliverables

By the end of Phase 4 we should have:

working backend solution, SQL Server database, EF Core migrations, seed data, core APIs, CQRS handlers, validation, authentication, authorization, logging, correlation IDs, error handling, Swagger, health checks, and backend tests.

The backend should be good enough for frontend integration.

---

# AI Prompt For Phase 4

Use this:

> Complete Phase 4 only. Implement the backend API and application layer for the legal practice management platform using ASP.NET Core, Clean Architecture, CQRS, MediatR, EF Core, SQL Server, FluentValidation, Serilog, Swagger, JWT authentication, authorization, audit logging, correlation IDs, health checks, and backend tests. Build the core APIs for Clients, Matters, Documents, Time Recording, Billing, Payments, and Compliance. Keep controllers thin and business logic in Application handlers. Generate backend documentation for junior developers. Do not build the Angular frontend yet.

