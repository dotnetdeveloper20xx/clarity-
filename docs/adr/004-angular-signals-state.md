# ADR-004: Angular Signals for State Management

## Status
Accepted

## Context
The frontend needs predictable, reactive state management. Options: NgRx, NGXS, Akita, Angular Signals.

## Decision
Use Angular Signals in feature stores for lightweight reactive state. No external state management library.

## Consequences
- Zero external dependencies for state management
- Simpler mental model (signals are just reactive variables)
- Less boilerplate than NgRx (no actions, reducers, effects)
- Built into Angular (no maintenance risk from third-party library)
- Can upgrade to NgRx later if complexity demands it
- Trade-off: less opinionated structure (mitigated by consistent store pattern)
