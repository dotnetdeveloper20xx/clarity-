# Glossary

This document defines the key terms used throughout the Clarity platform. Understanding these terms is essential for developers, business stakeholders, and support teams.

---

## Business Terms

| Term | Definition |
|------|-----------|
| **Client** | A person or organisation that has engaged the legal firm for legal services. A client may have multiple matters. |
| **Matter** | A legal case, instruction, or piece of work undertaken on behalf of a client. A matter is the central entity that connects all work — time, documents, billing, and compliance. |
| **Consultant / Solicitor** | A qualified legal professional who advises clients and works on matters. They are the primary fee earners. |
| **Fee Earner** | Any person whose time can be billed to a client. Typically consultants/solicitors, but may include senior legal assistants. |
| **Legal Assistant** | A support professional who assists consultants with administrative and preparatory work on matters. |
| **Disbursement** | A third-party cost incurred on behalf of a client (e.g., court fees, search fees, expert reports) that is passed through on the invoice. |
| **Billable Time** | Time spent working on a matter that will be charged to the client. |
| **Non-Billable Time** | Time spent on internal activities (training, administration) that is not charged to clients. |
| **Time Entry** | A record of time spent by a fee earner on a specific matter, including date, duration, and description. |
| **Billing Rate** | The hourly rate charged for a fee earner's time, typically based on their grade or seniority. |
| **Invoice** | A formal request for payment sent to a client, detailing time and disbursements for a period. |
| **Credit Note** | A document issued to reduce the amount owed on a previously issued invoice. |
| **Write-Off** | A decision to forgive an amount owed, recording it as a loss rather than pursuing payment. |
| **Matter Type** | A classification of the legal work (e.g., conveyancing, litigation, family law, commercial). |
| **Conflict of Interest** | A situation where the firm's duty to one client could conflict with its duty to another. Must be checked before taking on work. |
| **AML** | Anti-Money Laundering — regulatory checks required to verify client identity and source of funds. |
| **KYC** | Know Your Customer — the process of verifying client identity as part of compliance. |
| **Retainer** | An advance payment held by the firm on behalf of the client, drawn down as work is performed. |

---

## Technical Terms

| Term | Definition |
|------|-----------|
| **Soft Delete** | Marking a record as deleted (IsDeleted = true) without physically removing it from the database. The record is excluded from normal queries but remains for audit and recovery. |
| **Audit Log** | An append-only record of every significant action performed in the system, used for compliance and investigation. |
| **Bounded Context** | A clearly defined boundary within the system where a particular domain model applies. Each module (Client Management, Billing, etc.) is a bounded context. |
| **CQRS** | Command Query Responsibility Segregation — a pattern where read operations (queries) are separated from write operations (commands) for clarity and performance. |
| **MediatR** | A library used to implement the mediator pattern, dispatching commands and queries to their handlers without direct coupling. |
| **Clean Architecture** | An architectural pattern that separates business logic from infrastructure concerns, ensuring the domain is independent of frameworks and databases. |
| **Entity** | A domain object with a unique identity that persists over time (e.g., a Client, Matter, or Invoice). |
| **Value Object** | A domain object defined by its attributes rather than identity (e.g., an Address or Money amount). |
| **DTO** | Data Transfer Object — a simple object used to transfer data between layers without business logic. |
| **Correlation ID** | A unique identifier assigned to a request, carried through all logs and services to enable end-to-end tracing. |
| **Optimistic Concurrency** | A strategy where conflicts are detected at save time rather than locking records, using version numbers or timestamps. |
| **Idempotent** | An operation that produces the same result regardless of how many times it is executed. Important for retry safety. |

---

## Status Terms

| Term | Context | Values |
|------|---------|--------|
| **Client Status** | Client record | Pending, Active, On Hold, Archived |
| **Matter Status** | Matter record | Open, In Progress, On Hold, Closed |
| **Time Entry Status** | Time recording | Draft, Approved, Billed |
| **Invoice Status** | Billing | Draft, Issued, Partially Paid, Paid, Written Off |
| **Task Status** | Workflow | To Do, In Progress, Blocked, Complete, Cancelled |
| **Compliance Check Status** | Compliance | Pending, Pass, Fail, Requires Investigation |
| **Document Status** | Documents | Active, Archived |

---

## Role Terms

| Term | Definition |
|------|-----------|
| **RBAC** | Role-Based Access Control — a security model where permissions are assigned to roles, and roles are assigned to users. |
| **Permission** | A specific action a user is allowed to perform (e.g., create_matter, approve_time_entry). |
| **Role** | A named collection of permissions assigned to users (e.g., Consultant, Finance User). |
| **Principle of Least Privilege** | Users should only be granted the minimum permissions necessary to perform their job. |

---

## Financial Terms

| Term | Definition |
|------|-----------|
| **WIP** | Work In Progress — time recorded but not yet billed to the client. A key financial metric for legal firms. |
| **Aged Debt** | Invoices that remain unpaid beyond their due date, categorised by age (30, 60, 90+ days). |
| **Revenue Recognition** | The point at which income is formally recognised in accounts (typically when invoiced or when paid, depending on accounting method). |
| **VAT** | Value Added Tax — a consumption tax added to invoices where applicable. |
| **Trust Account / Client Account** | A separate bank account holding client funds (e.g., retainers) that must be managed in strict compliance with regulatory rules. |
