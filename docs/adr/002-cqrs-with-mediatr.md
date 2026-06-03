# ADR-002: CQRS with MediatR

## Status
Accepted

## Context
We need a pattern for handling commands (writes) and queries (reads) that supports validation, logging, and auditing consistently across all operations.

## Decision
Use MediatR to implement CQRS with pipeline behaviours for cross-cutting concerns.

## Consequences
- Every operation goes through a consistent pipeline (logging → validation → handling)
- Handlers are small, focused, and testable
- New cross-cutting concerns (caching, auth) can be added as behaviours
- Slight indirection (controller → MediatR → handler) adds one hop
- Trade-off accepted: consistency and extensibility over simplicity
