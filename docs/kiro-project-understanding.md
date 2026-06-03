# Kiro Project Understanding

## High-Level Business Understanding

**Clarity** is an enterprise legal practice management platform designed for modern legal firms. It manages the full lifecycle of legal work: client onboarding, matter management, document handling, time recording, billing, payments, compliance, and reporting.

The platform serves 8 user roles (Client, Consultant/Solicitor, Legal Assistant, Team Leader, Finance, Compliance, System Administrator, Support) and is designed as a mission-critical system expected to operate for 10+ years.

Revenue generation is through billable time, fixed fees, and disbursement pass-through. The platform ensures every billable minute is captured and converted to invoiced revenue.

---

## Technical Architecture Summary

- **Architecture Style:** Clean Architecture (Domain → Application → Infrastructure → API)
- **Backend:** ASP.NET Core (Latest LTS), C#, Entity Framework Core, MediatR (CQRS), FluentValidation, AutoMapper, Serilog, OpenTelemetry, Swagger/OpenAPI
- **Database:** SQL Server with EF Core Migrations, soft-delete strategy, optimistic concurrency, GUID primary keys
- **Frontend:** Angular (Latest), TypeScript, Tailwind CSS, DaisyUI, RxJS, Angular Signals
- **Cloud-Ready:** Azure App Services, Azure SQL, Azure Blob Storage, Azure Service Bus, Azure Key Vault, Azure Application Insights (all optional; must run locally without Azure)
- **Patterns:** CQRS, Domain Events, Repository Pattern, Pipeline Behaviours, Feature Stores (Angular)

---

## Project Inventory

### Current State

| Item | Status |
|------|--------|
| Solution/Code | Not yet created |
| Planning Documents | Complete (10 phases defined) |
| Business Documentation (Phase 1) | Complete |
| Architecture Documentation (Phase 2) | Complete |
| Database Design Documentation (Phase 3) | Complete |
| Backend Implementation (Phase 4) | Not started |
| Frontend Implementation (Phase 5) | Not started |
| Workflows/Background Jobs (Phase 6) | Not started |
| Reporting/Dashboards (Phase 7) | Not started |
| Security/Permissions (Phase 8) | Not started |
| DevOps/Deployment (Phase 9) | Not started |
| Testing/Quality (Phase 10) | Not started |

### Existing Documentation (docs/)

- business-overview.md, user-roles.md, core-workflows.md, business-rules.md, non-functional-requirements.md, glossary.md
- architecture-overview.md, domain-model.md, domain-boundaries.md, cqrs-strategy.md, event-strategy.md, project-structure.md, system-context.md
- database-design.md, entity-relationship-overview.md, table-catalog.md, indexing-strategy.md, audit-strategy.md, soft-delete-strategy.md, reporting-data-strategy.md, data-retention-policy.md, migration-strategy.md

---

## Key Dependencies (Planned)

### Backend (.NET)
- Microsoft.AspNetCore (framework)
- Microsoft.EntityFrameworkCore + SqlServer provider
- MediatR
- FluentValidation
- Serilog + sinks (Console, File, Application Insights)
- AutoMapper
- Swashbuckle (Swagger)
- Microsoft.AspNetCore.Authentication.JwtBearer
- OpenTelemetry SDKs
- Microsoft.Extensions.Diagnostics.HealthChecks

### Frontend (Angular/Node)
- @angular/core (latest)
- @angular/router, @angular/forms, @angular/common/http
- tailwindcss
- daisyui
- rxjs
- typescript

### Database
- SQL Server (LocalDB or Docker for development)

---

## Build Process

**Backend:**
```
dotnet restore
dotnet build
dotnet test
dotnet ef database update (migrations)
dotnet run (API project)
```

**Frontend:**
```
npm install
npm run build (ng build)
npm run start (ng serve)
npm run test (ng test)
npm run lint (ng lint)
```

---

## Test Process

- **Unit Tests:** Domain and Application layer logic (xUnit or NUnit)
- **Integration Tests:** EF Core with in-memory or test database
- **API Tests:** WebApplicationFactory-based integration tests
- **Frontend Tests:** Karma/Jest for Angular components and services
- **Architecture Tests:** NetArchTest or similar for dependency rule enforcement

---

## Deployment Process (Planned)

- Local development with LocalDB/Docker SQL Server
- CI/CD pipelines (GitHub Actions or Azure DevOps)
- Environment promotion: Local → Dev → Test → UAT → Production
- Database migrations applied via pipeline with review gates
- Docker support for consistent local development
- Azure deployment target (optional)

---

## Important Coding Conventions

- Clean Architecture dependency rule: Domain depends on nothing, Application depends on Domain only
- CQRS naming: `{Verb}{Entity}Command`, `Get{Entity}Query`
- Thin controllers — all logic in MediatR handlers
- FluentValidation for every command
- Soft-delete via `IsDeleted`, `DeletedAt`, `DeletedBy`
- Audit fields on all entities: `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`
- GUID primary keys with NEWSEQUENTIALID()
- UTC for all timestamps
- DECIMAL(18,2) for all monetary values
- Structured logging with correlation IDs
- Feature-folder organisation in Application layer
- Feature stores in Angular (not global monolithic state)

---

## Risks and Assumptions

| Risk/Assumption | Detail |
|----------------|--------|
| .NET SDK version | SDK 10.0.300 installed; will target net10.0 or latest LTS |
| SQL Server availability | Assumes LocalDB or Docker SQL Server available locally |
| Node.js availability | Assumes Node.js and npm are installed for Angular |
| No existing code | Greenfield project — no legacy constraints |
| Single-tenant | Designed for one legal firm per deployment (multi-tenancy future) |
| Phase-based delivery | Each phase must be complete before the next begins |

---

## Areas Requiring Future Attention

- Multi-tenancy architecture (if needed)
- Azure deployment infrastructure
- Production-grade email integration
- Payment gateway integration
- Document virus scanning service
- SignalR real-time updates
- Performance testing with realistic data volumes
- GDPR data subject access request automation
- Two-factor authentication implementation
- External identity provider integration (Azure AD/OAuth)
