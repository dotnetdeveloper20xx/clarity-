# ADR-001: Clean Architecture

## Status
Accepted

## Context
We need an architecture that supports long-term maintainability, testability, and team scalability for a mission-critical legal platform expected to run for 10+ years.

## Decision
Use Clean Architecture with four main layers: Domain, Application, Infrastructure, Api.

## Consequences
- Business logic is isolated and testable without databases
- Technology can be replaced without rewriting business rules
- New developers can understand the system by layer
- Slightly more files and folders than simpler approaches
- Trade-off accepted: clarity over brevity
