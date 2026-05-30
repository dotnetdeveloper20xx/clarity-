
# Phase 1 — Business Discovery and Product Understanding

Before writing code, the AI or developer must understand the legal business first.

## Goal of Phase 1

Create a clear business understanding of the platform before architecture or coding starts.

This phase answers:

What are we building?
Who is it for?
Why does the legal firm need it?
What business problems does it solve?
What workflows must it support?

## Step 1 — Define the Product Vision

Write a short product vision:

> We are building an enterprise legal practice management platform for a modern legal firm. The system helps solicitors, consultants, legal assistants, finance teams, compliance teams, and administrators manage clients, matters, documents, time recording, billing, payments, compliance, reporting, and operational workflows in one secure platform.

## Step 2 — Identify User Roles

Document each user role clearly:

Client
Consultant / Solicitor
Legal Assistant
Team Leader
Finance User
Compliance Officer
System Administrator
Support User

For each role, document:

What do they do daily?
What information do they need?
What actions can they perform?
What should they not be allowed to do?

## Step 3 — Define Main Business Modules

Start with these modules:

Client Management
Matter Management
Document Management
Time Recording
Billing
Payments
Tasks and Workflows
Compliance
Audit Logs
Reporting
Administration
Notifications

Each module needs a plain-English explanation.

Example:

> Matter Management is the heart of the platform. A matter represents a legal case or legal instruction. It connects the client, solicitor, documents, tasks, time entries, billing, payments, notes, and compliance history.

## Step 4 — Map Core User Journeys

Document the major journeys.

Example journey:

Client is created
Matter is opened
Documents are uploaded
Tasks are assigned
Solicitor records time
Invoice is generated
Payment is received
Matter is closed
Audit history remains available

Do this for:

New client onboarding
Opening a new matter
Uploading documents
Recording time
Generating invoice
Receiving payment
Compliance review
Matter closure

## Step 5 — Capture Business Rules

Examples:

A matter must belong to a client.
Only finance users can mark invoices as paid.
Only compliance users can review flagged matters.
Every matter status change must be audited.
Deleted records should usually be soft deleted.
Documents must be linked to clients or matters.
Time entries must be billable or non-billable.
Closed matters should become read-only except for admin users.

## Step 6 — Define Non-Functional Requirements

Document enterprise expectations:

Security
Auditability
Performance
Reliability
Scalability
Role-based access
Data protection
Searchability
Reporting
Supportability
Maintainability

This is very important because corporate legal software is business-critical.

## Step 7 — Produce Phase 1 Documents

Create these files:

```text
docs/business-overview.md
docs/user-roles.md
docs/core-workflows.md
docs/business-rules.md
docs/non-functional-requirements.md
docs/glossary.md
```

## Phase 1 Output

At the end of Phase 1, we should have:

A clear business overview
Defined user roles
Core modules listed
Main workflows documented
Business rules captured
Non-functional requirements agreed
Legal terminology glossary started

## Instruction for AI Tool

Use this prompt:

> Complete Phase 1 only. Do not generate code yet. Analyse the legal practice management business domain. Create business documentation for user roles, workflows, modules, business rules, non-functional requirements, and glossary. Write everything in plain English so a junior developer and business stakeholder can understand the system before implementation begins.

