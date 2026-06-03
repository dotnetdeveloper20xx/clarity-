# How to Add a Feature

This guide walks through adding a new feature end-to-end across all layers.

## Example: Adding "Disbursements" to a Matter

### 1. Domain Entity

Create `src/Clarity.Domain/Entities/Disbursement.cs`:

```csharp
public class Disbursement : AuditableEntity
{
    public Guid MatterId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public bool IsBilled { get; set; }
    public Guid? InvoiceId { get; set; }
    
    public Matter Matter { get; set; } = null!;
}
```

### 2. EF Configuration

Create `src/Clarity.Infrastructure/Persistence/Configurations/DisbursementConfiguration.cs`:

```csharp
public class DisbursementConfiguration : IEntityTypeConfiguration<Disbursement>
{
    public void Configure(EntityTypeBuilder<Disbursement> builder)
    {
        builder.ToTable("Disbursements");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Amount).HasColumnType("decimal(18,2)");
        // ... indexes, relationships
    }
}
```

### 3. Add DbSet

In `IApplicationDbContext.cs` and `ApplicationDbContext.cs`:
```csharp
DbSet<Disbursement> Disbursements { get; }
```

### 4. Create Migration

```bash
dotnet ef migrations add AddDisbursements --project src/Clarity.Infrastructure --startup-project src/Clarity.Api
```

### 5. Command (Write)

Create folder: `src/Clarity.Application/Disbursements/Commands/CreateDisbursement/`

Files:
- `CreateDisbursementCommand.cs` — the request DTO
- `CreateDisbursementCommandValidator.cs` — FluentValidation rules
- `CreateDisbursementCommandHandler.cs` — business logic

### 6. Query (Read)

Create folder: `src/Clarity.Application/Disbursements/Queries/GetDisbursements/`

Files:
- `GetDisbursementsQuery.cs` — query with filters
- `GetDisbursementsQueryHandler.cs` — data retrieval
- `DisbursementDto.cs` — response shape

### 7. API Controller

Create `src/Clarity.Api/Controllers/DisbursementsController.cs`:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisbursementsController : ControllerBase
{
    // Inject IMediator, delegate to commands/queries
}
```

### 8. Angular API Service

Create `src/Clarity.Web/src/app/core/services/disbursement-api.service.ts`

### 9. Angular Store (if needed)

Create `src/Clarity.Web/src/app/core/stores/disbursement.store.ts`

### 10. Angular Component

Create `src/Clarity.Web/src/app/features/disbursements/`

### 11. Route

Add to `app.routes.ts`

### 12. Tests

- Validator tests in `tests/Clarity.Tests/Application/Disbursements/`
- Ensure existing tests still pass: `dotnet test`

### 13. Documentation

Update relevant docs if the feature is significant.

## Checklist for Every Feature

- [ ] Domain entity created (if new entity)
- [ ] EF configuration created
- [ ] Migration generated and reviewed
- [ ] Command + Validator + Handler created
- [ ] Query + DTO + Handler created
- [ ] Controller endpoint created
- [ ] Angular API service method added
- [ ] Angular store method added (if stateful)
- [ ] Angular component/page created
- [ ] Route added
- [ ] Tests written
- [ ] All existing tests still pass
- [ ] PR created and reviewed
