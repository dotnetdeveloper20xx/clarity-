# Stored Procedures

These stored procedures provide optimised, paginated, sorted, and filtered data access for the most performance-critical queries in the Clarity platform.

## Why Stored Procedures?

While Entity Framework Core LINQ handles most queries adequately, stored procedures are used for:

1. **Complex reporting queries** that benefit from SQL Server's query optimiser
2. **Dashboard aggregations** that combine data from multiple tables in a single round-trip
3. **High-traffic list screens** where pagination and sorting must be as fast as possible
4. **Financial calculations** where precision and performance are critical

## Available Procedures

| Procedure | Purpose | Used By |
|-----------|---------|---------|
| `sp_GetClients` | Paginated, sorted, filtered client list | Clients page |
| `sp_GetMatters` | Paginated, sorted, filtered matter list with joins | Matters page |
| `sp_GetTimeEntries` | Paginated time entries with calculated billable value | Time Recording page |
| `sp_GetDashboardKPIs` | All dashboard metrics in a single call | Dashboard page |
| `sp_GetAgedDebt` | Aged debt analysis with band grouping | Financial dashboard |
| `sp_GetConsultantProductivity` | Fee earner utilisation and revenue reporting | Management reports |

## Deployment

Apply these procedures to the database after migrations:

```sql
-- From SQL Server Management Studio or sqlcmd:
:r sp_GetClients.sql
:r sp_GetMatters.sql
:r sp_GetTimeEntries.sql
:r sp_GetDashboardKPIs.sql
:r sp_GetAgedDebt.sql
:r sp_GetConsultantProductivity.sql
```

Or run them individually against the target database.

## Integration

These procedures can be called from the Application layer via:

```csharp
// Using ADO.NET for stored procedures (when EF Core LINQ is not optimal)
var results = await connection.QueryAsync<MatterDto>("sp_GetMatters", parameters, commandType: CommandType.StoredProcedure);
```

Currently, the LINQ-based queries in the Application layer handle all operations. These stored procedures are provided as performance-optimised alternatives that can be swapped in when data volumes require them (typically >100K records per table).
