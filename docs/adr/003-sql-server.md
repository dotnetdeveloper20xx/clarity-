# ADR-003: SQL Server as Primary Database

## Status
Accepted

## Context
The platform stores legal, financial, and compliance data requiring strong ACID guarantees, complex queries, and enterprise support.

## Decision
Use SQL Server as the primary relational database.

## Consequences
- Enterprise-grade reliability and support
- Azure SQL available for cloud deployment
- Strong tooling (SSMS, Azure Data Studio)
- Licence cost for production (mitigated by Azure SQL pricing)
- Vendor lock-in to Microsoft ecosystem (acceptable given .NET stack)
