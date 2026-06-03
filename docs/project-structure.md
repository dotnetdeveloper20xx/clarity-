# Project Structure

## Solution Layout

```
Clarity/
├── Clarity.sln
├── docs/                              # Documentation (business, architecture, guides)
├── src/
│   ├── Clarity.Domain/                # Domain layer (entities, enums, interfaces)
│   ├── Clarity.Application/           # Application layer (CQRS, validators, workflows)
│   ├── Clarity.Infrastructure/        # Infrastructure layer (EF Core, repositories, external services)
│   ├── Clarity.Api/                   # API layer (controllers, middleware, startup)
│   └── Clarity.Web/                   # Frontend (Angular application)
├── tests/
│   ├── Clarity.Domain.Tests/          # Domain unit tests
│   ├── Clarity.Application.Tests/     # Application handler tests
│   ├── Clarity.Infrastructure.Tests/  # Integration tests
│   └── Clarity.Api.Tests/             # API integration tests
└── tools/                             # Build scripts, seed data, utilities
```

---

## Clarity.Domain

The innermost layer. Contains business entities, value objects, enums, and interfaces. Has zero dependencies on external packages (except language essentials).

```
Clarity.Domain/
├── Common/
│   ├── BaseEntity.cs                  # Base class with Id, audit fields, soft delete
│   ├── IAuditableEntity.cs            # Interface for auditable entities
│   ├── ISoftDeletable.cs              # Interface for soft-deletable entities
│   └── ValueObject.cs                 # Base value object
├── Entities/
│   ├── Client.cs
│   ├── Contact.cs
│   ├── Matter.cs
│   ├── MatterNote.cs
│   ├── MatterTask.cs
│   ├── MatterTeamMember.cs
│   ├── TimeEntry.cs
│   ├── Invoice.cs
│   ├── InvoiceLineItem.cs
│   ├── Payment.cs
│   ├── Document.cs
│   ├── ComplianceCheck.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── Permission.cs
│   ├── UserRole.cs
│   ├── RolePermission.cs
│   ├── AuditEntry.cs
│   ├── Notification.cs
│   └── BillingRate.cs
├── Enums/
│   ├── ClientType.cs
│   ├── ClientStatus.cs
│   ├── MatterType.cs
│   ├── MatterStatus.cs
│   ├── FeeArrangement.cs
│   ├── TimeEntryStatus.cs
│   ├── InvoiceStatus.cs
│   ├── PaymentMethod.cs
│   ├── DocumentStatus.cs
│   ├── TaskStatus.cs
│   ├── TaskPriority.cs
│   ├── ComplianceCheckType.cs
│   ├── ComplianceCheckStatus.cs
│   └── NotificationType.cs
├── ValueObjects/
│   ├── Address.cs
│   ├── Money.cs
│   └── DateRange.cs
├── Events/
│   ├── ClientCreatedEvent.cs
│   ├── MatterCreatedEvent.cs
│   ├── MatterClosedEvent.cs
│   ├── TimeEntryRecordedEvent.cs
│   ├── InvoiceIssuedEvent.cs
│   ├── PaymentReceivedEvent.cs
│   ├── DocumentUploadedEvent.cs
│   └── ComplianceCheckCompletedEvent.cs
└── Interfaces/
    ├── IRepository.cs                 # Generic repository interface
    ├── IUnitOfWork.cs                 # Unit of work interface
    ├── ICurrentUserService.cs         # Current user context
    ├── IDateTimeService.cs            # Abstraction over DateTime.UtcNow
    └── IFileStorageService.cs         # Blob storage abstraction
```

---

## Clarity.Application

Contains use cases, CQRS commands/queries, validators, and pipeline behaviours. Depends only on Domain.

