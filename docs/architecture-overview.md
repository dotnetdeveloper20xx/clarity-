# Architecture Overview

## Architectural Style

Clarity uses **Clean Architecture** (also known as Hexagonal or Onion Architecture). This ensures:

- Business logic is isolated from infrastructure concerns
- The system is testable without databases or external services
- Technology choices (SQL Server, Angular, Azure) can be changed without rewriting business logic
- New developers can understand the system by reading the domain and application layers first

## High-Level Structure

```
┌─────────────────────────────────────────────────────────────────────┐
│                           Clients                                    │
│         (Browser, Mobile, External Systems, Background Jobs)         │
└──────────────────────────────────┬──────────────────────────────────┘
                                   │
┌──────────────────────────────────▼──────────────────────────────────┐
│                      LegalPlatform.Api                               │
│         Controllers, Middleware, Authentication, Swagger             │
└──────────────────────────────────┬──────────────────────────────────┘
                                   │
┌──────────────────────────────────▼──────────────────────────────────┐
│                   LegalPlatform.Application                          │
│       Commands, Queries, Handlers, Validators, Workflows            │
└──────────────┬───────────────────────────────────────┬──────────────┘
               │                                       │
┌──────────────▼──────────────┐   ┌───────────────────▼──────────────┐
│   LegalPlatform.Domain      │   │   LegalPlatform.Infrastructure   │
│   Entities, Value Objects,  │   │   EF Core, Repositories,         │
│   Enums, Interfaces,        │   │   Blob Storage, Email,           │
│   Business Rules            │   │   External APIs                  │
└─────────────────────────────┘   └──────────────────────────────────┘
                                                   │
                                   ┌───────────────▼──────────────────┐
                                   │         SQL Server / Azure        │
                                   │   Database, Blob, Service Bus     │
                                   └──────────────────────────────────┘
```

## Dependency Rule

Dependencies point inward only:

- **Domain** depends on nothing (no NuGet packages except language essentials)
- **Application** depends on Domain
- **Infrastructure** depends on Application and Domain
- **Api** depends on Application (and registers Infrastructure via DI)
- **Web** (Angular) communicates with Api via HTTP only

This means:
- Domain never references Entity Framework, SQL Server, or Angular
- Application never references Infrastructure directly (uses interfaces)
- Infrastructure implements interfaces defined in Application/Domain

## Request Flow

A typical request flows through the system as follows:

```
1. Browser sends HTTP request
2. API Controller receives request
3. Controller creates a Command or Query object
4. MediatR dispatches to the appropriate Handler
5. Handler executes business logic (using Domain entities)
6. Handler calls Infrastructure (via interfaces) for persistence
7. Infrastructure interacts with SQL Server / Blob Storage / etc.
8. Result flows back up through the layers
9. Controller returns HTTP response
```

## Cross-Cutting Concerns

These concerns span all layers and are implemented as middleware or decorators:

| Concern | Implementation |
|---------|---------------|
| Logging | Serilog with structured logging, correlation IDs |
| Exception Handling | Global exception middleware with problem details |
| Validation | FluentValidation via MediatR pipeline behaviour |
| Auditing | MediatR pipeline behaviour + domain events |
| Authentication | ASP.NET Core Identity + JWT tokens |
| Authorisation | Policy-based authorisation with role checks |
| Caching | In-memory + distributed cache (Redis-ready) |
| Monitoring | OpenTelemetry + health check endpoints |
| Tracing | Distributed tracing with correlation IDs |

## Technology Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Backend Framework | ASP.NET Core (Latest LTS) | Enterprise-grade, high performance, long-term support |
| Language | C# | Strong typing, mature ecosystem, excellent tooling |
| ORM | Entity Framework Core | Productivity, migrations, LINQ, well-supported |
| CQRS Mediator | MediatR | Decouples handlers, enables pipeline behaviours |
| Validation | FluentValidation | Readable, testable validation rules |
| Mapping | AutoMapper | Convention-based mapping between layers |
| Logging | Serilog | Structured logging, multiple sinks, performant |
| Observability | OpenTelemetry | Vendor-neutral, standardised tracing and metrics |
| API Documentation | Swagger / OpenAPI | Self-documenting APIs, client generation |
| Frontend | Angular (Latest) | Enterprise SPA, TypeScript, modularity |
| CSS Framework | Tailwind CSS + DaisyUI | Utility-first, consistent design system |
| Database | SQL Server | Enterprise-grade, Azure-compatible, mature |
| Cloud | Azure (optional) | App Services, SQL, Blob, Service Bus, Key Vault |

## Design Principles Applied

- **SOLID** — Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **DRY** — Don't Repeat Yourself (shared abstractions for common patterns)
- **YAGNI** — You Aren't Gonna Need It (build what's required, not what might be needed)
- **Separation of Concerns** — Each layer has a clear, single responsibility
- **Dependency Injection** — All dependencies injected, never constructed directly
- **Composition over Inheritance** — Prefer composing behaviours over deep hierarchies
