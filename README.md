# Clarity — Enterprise Legal Practice Management Platform

[![Build](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Tests](https://img.shields.io/badge/tests-83%20passing-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)]()
[![Angular](https://img.shields.io/badge/Angular-19-DD0031)]()
[![License](https://img.shields.io/badge/license-MIT-blue)]()

> A production-grade legal practice management platform built with ASP.NET Core, Angular 19, SQL Server, and Clean Architecture. Designed for 10+ years of operational life.

![Clarity Architecture](https://img.shields.io/badge/architecture-Clean%20Architecture-orange)
![CQRS](https://img.shields.io/badge/pattern-CQRS%20%2B%20MediatR-blueviolet)
![Tailwind](https://img.shields.io/badge/css-Tailwind%20%2B%20DaisyUI-06B6D4)

---

## 🎯 Project Vision

**Clarity exists to solve a real-world problem that costs legal firms millions every year.**

Law firms run on time. Every six minutes a solicitor spends on a case is revenue. Every missed compliance check is regulatory risk. Every delayed invoice is cash flow lost. Every scattered document is a liability waiting to happen.

Most firms still operate on disconnected tools — spreadsheets for time tracking, email for document sharing, separate accounting systems, and manual compliance processes. The result: lost billable hours, compliance failures, poor client service, and operational chaos.

**Clarity changes that.**

### The Business Case

Clarity is a unified platform that captures every billable minute, enforces every compliance requirement, automates every invoice, and gives management real-time visibility across the entire practice — from client onboarding to matter closure.

**For law firm partners:** Clarity means higher revenue capture, reduced compliance risk, and data-driven decision making. Firms using integrated platforms report 15-25% improvement in billable hour recovery and 40% reduction in invoice disputes.

**For operations teams:** One system instead of five. Automated workflows replace manual handoffs. Real-time dashboards replace end-of-month surprises. Audit trails replace "who did what?" investigations.

**For clients:** Transparency. A portal where they see their matter status, access documents, view invoices, and communicate with their legal team — building trust and reducing support overhead.

### The Technical Differentiator

This is not a tutorial project. This is enterprise software engineered for production:

- **Clean Architecture** that separates business rules from technology decisions — the platform can outlive any framework choice
- **CQRS + Event-Driven Workflows** that model how legal work actually flows — matters progress, time gets approved, invoices get issued, payments reconcile — all with validation, permissions, and audit at every step
- **Security-First Design** with JWT refresh rotation, role-based access control, matter-level permissions, session management, and account protection — because legal data is among the most sensitive data that exists
- **Enterprise-Grade Observability** with structured logging, correlation tracking, health monitoring, and diagnostics — because production issues at 2am need answers in minutes, not hours
- **CI/CD and Docker** ready for any deployment target — Azure, AWS, on-premise — with feature flags for safe rollouts and disaster recovery planning

### Who This Is For

| Audience | What Clarity Demonstrates |
|----------|--------------------------|
| **Hiring managers** | Full-stack enterprise engineering capability — not just coding, but architecture, security, testing, DevOps, and documentation |
| **Technical leads** | Production-thinking: audit trails, workflow engines, background processing, error handling, performance considerations |
| **Business stakeholders** | A credible SaaS product with real domain modelling, compliance awareness, and financial workflow understanding |
| **Engineering teams** | A reference implementation of Clean Architecture + CQRS + Angular Signals at enterprise scale with 67 documentation files and 83 automated tests |

### By The Numbers

| Metric | Value |
|--------|-------|
| Source files | 295 |
| Lines of code | 16,688 |
| Automated tests | 83 |
| Documentation files | 67 |
| API endpoints | 40+ |
| Domain entities | 22 |
| Business workflows | 3 (Matter, TimeEntry, Invoice) |
| Architecture Decision Records | 6 |

---

## What is Clarity?

Clarity is a full-stack enterprise platform that manages the complete lifecycle of legal work: client onboarding, matter management, document handling, time recording, billing, payments, compliance, and reporting — all in one secure, auditable system.

It demonstrates how real-world enterprise software is designed, built, tested, deployed, and maintained by professional engineering teams.

---

## Quick Start

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- SQL Server (local instance or Docker)

### Backend

```bash
git clone https://github.com/dotnetdeveloper20xx/clarity-.git
cd clarity-
dotnet restore
dotnet build
dotnet test                              # 83 tests pass
cd src/Clarity.Api
dotnet ef database update                # Create database
dotnet run                               # API at https://localhost:5001
```

### Frontend

```bash
cd src/Clarity.Web
npm install
npm start                                # UI at http://localhost:4200
```

### Docker (Full Stack)

```bash
docker compose up                        # SQL Server + API at http://localhost:5001
```

### Default Login

| Email | Password | Roles |
|-------|----------|-------|
| admin@clarity.local | Admin123! | Admin |
| sarah.johnson@clarity.local | Password1! | Consultant, TeamLeader |
| finance@clarity.local | Password1! | Finance |
| compliance@clarity.local | Password1! | Compliance |

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  Angular 19 (SPA)                        │
│    Tailwind + DaisyUI │ Signals │ Feature Stores        │
└────────────────────────────┬────────────────────────────┘
                             │ REST / JWT
┌────────────────────────────▼────────────────────────────┐
│                 ASP.NET Core API                         │
│   Controllers │ Middleware │ Swagger │ Health Checks     │
└────────────────────────────┬────────────────────────────┘
                             │ MediatR
┌────────────────────────────▼────────────────────────────┐
│              Application Layer (CQRS)                    │
│   Commands │ Queries │ Validators │ Workflows           │
└──────────┬─────────────────────────────────┬────────────┘
           │                                 │
┌──────────▼──────────┐         ┌────────────▼────────────┐
│   Domain Layer      │         │  Infrastructure Layer   │
│ Entities │ Enums    │         │ EF Core │ SQL Server    │
│ Interfaces │ Rules  │         │ Services │ Storage      │
└─────────────────────┘         └─────────────────────────┘
```

**Dependency Rule**: Domain → nothing. Application → Domain. Infrastructure → Application + Domain. API → Application + Infrastructure.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10, C# 13, MediatR, FluentValidation, Serilog |
| Frontend | Angular 19, TypeScript, Tailwind CSS, DaisyUI, Signals |
| Database | SQL Server, Entity Framework Core 9, Code-First Migrations |
| Auth | JWT Bearer + Refresh Token Rotation, RBAC |
| DevOps | Docker, GitHub Actions CI/CD, Multi-environment configs |
| Testing | xUnit, FluentAssertions, 83 tests (unit + architecture) |

---

## Key Features

### Business Capabilities

| Module | Description |
|--------|-------------|
| 👥 Client Management | Create, search, filter clients with full audit trail |
| 📁 Matter Management | Lifecycle workflows (Open → InProgress → Closed) with precondition enforcement |
| ⏱️ Time Recording | Billable/non-billable tracking with approval workflows |
| 💰 Billing | Invoice generation from approved time, issue/pay lifecycle |
| 💳 Payments | Payment recording with automatic invoice status updates |
| 📄 Documents | Upload, version, categorise with secure storage |
| 🛡️ Compliance | AML/KYC checks that gate matter progression |
| 📊 Dashboards | Real-time KPIs, financial summaries, aged debt |
| 🔍 Global Search | Unified search across clients, matters, invoices |
| 📋 Audit Trail | Immutable, searchable record of every significant action |

### Technical Capabilities

| Capability | Implementation |
|-----------|---------------|
| Workflow Engine | Status transition validators with business rule enforcement |
| Background Jobs | Queue-based with retry and dead-letter handling |
| Notifications | In-app with role-based delivery |
| Feature Flags | Configuration-based gradual rollout |
| Correlation Tracking | End-to-end request tracing via X-Correlation-Id |
| CSV Exports | Server-side generation for clients, invoices, time entries |
| Health Checks | SQL Server connectivity monitoring |
| Account Security | Lockout after 5 failed attempts, session management |

---

## Project Structure

```
clarity-/
├── src/
│   ├── Clarity.Domain/           # Entities, enums, interfaces (0 dependencies)
│   ├── Clarity.Application/      # CQRS commands/queries, validators, workflows
│   ├── Clarity.Infrastructure/   # EF Core, SQL Server, services
│   ├── Clarity.Api/              # Controllers, middleware, auth, Swagger
│   └── Clarity.Web/              # Angular 19 SPA
├── tests/
│   └── Clarity.Tests/            # 83 tests (unit, architecture, workflow)
├── docs/                          # 67 documentation files
├── .github/workflows/             # CI + Release pipelines
├── Dockerfile.api                 # Multi-stage Docker build
├── docker-compose.yml             # Full local development stack
└── Clarity.sln                    # Solution file
```

---

## Testing

```bash
dotnet test
```

**83 tests** covering:
- Validator rules (client, time entry)
- Domain entity defaults
- Workflow transitions (matter, time entry, invoice — valid and invalid paths)
- Architecture rules (dependency direction, layer isolation)
- Security (refresh tokens, access levels)
- Financial calculations (invoice totals, payment allocation)

---

## Documentation

The `docs/` folder contains **67 files** covering every aspect of the platform:

| Category | Key Documents |
|----------|--------------|
| Business | business-overview, user-roles, core-workflows, business-rules, glossary |
| Architecture | architecture-overview, domain-model, domain-boundaries, cqrs-strategy, event-strategy |
| Database | database-design, table-catalog, indexing-strategy, soft-delete-strategy |
| API | api-guide, error-handling, authentication-authorization |
| Frontend | frontend-architecture, state-management, angular-folder-structure |
| Workflows | workflow-engine, matter-lifecycle, time-recording-workflow, billing-workflow |
| Security | security-model, permission-matrix, session-management, secure-configuration |
| DevOps | devops-strategy, environment-strategy, release-management, disaster-recovery |
| Quality | testing-strategy, definition-of-done, code-review-guide, technical-debt-register |
| Onboarding | developer-onboarding, how-to-add-a-feature, support-playbooks |
| ADRs | 6 Architecture Decision Records explaining key choices |

---

## API Endpoints

| Area | Endpoints |
|------|-----------|
| Auth | login, refresh, logout, change-password, sessions |
| Clients | CRUD + search + filter |
| Matters | CRUD + status transitions + timeline |
| Time Entries | record, approve, submit, reject |
| Invoices | create (from time), issue, list |
| Payments | record payment |
| Documents | upload |
| Compliance | create checks |
| Dashboard | KPIs, financial summary |
| Search | global search across entities |
| Export | CSV for clients, invoices, time entries |
| Audit | searchable audit log |
| Diagnostics | job monitoring, system info, retry |
| Security | audit log, sessions, locked accounts, unlock |
| Notifications | list, unread count, mark read |
| Workflow | matter transitions, time entry submit/reject |

Full API docs available via Swagger at `/swagger` when running locally.

---

## Development Phases

This project was built in 10 structured phases:

| Phase | Focus | Status |
|-------|-------|--------|
| 1 | Business Discovery & Documentation | ✅ Complete |
| 2 | Domain Modelling & Architecture | ✅ Complete |
| 3 | Database Design & Data Architecture | ✅ Complete |
| 4 | Backend API & Application Layer | ✅ Complete |
| 5 | Angular Frontend & State Management | ✅ Complete |
| 6 | Workflow Engine & Background Processing | ✅ Complete |
| 7 | Reporting, Dashboards & Search | ✅ Complete |
| 8 | Security, Permissions & Data Protection | ✅ Complete |
| 9 | DevOps, Deployment & Production Readiness | ✅ Complete |
| 10 | Testing, Quality Gates & Onboarding | ✅ Complete |

---

## Future Enhancements

- Azure Service Bus for async messaging
- SignalR real-time notifications
- Azure Blob Storage integration
- OpenTelemetry distributed tracing
- Multi-tenant support
- Two-factor authentication
- Document content search (OCR + full-text)
- Mobile responsive optimisation
- Performance testing with realistic data volumes

---

## Contributing

This is a portfolio/learning project. Feel free to fork and adapt for your own use.

---

## Author

Built to demonstrate enterprise software engineering practices — architecture, scalability, security, maintainability, and professional delivery — using modern Microsoft and Angular technologies.

---

## License

[MIT](LICENSE)
