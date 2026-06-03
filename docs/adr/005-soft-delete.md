# ADR-005: Soft Delete Strategy

## Status
Accepted

## Context
Legal and financial data must be preserved for compliance, audit, and potential recovery. Hard deletes risk data loss and broken audit trails.

## Decision
All business entities use soft delete (IsDeleted flag). EF Core global query filters automatically exclude deleted records.

## Consequences
- No data ever physically lost
- Audit trails remain complete
- Recovery is possible without database backups
- Database grows larger over time (mitigated by archival strategy)
- All queries automatically filter deleted records (transparent to developers)
- Trade-off: storage cost vs. data safety — data safety wins for legal software
