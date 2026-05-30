
# Phase 2 — Domain Modelling and Enterprise Architecture

A lot of junior developers think architecture starts with:

* ASP.NET Core
* Angular
* SQL Server
* Azure

It doesn't.

Architecture starts with understanding the business concepts.

If we get the business concepts wrong, no amount of good code can save us.

---

# Goal Of Phase 2

Transform business knowledge into technical design.

By the end of this phase we should know:

* What domains exist
* What entities exist
* What relationships exist
* What projects exist
* What responsibilities exist
* What architecture style will be used

Still:

NO coding.

NO database creation.

NO API generation.

Just design.

---

# Step 1 — Identify Core Domains

Think like a business analyst.

Do not think like a developer yet.

Review all workflows discovered in Phase 1.

Now identify business domains.

For Halo-like software these are likely:

## Client Domain

Responsible for:

* Client details
* Contact information
* Client relationships
* Client history

---

## Matter Domain

Responsible for:

* Legal matters
* Matter status
* Matter ownership
* Matter lifecycle

---

## Document Domain

Responsible for:

* Uploading documents
* Versioning
* Storage references
* Security permissions

---

## Time Recording Domain

Responsible for:

* Time entries
* Billable hours
* Non-billable hours
* Time reporting

---

## Billing Domain

Responsible for:

* Invoices
* Credit notes
* Financial calculations

---

## Payment Domain

Responsible for:

* Payments
* Receipts
* Payment allocation

---

## Compliance Domain

Responsible for:

* AML checks
* Risk reviews
* Audit reviews

---

## Security Domain

Responsible for:

* Users
* Roles
* Permissions

---

## Reporting Domain

Responsible for:

* Dashboards
* KPIs
* Operational reports

---

# Step 2 — Create Domain Boundaries

One of the biggest enterprise mistakes:

Everything talks to everything.

Avoid this.

Each domain owns its data.

Example:

Client Domain owns client information.

Matter Domain can reference a client.

But Matter Domain does not own client data.

This creates clean boundaries.

---

# Step 3 — Identify Entities

Now identify business objects.

Example:

Client

Matter

MatterNote

Document

TimeEntry

Invoice

Payment

User

Role

Permission

AuditEntry

Task

Notification

ComplianceReview

Each entity needs:

Purpose

Responsibilities

Relationships

Business rules

---

# Step 4 — Create Aggregate Roots

Think Domain Driven Design.

Ask:

Which entities are the owners?

Example:

Matter

May own:

MatterNotes

MatterTasks

MatterDocuments

MatterHistory

This becomes an aggregate.

---

# Step 5 — Create Domain Language

This is extremely important.

Create a business glossary.

Examples:

Matter

Client

Fee Earner

Billable Time

Compliance Review

Time Entry

Disbursement

Invoice

Write definitions.

Use the same language everywhere.

Code.

Database.

Documentation.

UI.

Reports.

Meetings.

Everything.

---

# Step 6 — Design High-Level Architecture

Now create system architecture.

We will use:

Clean Architecture.

Projects:

```text
LegalPlatform.sln

LegalPlatform.Domain

LegalPlatform.Application

LegalPlatform.Infrastructure

LegalPlatform.Api

LegalPlatform.Web

LegalPlatform.Tests
```

---

# Step 7 — Define Project Responsibilities

## Domain

Contains:

Business entities

Enums

Value objects

Business rules

Interfaces

No database code.

No Angular.

No SQL.

No Azure.

---

## Application

Contains:

Use cases

CQRS

Commands

Queries

Validators

Business workflows

MediatR handlers

---

## Infrastructure

Contains:

Entity Framework

Repositories

SQL Server

Blob Storage

Email

File Storage

External APIs

---

## API

Contains:

Controllers

Authentication

Authorization

Swagger

Middleware

---

## Web

Contains:

Angular

Pages

Components

Layouts

Services

State management

---

## Tests

Contains:

Unit tests

Integration tests

Architecture tests

---

# Step 8 — Define Cross-Cutting Concerns

Every enterprise application needs these.

Logging

Exception handling

Auditing

Caching

Security

Validation

Notifications

Monitoring

Tracing

These should not be duplicated.

Design them once.

Reuse everywhere.

---

# Step 9 — Define CQRS Strategy

Not every feature needs CQRS.

But enterprise systems benefit from consistency.

Examples:

Commands:

CreateMatter

CloseMatter

CreateInvoice

RecordTime

UploadDocument

---

Queries:

GetMatter

GetClient

GetInvoices

GetDashboard

GetAuditHistory

---

Document this approach.

---

# Step 10 — Define Event Strategy

Think ahead.

Events are extremely powerful.

Examples:

MatterCreated

MatterClosed

InvoiceGenerated

InvoicePaid

DocumentUploaded

TimeRecorded

ClientCreated

These events will later power:

Notifications

Reporting

Integrations

Background jobs

Audit logs

---

# Step 11 — Create Architecture Diagrams

Generate:

## System Context Diagram

Shows:

Users

External systems

Platform

---

## Domain Diagram

Shows:

Domains

Relationships

Ownership

---

## Application Flow Diagram

Shows:

UI

API

Application

Infrastructure

Database

---

## Request Flow Diagram

Shows:

Browser

API

Service

Database

Response

---

# Step 12 — Create Architecture Documentation

Generate:

```text
docs/architecture-overview.md

docs/domain-model.md

docs/domain-boundaries.md

docs/cqrs-strategy.md

docs/event-strategy.md

docs/project-structure.md

docs/system-context.md
```

---

# Step 13 — Review Against Enterprise Principles

Before proceeding ask:

Can new developers understand it?

Can we scale it?

Can we test it?

Can we deploy it?

Can we monitor it?

Can we support it?

Can we add features safely?

Can it survive 10 years?

If not:

Refine architecture.

---

# Phase 2 Deliverables

By the end of Phase 2 we should have:

✅ Domain model

✅ Architecture diagrams

✅ Clean Architecture structure

✅ Project structure

✅ CQRS strategy

✅ Event strategy

✅ Business glossary

✅ Domain boundaries

✅ Architecture documentation

Still:

❌ No code

❌ No database

❌ No APIs

❌ No Angular

Only design.

---

# AI Prompt For Phase 2

> Complete Phase 2 only. Analyse the business documentation from Phase 1 and design the complete enterprise architecture. Identify domains, entities, aggregates, boundaries, project structure, CQRS strategy, event strategy, and Clean Architecture design. Generate architecture documentation and diagrams. Do not generate implementation code yet. Focus on maintainability, scalability, onboarding, and long-term ownership.

---

