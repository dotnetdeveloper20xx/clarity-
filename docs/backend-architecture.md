# Backend Architecture

## Solution Structure

```
Clarity.sln
├── src/
│   ├── Clarity.Domain/          # Business entities, enums, value objects, interfaces
│   ├── Clarity.Application/     # CQRS commands, queries, handlers, validators, behaviours
│   ├── Clarity.Infrastructure/  # EF Core DbContext, configurations, external services
│   └── Clarity.Api/             # Controllers, middleware, Program.cs, Swagger
└── tests/
    └── Clarity.Tests/           # Unit and integration tests
```

## Dependency Direction

```
Api → Application → Domain
Api → Infrastructure → Application → Domain
```

Domain has zero external dependencies. Application depends only on Domain (+ MediatR, FluentValidation, EF Core abstractions). Infrastructure implements Application interfaces using EF Core and SQL Server.

## Request Pipeline

```
HTTP Request
  → CorrelationIdMiddleware (assigns tracking ID)
  → ExceptionHandlingMiddleware (catches and formats errors)
  → Authentication/Authorization
  → Controller (thin — creates command/query, delegates to MediatR)
  → MediatR Pipeline:
      → LoggingBehaviour (logs request + timing)
      → ValidationBehaviour (FluentValidation rules)
      → Handler (business logic)
  → Response
```

## Key Patterns

### CQRS with MediatR

- **Commands** modify state: `CreateClientCommand`, `ApproveTimeEntryCommand`
- **Queries** read state: `GetClientQuery`, `GetInvoicesQuery`
- Each has a Handler that contains the business logic
- Commands have Validators (FluentValidation)

### Folder-by-Feature

```
Application/
├── Clients/
│   ├── Commands/CreateClient/
│   │   ├── CreateClientCommand.cs
│   │   ├── CreateClientCommandHandler.cs
│   │   └── CreateClientCommandValidator.cs
│   └── Queries/GetClients/
│       ├── GetClientsQuery.cs
│       └── GetClientsQueryHandler.cs
├── Matters/
├── TimeEntries/
├── Invoices/
├── Payments/
├── Documents/
└── Compliance/
```

### Soft Delete

Entities implementing `ISoftDeletable` are never physically removed. EF Core global query filters automatically exclude deleted records.

### Audit Fields

All entities inherit from `BaseEntity` which includes `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`.

## Configuration

- Connection string: `appsettings.json` → `ConnectionStrings:DefaultConnection`
- JWT settings: `appsettings.json` → `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- Logging: Serilog configured in Program.cs, outputs to console and rolling file

## Running Locally

1. Ensure SQL Server LocalDB is available
2. From the `src/Clarity.Api` directory: `dotnet run`
3. Navigate to `https://localhost:5001/swagger`
4. Login via POST `/api/auth/login` to get a JWT token
5. Use the token in Swagger's Authorize button
