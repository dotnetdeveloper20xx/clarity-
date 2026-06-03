# Workflow Engine

## Overview

Clarity's workflow engine enforces business rules during status transitions. It ensures that entities move through their lifecycle in valid, audited, and permission-checked steps.

## Principles

Every workflow transition:
1. **Validates the transition** — only allowed state changes are permitted
2. **Checks permissions** — certain transitions require specific roles
3. **Enforces preconditions** — business rules must be satisfied
4. **Records audit** — every transition is permanently logged
5. **Creates timeline events** — human-readable activity history
6. **Sends notifications** — relevant users are informed

## Implementation Pattern

Each workflow domain has:
- A **StatusTransitionValidator** — defines the allowed transitions (static class)
- A **Command + Handler** — executes the transition with full business logic
- **Tests** — verify every valid and invalid transition path

## Workflow Domains

| Domain | Status Flow |
|--------|-------------|
| Matter | Open → InProgress → OnHold/Awaiting → BillingReview → Closed → Archived |
| Time Entry | Draft → Submitted → Approved/Rejected → Billed/WrittenOff |
| Invoice | Draft → Issued → PartiallyPaid → Paid / Cancelled / WrittenOff |
| Compliance | Pending → InReview → Pass/Fail/Escalated |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/workflow/matters/{id}/transition | Change matter status |
| POST | /api/workflow/time-entries/{id}/submit | Submit time entry for approval |
| POST | /api/workflow/time-entries/{id}/reject | Reject time entry with reason |
| PUT | /api/timeentries/{id}/approve | Approve a time entry |
| PUT | /api/invoices/{id}/issue | Issue a draft invoice |

## Adding a New Workflow

1. Create `StatusTransitionValidator` in `Application/Workflows/{Domain}/`
2. Define allowed transitions as a dictionary
3. Create command + handler that calls the validator
4. Add audit, timeline, and notification calls
5. Write tests for all valid and invalid transitions
6. Add API endpoint in controller
