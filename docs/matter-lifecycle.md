# Matter Lifecycle

## Status Flow

```
Open → InProgress → OnHold ←→ Open/InProgress
  │         │
  │         ├→ AwaitingClient → Open/InProgress
  │         │
  │         ├→ AwaitingThirdParty → Open/InProgress
  │         │
  │         └→ BillingReview → InProgress / Closed
  │
  └→ Closed → Archived (terminal)
```

## Status Descriptions

| Status | Meaning |
|--------|---------|
| Open | Matter has been created, work can begin |
| InProgress | Active legal work underway |
| OnHold | Paused temporarily (awaiting instructions, funding, etc.) |
| AwaitingClient | Blocked waiting for client action |
| AwaitingThirdParty | Blocked waiting for external party |
| BillingReview | Work complete, reviewing for final billing |
| Closed | Matter complete, all obligations fulfilled |
| Archived | Historical record, fully read-only |

## Transition Rules

### Closing a Matter

Required conditions:
- All invoices must be Paid, Written Off, or Cancelled
- No unresolved compliance issues (Fail or RequiresInvestigation)
- Only **Team Leaders** or **Admins** can close

If conditions are not met, the API returns a clear error message explaining what must be resolved first.

### Reopening a Closed Matter

- Only **Admins** can reopen (handled as special case)
- Requires a reason (audited)
- Resets status to InProgress

## Audit Trail

Every transition records:
- Old status → New status
- Who performed it
- When
- Reason (if provided)
- Correlation ID

## Notifications

- Lead consultant is notified when their matter is closed
- Team is notified when matter goes on hold
- Finance is notified when matter enters billing review
