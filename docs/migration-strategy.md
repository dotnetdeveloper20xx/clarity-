# Migration Strategy

## Overview

Database schema changes are managed using **Entity Framework Core Migrations**. This provides:

- Version-controlled schema evolution
- Repeatable deployments across environments
- Rollback capability
- Team collaboration on schema changes

---

## Principles

| Principle | Detail |
|-----------|--------|
| Never modify production schema manually | All changes go through migrations |
| One migration per logical change | Don't combine unrelated changes |
| Always test with realistic data | Migration against empty database tells you nothing about performance |
| Plan rollback before applying | Know how to undo every migration |
| Non-destructive by default | Never drop columns/tables without a deprecation period |
| Forward-only in production | Rollbacks are separate forward migrations |

---

## Migration Workflow

### Development

```
1. Developer modifies entity or configuration
2. Run: dotnet ef migrations add <MigrationName> -p Clarity.Infrastructure -s Clarity.Api
3. Review generated migration code
4. Test migration against local database with seed data
5. Commit migration files to source control
6. PR review includes migration review
```

### Naming Convention

Migrations follow a descriptive naming pattern:

```
YYYYMMDD_HHMM_DescriptiveAction

Examples:
20250301_1430_AddMatterTable
20250305_0900_AddClientEmailIndex
20250310_1100_AddDisbursementsTable
20250315_1600_AlterInvoiceAddTaxRate
```

### Testing

Before applying migrations to any shared environment:

1. Run migration against a copy of production data
2. Verify data integrity after migration
3. Measure migration execution time
4. Verify rollback works
5. Check index creation doesn't lock tables for too long

---

## Environment Promotion

```
Local Dev → Dev/Integration → Staging → Production
```

| Environment | Migration Method | Approval |
|-------------|-----------------|----------|
| Local Dev | Auto-apply on startup (Development mode only) | None |
| Dev/Integration | Applied by CI/CD pipeline | Automatic |
| Staging | Applied by CI/CD pipeline | Team lead approval |
| Production | Applied by deployment pipeline | Release manager approval |

---

## Handling Destructive Changes

### Column Removal

Never drop a column in one migration. Use a three-phase approach:

**Phase 1: Deprecate**
- Stop writing to the column in application code
- Deploy application changes
- Column still exists with data

**Phase 2: Nullify**
- Make column nullable (if not already)
- Set default values
- Deploy and verify nothing breaks

**Phase 3: Remove** (after a safe period, e.g., 2 weeks)
- Drop the column in a new migration
- Deploy after confirming no queries reference it

### Table Removal

Same phased approach:
1. Stop all application access
2. Rename table with a `_deprecated` suffix
3. After a safe period, drop the table

### Data Type Changes

- Widening (e.g., NVARCHAR(100) → NVARCHAR(200)) is safe
- Narrowing (e.g., NVARCHAR(200) → NVARCHAR(100)) requires data validation first
- Type changes (e.g., INT → NVARCHAR) require data migration scripts

---

## Large Table Migrations

For tables with millions of rows:

| Concern | Mitigation |
|---------|-----------|
| Table locks | Use ONLINE index operations where possible |
| Long execution | Run during maintenance windows |
| Transaction log growth | Batch large data changes |
| Timeout | Set appropriate command timeout in migration |

Example for large migrations:

```csharp
// In the migration
migrationBuilder.Sql("SET LOCK_TIMEOUT 30000"); // 30 seconds
migrationBuilder.CreateIndex(
    name: "IX_TimeEntries_Date",
    table: "TimeEntries",
    column: "Date")
    // Note: For production, consider WITH (ONLINE = ON) for Enterprise edition
```

---

## Seed Data Migrations

Seed data (reference data like roles, permissions, matter types) is applied via migrations:

```csharp
// Example: Seed default roles
migrationBuilder.InsertData(
    table: "Roles",
    schema: "security",
    columns: new[] { "Id", "Name", "Description", "IsSystemRole" },
    values: new object[] { Guid.Parse("..."), "SystemAdmin", "Full system access", true }
);
```

This ensures seed data is consistent across all environments.

---

## Migration Checklist

Before merging any migration PR:

- [ ] Migration name follows naming convention
- [ ] Migration code is reviewed (not just auto-generated)
- [ ] Up and Down methods are both implemented
- [ ] Tested against local database with existing data
- [ ] No data loss (unless explicitly approved)
- [ ] Indexes are appropriate (not missing, not excessive)
- [ ] Foreign keys are correct
- [ ] Default values make sense
- [ ] Performance impact assessed for large tables
- [ ] Rollback tested

---

## Emergency Rollback

If a migration causes production issues:

1. **Immediate**: Roll back the application deployment (previous version)
2. **Assess**: Determine if the migration can be reversed
3. **Reverse**: Apply a new forward migration that undoes the change
4. **Never**: Use `dotnet ef database update <previous>` in production (no rollback to previous migration directly)

The rollback migration is a new migration that reverses the changes, ensuring the history remains clean and auditable.

---

## Tools

| Tool | Purpose |
|------|---------|
| `dotnet ef migrations add` | Create new migration |
| `dotnet ef migrations list` | View all migrations |
| `dotnet ef database update` | Apply pending migrations |
| `dotnet ef migrations script` | Generate SQL script for review |
| `dotnet ef migrations remove` | Remove last unapplied migration |
