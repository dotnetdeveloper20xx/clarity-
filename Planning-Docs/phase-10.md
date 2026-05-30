# Phase 10 — Testing, Quality Gates, Onboarding, and Long-Term Maintainability

Now we make the platform sustainable.

This phase answers one question:

**Can a team safely maintain this system for years?**

A Halo-style legal platform cannot depend on one clever developer. It needs quality gates, tests, standards, onboarding, documentation, and maintainable engineering habits.

## Goal of Phase 10

Create a professional engineering foundation around the application:

automated tests, code quality checks, documentation, onboarding guides, developer standards, support playbooks, architecture rules, and team working practices.

---

# Step 1 — Define Testing Strategy

Create a clear test pyramid.

Use:

Unit tests
Integration tests
API tests
UI tests
Security tests
Performance tests
Architecture tests

The goal is not “test everything blindly.”

The goal is to protect important business behaviour.

---

# Step 2 — Unit Tests

Unit test the business rules.

Examples:

Matter cannot close with unpaid invoices.
Billed time cannot be edited.
Invoice total is calculated correctly.
Payment allocation updates invoice balance.
High-risk compliance blocks matter closure.

These tests should be fast and easy to run.

---

# Step 3 — Integration Tests

Test real workflows across multiple parts of the backend.

Examples:

Create client → create matter → upload document
Record time → approve time → create invoice
Create invoice → record payment → invoice becomes paid
Flag matter → compliance review → matter becomes unblocked

These tests prove the system works as a flow.

---

# Step 4 — API Tests

Test API behaviour.

Check:

correct status codes
validation errors
authorization rules
pagination
filtering
search results
error responses
correlation IDs

This protects the contract between Angular and the backend.

---

# Step 5 — Frontend Tests

Test important Angular behaviour.

Examples:

AuthStore login/logout
PermissionGuard route access
MatterStore loading and error states
BillingStore invoice flow
UnsavedChangesGuard
Shared DataTable component
Error interceptor

Do not test meaningless UI details.

Test behaviour users depend on.

---

# Step 6 — Architecture Tests

Add tests that protect architectural rules.

Examples:

Domain must not reference Infrastructure.
Application must not reference API.
Controllers must not access DbContext directly.
Business logic must not live in controllers.
Infrastructure must implement Application interfaces.

These tests stop architecture from slowly decaying.

---

# Step 7 — Code Quality Gates

In CI pipeline, enforce:

backend build passes
backend tests pass
frontend build passes
frontend tests pass
lint passes
formatting passes
security scan passes
no high-risk package vulnerabilities

Broken code should not merge.

---

# Step 8 — Definition of Done

Create a Definition of Done.

A feature is not complete unless:

business rule is implemented
validation exists
authorization exists
audit logging exists where needed
tests exist
frontend loading/error/empty states exist
documentation updated
review completed
build passes

This protects quality.

---

# Step 9 — Developer Onboarding Guide

Create:

```text
docs/developer-onboarding.md
```

Explain:

how to clone the repo
how to run backend
how to run frontend
how to create database
how to seed data
how to run tests
how architecture works
how to add a new feature

Write it for a very junior developer.

---

# Step 10 — Feature Development Guide

Create:

```text
docs/how-to-add-a-feature.md
```

Example flow:

create domain entity if needed
create migration
create command/query
create validator
create handler
create API endpoint
create Angular API service
create feature store method
create UI screen
add tests
update docs

This gives the team a repeatable pattern.

---

# Step 11 — Code Review Guide

Create standards for reviews.

Reviewers should check:

business logic location
security
performance
readability
test coverage
naming
error handling
audit requirements
frontend state pattern

Code review is not about criticism.

It is team learning.

---

# Step 12 — Support Playbooks

Create support guides.

Examples:

How to investigate failed document upload
How to trace a correlation ID
How to retry failed background job
How to investigate slow dashboard
How to check permission issue
How to investigate missing invoice payment

This makes production support calmer.

---

# Step 13 — Architecture Decision Records

Create ADRs.

```text
docs/adr/
```

Examples:

Why Clean Architecture
Why Angular feature stores
Why SQL Server
Why soft delete
Why audit logs
Why JWT authentication
Why background jobs
Why SignalR

ADRs explain why decisions were made.

Future developers will thank you.

---

# Step 14 — Technical Debt Register

Create:

```text
docs/technical-debt-register.md
```

Track:

debt item
risk
impact
owner
target date
decision

Mature teams do not pretend technical debt does not exist.

They manage it.

---

# Step 15 — Final Documentation Pack

Create:

```text
docs/business-overview.md
docs/architecture-overview.md
docs/database-guide.md
docs/api-guide.md
docs/frontend-guide.md
docs/security-guide.md
docs/workflow-guide.md
docs/devops-guide.md
docs/testing-guide.md
docs/support-guide.md
```

This turns the project into a real enterprise platform.

---

# Phase 10 Deliverables

At the end of Phase 10 we should have:

testing strategy, unit tests, integration tests, API tests, frontend tests, architecture tests, CI quality gates, Definition of Done, onboarding guide, feature development guide, code review guide, support playbooks, architecture decision records, technical debt register, and final documentation pack.

---

# AI Prompt For Phase 10

Use this:

> Complete Phase 10 only. Implement the testing, quality, onboarding, documentation, support, and long-term maintainability foundation for the enterprise legal practice management platform. Add unit tests, integration tests, API tests, frontend tests, architecture tests, CI quality gates, Definition of Done, developer onboarding guide, feature development guide, code review guide, support playbooks, architecture decision records, technical debt register, and final documentation pack. Focus on making the platform maintainable by a real enterprise team for many years.
