# Clarity — Enterprise Legal Practice Management Platform

[![Build](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Tests](https://img.shields.io/badge/tests-83%20passing-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)]()
[![Angular](https://img.shields.io/badge/Angular-19-DD0031)]()
[![License](https://img.shields.io/badge/license-MIT-blue)]()
[![Architecture](https://img.shields.io/badge/architecture-Clean%20Architecture-orange)]()
[![Pattern](https://img.shields.io/badge/pattern-CQRS%20%2B%20MediatR-blueviolet)]()
[![CSS](https://img.shields.io/badge/css-Tailwind%20%2B%20DaisyUI-06B6D4)]()

---

## 🎯 Project Vision

### The Problem We Solve

Imagine you run a law firm with 50 solicitors, 20 assistants, a finance team, and hundreds of active clients. Every day your solicitors spend hours working on legal cases — researching laws, drafting contracts, attending court, advising clients. Every hour they work should be recorded, billed, and paid for. That is how your firm makes money.

Now imagine your firm uses spreadsheets to track time. Emails to share documents. A separate accounting system for invoices. Paper forms for compliance checks. A shared drive for legal files. Sound chaotic? It is. And it is exactly how many firms still operate.

Here is what goes wrong:

- **A solicitor forgets to log 30 minutes of billable work.** At £300/hour, that is £150 of lost revenue. Across 50 solicitors over a year, that becomes hundreds of thousands of pounds in unbilled work that simply vanishes.

- **A compliance check expires and nobody notices.** The firm continues working on a matter for a client who should have been flagged. If regulators find out, the firm faces fines, reputational damage, or worse — losing its licence to operate.

- **An invoice is sent late because the finance team was waiting for time entry approvals.** The client disputes charges because the descriptions are vague. Cash flow suffers. Relationships deteriorate.

- **A junior solicitor accidentally shares confidential documents from Client A with Client B** because there is no proper access control on the shared drive.

- **The managing partner asks: "How much unbilled work do we have across the firm right now?"** Nobody can answer without days of manual spreadsheet collation.

**Clarity solves all of these problems in one unified platform.**

---

### What is Clarity?

Clarity is a complete legal practice management system. Think of it as the operating system for a law firm. It is a single platform where everyone in the firm — solicitors, assistants, finance staff, compliance officers, administrators, and even clients — can do their work, see what they need to see, and nothing more.

Here is what Clarity does, explained simply:

**Client Management** — When a new client comes to the firm, their details are recorded in Clarity. Name, contact information, whether they are a person or a company. The system generates a unique client reference number. Before any work can begin, the compliance team must verify the client's identity (this is a legal requirement called "Know Your Customer" or KYC, and "Anti-Money Laundering" or AML checks). Until compliance passes, no legal work can start. This protects the firm from regulatory risk.

**Matter Management** — A "matter" is legal terminology for a case or piece of work. When a client asks the firm to help them buy a house, fight a lawsuit, or draft a contract, that becomes a matter. Each matter is linked to a client, assigned to a solicitor, and moves through a lifecycle: Open → In Progress → Awaiting Client → Billing Review → Closed → Archived. The system enforces rules at each stage — for example, you cannot close a matter until all invoices are paid and compliance checks are resolved.

**Document Management** — Legal work generates enormous amounts of documents: contracts, court filings, correspondence, evidence, research notes. Clarity stores all documents linked to the relevant client or matter. Documents are versioned (if you upload a new version of a contract, the old version is preserved). Documents are never physically deleted — they are archived. Access is controlled so clients can only see documents explicitly shared with them, and internal confidential notes remain private.

**Time Recording** — This is where the firm's revenue begins. When a solicitor works on a matter, they record how long they spent and what they did. For example: "2 hours — Reviewed contract and provided mark-up to client." Each time entry is marked as billable or non-billable. The entry starts as a Draft, then gets Submitted to a team leader for approval. Once Approved, it becomes available for billing. If Rejected, it goes back to the solicitor with a reason. This workflow ensures only accurate, verified time gets billed to clients.

**Billing** — The finance team takes approved time entries and creates invoices. The system calculates: hours worked × hourly rate = line item amount. It adds VAT/tax, generates a unique invoice number, and produces a professional invoice. Invoices start as Draft (editable), then get Issued (sent to client, now immutable — it becomes a legal document). The client can see their invoices in their portal.

**Payments** — When a client pays, the finance team records the payment against the invoice. The system automatically updates the invoice status: if partially paid, it shows how much is still outstanding. If fully paid, it marks the invoice as complete. The system tracks aged debt (how long invoices have been overdue) so finance can chase late payments.

**Compliance** — Law firms are heavily regulated. They must verify client identities, check for money laundering risk, ensure no conflicts of interest (e.g., the firm is not representing both sides of a dispute), and maintain records of all checks. Clarity automates this: compliance officers have a queue of checks to perform, the results are permanently recorded (they can never be deleted or modified), and failed checks block work from proceeding until resolved.

**Reporting & Dashboards** — Management needs answers: How much unbilled work exists? Which invoices are overdue? Which matters are stuck? How productive is each solicitor? Clarity provides real-time dashboards with these metrics. Different roles see different dashboards — finance sees revenue and debt, team leaders see workload, compliance sees risk flags.

**Notifications** — When something important happens (a time entry is rejected, a matter is closed, a compliance check fails), the relevant people are notified instantly in the platform. No more waiting for someone to forward an email.

**Audit Trail** — Every significant action is permanently recorded: who did what, when, and what changed. If a dispute arises about who approved an invoice or when a document was uploaded, the audit trail provides undeniable proof. This is a legal requirement for regulated firms.

---

### Who Uses Clarity?

Clarity serves eight distinct user roles, each with carefully controlled access:

| Role | What They Do | What They Can See |
|------|-------------|-------------------|
| **Client** | View their matter status, upload documents, pay invoices | Only their own matters, documents, and invoices |
| **Solicitor/Consultant** | Work on legal matters, record time, manage documents | Their assigned matters and related data |
| **Legal Assistant** | Support solicitors with admin tasks, manage documents | Matters they assist with |
| **Team Leader** | Approve time entries, monitor team workload, close matters | All matters for their team |
| **Finance** | Generate invoices, record payments, produce financial reports | All billing and payment data |
| **Compliance Officer** | Perform regulatory checks, investigate flagged activity | Audit logs and compliance data across all clients |
| **System Administrator** | Manage users, roles, permissions, system configuration | Everything (platform management) |
| **Support** | Help internal users troubleshoot issues | Diagnostic data and logs |

The platform enforces strict boundaries. A solicitor cannot see another solicitor's matters (unless they are on the same team). A client can never see another client's data. Finance cannot modify legal matter details. Compliance cannot alter financial records. This is not just good practice — for legal software, it is a regulatory requirement.

---

### How Revenue Flows Through the System

Understanding this flow is key to understanding why Clarity exists:

```
Solicitor works on a matter
         │
         ▼
Records time entry (e.g., "2 hours — Contract review")
         │
         ▼
Team Leader approves the time entry
         │
         ▼
Finance generates an invoice (hours × rate = £600)
         │
         ▼
Invoice is issued to the client
         │
         ▼
Client pays → Payment recorded → Invoice marked as Paid
         │
         ▼
Revenue recognised. The firm made money.
```

If any step in this chain breaks — time is not recorded, approvals are delayed, invoices are inaccurate, payments are lost — the firm loses money. Clarity ensures this chain is fast, accurate, audited, and visible to management at every step.

---

### The Technical Solution

Clarity is not a tutorial application or a proof-of-concept. It is engineered as production-grade enterprise software with the following characteristics:

**Clean Architecture** — The system is organised into four layers that follow strict dependency rules. The business logic (what a matter is, how billing works, what compliance rules exist) lives in the innermost layers and has zero knowledge of databases, web frameworks, or UI libraries. This means the business rules can survive any technology change. If SQL Server is replaced with PostgreSQL in five years, or Angular is replaced with a different framework, the core business logic does not change. This is how systems are built to last decades.

**CQRS (Command Query Responsibility Segregation)** — Every operation in the system is either a Command (changes data: "Create Client", "Approve Time Entry", "Issue Invoice") or a Query (reads data: "Get Client List", "Get Dashboard KPIs"). This separation makes the code predictable, testable, and easy to extend. Each operation has its own handler — a small, focused piece of code that does exactly one thing.

**Workflow Engine** — Status transitions are not random. The system defines exactly which status changes are allowed (a matter cannot jump from "Open" directly to "Archived"), who is permitted to make them (only Team Leaders can close matters), and what preconditions must be met (all invoices must be paid before closure). Every transition is validated, audited, and triggers notifications. This is enforced in code and tested with automated tests.

**Enterprise Security** — Authentication uses short-lived JWT tokens (15 minutes) with rotating refresh tokens (7 days). Accounts lock after 5 failed login attempts. Sessions are tracked and can be revoked by the user or an administrator. Every security event (login, failure, lockout, password change) is logged permanently. Authorization is enforced server-side on every API call — the frontend hides buttons you cannot use, but the backend will reject the request even if you bypass the UI.

**Full Audit Trail** — Every significant action creates an immutable audit record: who performed it, what changed (before and after values), when, and a correlation ID that links it to the original request. Audit records cannot be modified or deleted by anyone, including administrators. This meets the regulatory requirements of legal and financial systems.

**Background Job Processing** — Some operations should not make the user wait. Sending notifications, generating reports, checking for overdue items — these happen asynchronously via a job queue with automatic retry and dead-letter handling (if a job fails repeatedly, it is flagged for human investigation rather than silently lost).

**Structured Observability** — Every API request gets a unique correlation ID. All logs are structured (machine-readable JSON). If a user reports an error, support can search by correlation ID and trace the entire request through the system. Slow requests (>500ms) are automatically flagged. Health checks confirm database connectivity. This is how production systems are debugged at 2am.

---

### The Frontend

The Angular 19 frontend is designed as an enterprise SPA (Single Page Application) that feels like Salesforce, Microsoft 365, or Jira — not a Bootstrap template.

**Signal-Based State Management** — Each feature area (clients, matters, billing, etc.) has its own reactive store built with Angular Signals. Components never call APIs directly. Instead: Component → Store → API Service → Backend. This keeps state predictable, cacheable, and testable.

**Role-Aware Navigation** — The sidebar only shows menu items the user has permission to access. Finance users see Billing and Payments. Compliance users see Compliance and Audit. Clients see only their portal. Admins see everything.

**Enterprise UX Patterns** — Every page handles four states: loading (skeleton animation), loaded with data, loaded with no data (helpful empty state message), and error (with correlation ID for support). Status badges are colour-coded. Financial amounts are formatted. Dates are localised.

**Professional Visual Design** — Tailwind CSS with DaisyUI provides a custom "Clarity" theme: deep navy sidebar, clean white content areas, blue accents, amber warnings, red alerts, green success states. The design communicates trust, professionalism, and seriousness appropriate for legal software.

---

### Why This Project Matters

| If you are a... | Clarity demonstrates... |
|----------------|------------------------|
| **Hiring Manager** | End-to-end enterprise engineering: architecture, backend, frontend, database, security, DevOps, testing, and 67 files of documentation. This is not "I watched a tutorial." This is "I can design, build, and ship production software." |
| **Technical Lead** | Production thinking: workflow engines that enforce business rules, audit trails that satisfy regulators, security that goes beyond basic auth, observability that enables production debugging, architecture that survives team turnover. |
| **Business Stakeholder** | Real domain understanding: the code models how legal firms actually work — billable time, matter lifecycles, compliance gating, financial workflows, role-based access. This is software built by someone who understood the business first, then coded. |
| **Engineering Team** | A reference implementation: Clean Architecture + CQRS + MediatR + Angular Signals at enterprise scale, with tests that protect business behaviour, ADRs that explain decisions, and onboarding docs that get new developers productive in a week. |

---

## Quick Start

### Prerequisites

- .NET 10 SDK (`dotnet --version`)
- Node.js 20+ (`node --version`)
- SQL Server (local instance or Docker)

### Backend

```bash
git clone https://github.com/dotnetdeveloper20xx/clarity-.git
cd clarity-
dotnet restore
dotnet build
dotnet test                              # 83 tests pass
cd src/Clarity.Api
dotnet ef database update                # Create database and apply schema
dotnet run                               # API at https://localhost:5001/swagger
```

### Frontend

```bash
cd src/Clarity.Web
npm install
npm start                                # UI at http://localhost:4200
```

### Docker (Full Stack — One Command)

```bash
docker compose up                        # SQL Server + API at http://localhost:5001
```

### Test Accounts

| Email | Password | Roles |
|-------|----------|-------|
| admin@clarity.local | Admin123! | System Administrator |
| sarah.johnson@clarity.local | Password1! | Consultant, Team Leader |
| james.wilson@clarity.local | Password1! | Consultant |
| finance@clarity.local | Password1! | Finance |
| compliance@clarity.local | Password1! | Compliance Officer |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Angular 19 (SPA)                          │
│      Tailwind + DaisyUI │ Signals │ Feature Stores          │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTPS / JWT Bearer
┌──────────────────────────────▼──────────────────────────────┐
│                   ASP.NET Core 10 API                        │
│     Controllers │ Middleware │ Auth │ Swagger │ Health       │
└──────────────────────────────┬──────────────────────────────┘
                               │ MediatR Pipeline
┌──────────────────────────────▼──────────────────────────────┐
│               Application Layer (CQRS)                       │
│    Commands │ Queries │ Validators │ Workflow Engine         │
└────────────┬────────────────────────────────────┬───────────┘
             │                                    │
┌────────────▼────────────┐       ┌───────────────▼───────────┐
│     Domain Layer        │       │   Infrastructure Layer    │
│  Entities │ Enums       │       │  EF Core │ SQL Server     │
│  Interfaces │ Rules     │       │  Services │ File Storage  │
│  (Zero dependencies)    │       │  (Implements interfaces)  │
└─────────────────────────┘       └───────────────────────────┘
```

**The golden rule**: Dependencies point inward. Domain depends on nothing. Application depends on Domain. Infrastructure implements Application interfaces. API wires everything together. The frontend communicates only via HTTP.

---

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| Backend | ASP.NET Core 10, C# 13, MediatR, FluentValidation, Serilog, Swagger |
| Frontend | Angular 19, TypeScript, Tailwind CSS 3, DaisyUI 4, Angular Signals, RxJS |
| Database | SQL Server, Entity Framework Core 9, Code-First Migrations, Fluent API |
| Authentication | JWT Bearer (15-min) + Refresh Token Rotation (7-day), Account Lockout |
| Authorization | Role-Based (8 roles) + Policy-Based (15 policies) + Matter-Level Access |
| Testing | xUnit, FluentAssertions, Moq — 83 automated tests |
| DevOps | Docker, GitHub Actions CI/CD, Multi-environment config, Feature Flags |
| Documentation | 67 markdown files covering business, architecture, API, security, onboarding |

---

## Business Modules

| Module | Description | Key Operations |
|--------|-------------|----------------|
| 👥 Client Management | Individual and corporate client records | Create, update, search, archive, compliance gating |
| 📁 Matter Management | Legal cases with full lifecycle | Create, assign team, transition status, enforce closure rules |
| 📄 Document Management | Secure file storage with versioning | Upload, version, categorise, access control |
| ⏱️ Time Recording | Billable hour capture and approval | Record, submit, approve/reject, link to billing |
| 💰 Billing | Invoice generation from approved time | Create from time, issue, track outstanding |
| 💳 Payments | Payment recording and reconciliation | Record, allocate to invoices, auto-update status |
| 🛡️ Compliance | Regulatory checks (AML, KYC) | Create checks, record outcomes, block non-compliant work |
| 📊 Dashboards | Real-time KPIs and financial summaries | Open matters, unbilled time, aged debt, compliance alerts |
| 🔍 Global Search | Unified search across all entities | Search clients, matters, invoices by keyword |
| 📋 Audit Trail | Immutable action history | Every status change, creation, update permanently logged |
| 🔔 Notifications | In-app alerts for important events | Time rejected, matter closed, compliance failed |
| ⚙️ Administration | User and role management | Create users, assign roles, unlock accounts, system config |

---

## Workflow Engine

The system enforces business rules through validated status transitions:

### Matter Lifecycle
```
Open → In Progress → On Hold ←→ Open
  │         │ → Awaiting Client → Open
  │         │ → Awaiting Third Party → Open  
  │         │ → Billing Review → Closed
  │         └→ Closed → Archived (terminal)
  └→ Closed (requires: all invoices paid + compliance resolved + Team Leader role)
```

### Time Entry Lifecycle
```
Draft → Submitted → Approved → Billed (terminal)
                  → Rejected → Draft (re-editable)
                  Approved → Written Off (terminal)
```

### Invoice Lifecycle
```
Draft → Issued → Partially Paid → Paid (terminal)
              → Cancelled (terminal)
              → Written Off (terminal)
```

Each transition is: validated → permission-checked → executed → audited → notified.

---

## API Endpoints (40+)

| Area | Key Endpoints |
|------|--------------|
| Auth | `POST /login`, `POST /refresh`, `POST /logout`, `POST /change-password`, `GET /sessions` |
| Clients | `GET /clients`, `GET /clients/{id}`, `POST /clients`, `PUT /clients/{id}`, `DELETE /clients/{id}` |
| Matters | `GET /matters`, `GET /matters/{id}`, `POST /matters`, `PUT /matters/{id}/status` |
| Time | `GET /timeentries`, `POST /timeentries`, `PUT /timeentries/{id}/approve` |
| Workflow | `POST /workflow/matters/{id}/transition`, `POST /workflow/time-entries/{id}/submit`, `POST /workflow/time-entries/{id}/reject` |
| Invoices | `GET /invoices`, `POST /invoices`, `PUT /invoices/{id}/issue` |
| Payments | `POST /payments` |
| Documents | `POST /documents` (multipart upload) |
| Compliance | `POST /compliance/checks` |
| Dashboard | `GET /dashboard`, `GET /dashboard/financial` |
| Search | `GET /search?q={term}` |
| Export | `GET /export/clients`, `GET /export/invoices`, `GET /export/time-entries` |
| Notifications | `GET /notifications`, `GET /notifications/unread-count`, `PUT /notifications/{id}/read` |
| Audit | `GET /audit` (filterable by entity, user, date, correlation ID) |
| Diagnostics | `GET /diagnostics/jobs`, `GET /diagnostics/system-info`, `POST /diagnostics/jobs/{id}/retry` |
| Security | `GET /security/audit-log`, `GET /security/active-sessions`, `POST /security/users/{id}/unlock` |

Full interactive documentation available via Swagger at `/swagger` when running locally.

---

## Security Model

| Layer | Protection |
|-------|-----------|
| Authentication | JWT (15-min access token) + Refresh Token Rotation (7-day, server-side, single-use) |
| Account Protection | Lockout after 5 failed attempts (15 minutes), admin can unlock |
| Authorization | 8 roles × 15 named policies, enforced on every API endpoint |
| Matter Security | Per-matter access grants (Read / Contribute / Manage / Restricted) |
| Session Management | Users can view and revoke their own sessions; admins can revoke any |
| Audit | Immutable security event log (logins, failures, lockouts, password changes, session revocations) |
| Configuration | Secrets in environment variables / Key Vault, never in source code |
| Error Handling | Internal details never exposed to users; correlation IDs for support |

---

## Testing (83 Automated Tests)

| Category | Tests | What They Protect |
|----------|-------|-------------------|
| Client Validation | 4 | Names required, email format, length limits |
| Time Entry Validation | 5 | Duration bounds, description required, matter required |
| Domain Defaults | 8 | Entities have correct initial status/values |
| Matter Workflow | 14 | Only valid status transitions allowed |
| Time Entry Workflow | 11 | Correct approval/rejection/billing flow |
| Invoice Workflow | 9 | Correct issuance/payment/cancellation flow |
| Architecture Rules | 11 | Domain never references Infrastructure, layers enforced |
| Security | 5 | Refresh tokens expire, revoke, rotate correctly |
| Invoice Calculations | 7 | Tax, totals, payment allocation correct |
| Payment Logic | 7 | Full/partial payment, status updates |

Run all tests: `dotnet test` (executes in <1 second)

---

## Project Structure

```
clarity-/
├── src/
│   ├── Clarity.Domain/            # 22 entities, 16 enums, interfaces (zero dependencies)
│   ├── Clarity.Application/       # CQRS handlers, validators, workflow engine
│   ├── Clarity.Infrastructure/    # EF Core, SQL Server, 7 service implementations
│   ├── Clarity.Api/               # 12 controllers, middleware, auth, Swagger
│   └── Clarity.Web/               # Angular 19 SPA (10 components, 5 services, 2 stores)
├── tests/
│   └── Clarity.Tests/             # 83 automated tests
├── docs/                           # 67 documentation files
├── .github/workflows/              # CI (build + test) and Release (artifact) pipelines
├── Dockerfile.api                  # Multi-stage production Docker build
├── docker-compose.yml              # One-command local development stack
└── Clarity.sln
```

---

## Documentation (67 Files)

| Category | Documents |
|----------|-----------|
| Business Domain | Business overview, user roles, core workflows, business rules, glossary |
| Architecture | Architecture overview, domain model, domain boundaries, CQRS strategy, event strategy, system context |
| Database | Database design, table catalog (every column), entity relationships, indexing strategy, soft-delete strategy, data retention |
| API | API guide, error handling, authentication/authorization |
| Frontend | Frontend architecture, state management, folder structure, role-based UI, debugging guide |
| Workflows | Workflow engine, matter lifecycle, time recording workflow, billing workflow, compliance workflow |
| Security | Security model, permission matrix, session management, secure configuration |
| DevOps | DevOps strategy, environment strategy, release management, production readiness checklist, disaster recovery |
| Quality | Testing strategy, definition of done, code review guide, technical debt register |
| Onboarding | Developer onboarding (clone to running in 5 steps), how to add a feature, support playbooks |
| Decisions | 6 Architecture Decision Records (Clean Architecture, CQRS, SQL Server, Angular Signals, Soft Delete, JWT) |

---

## Development Phases

This project was designed and built in 10 structured phases:

| Phase | Focus | Deliverables |
|-------|-------|-------------|
| 1 | Business Discovery | User roles, workflows, business rules, glossary |
| 2 | Domain Modelling | Architecture design, domain boundaries, CQRS strategy |
| 3 | Database Design | Table catalog, indexes, audit strategy, retention policy |
| 4 | Backend API | Clean Architecture, 12 controllers, CQRS handlers, seed data |
| 5 | Angular Frontend | Enterprise SPA, signal stores, role-aware UI |
| 6 | Workflow Engine | Status validators, audit trail, notifications, background jobs |
| 7 | Reporting & Search | Dashboards, financial summary, global search, CSV exports |
| 8 | Security | JWT refresh rotation, RBAC, sessions, lockout, security audit |
| 9 | DevOps | Docker, CI/CD, environments, feature flags, disaster recovery |
| 10 | Quality & Onboarding | 83 tests, architecture tests, ADRs, onboarding docs |

---

## By The Numbers

| Metric | Value |
|--------|-------|
| Total source files | 295 |
| Lines of code | 16,688 |
| Automated tests | 83 |
| Documentation files | 67 |
| API endpoints | 40+ |
| Domain entities | 22 |
| Enums | 16 |
| EF Core configurations | 12 |
| API controllers | 12 |
| Angular components | 10 |
| Authorization policies | 15 |
| User roles | 8 |
| Workflow validators | 3 |
| Architecture Decision Records | 6 |
| Business workflows documented | 10 |
| Business rules captured | 50+ |

---

## Future Roadmap

- Azure Service Bus for distributed async messaging
- SignalR for real-time push notifications
- Azure Blob Storage for document storage at scale
- OpenTelemetry distributed tracing
- Full-text document content search (OCR)
- Two-factor authentication (TOTP)
- Multi-tenant architecture
- Email notification delivery
- Performance/load testing suite
- Mobile-responsive optimisation

---

## Contributing

This is a portfolio and reference project. Feel free to fork and adapt for your own purposes.

---

## License

[MIT](LICENSE)