```
Clarity.Application/
├── Common/
│   ├── Behaviours/
│   │   ├── LoggingBehaviour.cs
│   │   ├── ValidationBehaviour.cs
│   │   ├── AuthorisationBehaviour.cs
│   │   ├── AuditBehaviour.cs
│   │   └── PerformanceBehaviour.cs
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   └── IEmailService.cs
│   ├── Models/
│   │   ├── Result.cs                  # Generic result wrapper
│   │   ├── PaginatedList.cs           # Paginated response
│   │   └── SortOrder.cs
│   ├── Mappings/
│   │   └── MappingProfile.cs          # AutoMapper profiles
│   └── Exceptions/
│       ├── ValidationException.cs
│       ├── NotFoundException.cs
│       └── ForbiddenException.cs
├── Clients/
│   ├── Commands/
│   │   ├── CreateClient/
│   │   │   ├── CreateClientCommand.cs
│   │   │   ├── CreateClientCommandHandler.cs
│   │   │   └── CreateClientCommandValidator.cs
│   │   ├── UpdateClient/
│   │   └── ArchiveClient/
│   ├── Queries/
│   │   ├── GetClient/
│   │   │   ├── GetClientQuery.cs
│   │   │   ├── GetClientQueryHandler.cs
│   │   │   └── ClientDto.cs
│   │   ├── GetClients/
│   │   └── SearchClients/
│   └── EventHandlers/
│       └── ClientCreatedEventHandler.cs
├── Matters/
│   ├── Commands/
│   │   ├── CreateMatter/
│   │   ├── UpdateMatter/
│   │   ├── CloseMatter/
│   │   └── AssignTeamMember/
│   ├── Queries/
│   │   ├── GetMatter/
│   │   ├── GetMatters/
│   │   └── GetMyMatters/
│   └── EventHandlers/
├── TimeRecording/
│   ├── Commands/
│   │   ├── RecordTime/
│   │   ├── ApproveTimeEntry/
│   │   └── RevertTimeEntry/
│   ├── Queries/
│   │   ├── GetTimeEntries/
│   │   └── GetUnbilledTime/
│   └── EventHandlers/
├── Billing/
│   ├── Commands/
│   │   ├── GenerateInvoice/
│   │   ├── IssueInvoice/
│   │   └── WriteOffInvoice/
│   ├── Queries/
│   │   ├── GetInvoice/
│   │   └── GetOutstandingInvoices/
│   └── EventHandlers/
├── Payments/
│   ├── Commands/
│   │   ├── RecordPayment/
│   │   └── ReversePayment/
│   ├── Queries/
│   │   └── GetPayments/
│   └── EventHandlers/
├── Documents/
│   ├── Commands/
│   │   ├── UploadDocument/
│   │   └── ArchiveDocument/
│   ├── Queries/
│   │   ├── GetDocument/
│   │   └── GetMatterDocuments/
│   └── EventHandlers/
├── Compliance/
│   ├── Commands/
│   │   └── CreateComplianceCheck/
│   ├── Queries/
│   │   └── GetComplianceChecks/
│   └── EventHandlers/
├── Security/
│   ├── Commands/
│   │   ├── CreateUser/
│   │   └── AssignRole/
│   ├── Queries/
│   │   └── GetUsers/
│   └── EventHandlers/
└── Reporting/
    └── Queries/
        ├── GetDashboard/
        ├── GetFinancialSummary/
        └── GetTeamWorkload/
```

---

## Clarity.Infrastructure

Implements interfaces defined in Domain/Application. Contains all external dependencies.

```
Clarity.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs        # EF Core DbContext
│   ├── Configurations/                # Entity type configurations (Fluent API)
│   │   ├── ClientConfiguration.cs
│   │   ├── MatterConfiguration.cs
│   │   ├── TimeEntryConfiguration.cs
│   │   ├── InvoiceConfiguration.cs
│   │   └── ...
│   ├── Migrations/                    # EF Core migrations
│   ├── Repositories/
│   │   └── GenericRepository.cs
│   ├── Interceptors/
│   │   ├── AuditableEntityInterceptor.cs
│   │   └── SoftDeleteInterceptor.cs
│   └── Seeds/
│       └── ApplicationDbContextSeed.cs
├── Services/
│   ├── DateTimeService.cs
│   ├── CurrentUserService.cs
│   ├── EmailService.cs
│   └── FileStorageService.cs
├── Identity/
│   ├── IdentityService.cs
│   └── JwtTokenService.cs
└── DependencyInjection.cs             # Infrastructure service registration
```

---

## Clarity.Api

The ASP.NET Core Web API project. Contains controllers, middleware, and startup configuration.

```
Clarity.Api/
├── Controllers/
│   ├── ClientsController.cs
│   ├── MattersController.cs
│   ├── TimeEntriesController.cs
│   ├── InvoicesController.cs
│   ├── PaymentsController.cs
│   ├── DocumentsController.cs
│   ├── ComplianceController.cs
│   ├── UsersController.cs
│   ├── ReportsController.cs
│   └── AuditController.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   ├── CorrelationIdMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Filters/
│   └── ApiExceptionFilterAttribute.cs
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── DependencyInjection.cs             # API service registration
```

---

## Clarity.Web (Angular)

The Angular frontend application.

```
Clarity.Web/
├── src/
│   ├── app/
│   │   ├── core/                      # Singleton services, guards, interceptors
│   │   │   ├── auth/
│   │   │   ├── interceptors/
│   │   │   ├── guards/
│   │   │   └── services/
│   │   ├── shared/                    # Shared components, pipes, directives
│   │   │   ├── components/
│   │   │   ├── pipes/
│   │   │   └── directives/
│   │   ├── features/                  # Feature modules (lazy loaded)
│   │   │   ├── dashboard/
│   │   │   ├── clients/
│   │   │   ├── matters/
│   │   │   ├── time-recording/
│   │   │   ├── billing/
│   │   │   ├── documents/
│   │   │   ├── compliance/
│   │   │   ├── reports/
│   │   │   └── admin/
│   │   ├── layout/                    # Shell, sidebar, header, footer
│   │   └── app.routes.ts
│   ├── assets/
│   ├── environments/
│   └── styles/
├── angular.json
├── package.json
├── tailwind.config.js
└── tsconfig.json
```

---

## Tests

```
tests/
├── Clarity.Domain.Tests/              # Entity business rule tests
├── Clarity.Application.Tests/         # Command/query handler unit tests
├── Clarity.Infrastructure.Tests/      # Database integration tests
└── Clarity.Api.Tests/                 # API endpoint integration tests
```

---

## Dependency Graph

```
Clarity.Api ──────────► Clarity.Application ──────────► Clarity.Domain
     │                         ▲
     │                         │
     └──► Clarity.Infrastructure ──────────────────────► Clarity.Domain
```

- Api references Application and Infrastructure (for DI registration)
- Application references Domain only
- Infrastructure references Domain and Application (implements interfaces)
- Domain references nothing
