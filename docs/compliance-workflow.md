# Compliance Workflow

## Status Flow

```
Pending → Pass (allows matter progression)
       ↘ Fail (blocks matter closure)
       ↘ RequiresInvestigation (escalated)
```

## Check Types

| Type | When Triggered | Purpose |
|------|---------------|---------|
| AML | New client onboarding | Anti-money laundering verification |
| KYC | New client onboarding | Know Your Customer identity check |
| ConflictOfInterest | New matter creation | Ensure no conflicting interests |
| RiskAssessment | Periodic or trigger-based | Overall risk evaluation |

## Business Rules

1. **Compliance checks are mandatory** for new clients before matters can be opened
2. **Failed checks block matter progression** — matter cannot be closed
3. **Only Compliance Officers or Admins** can perform and complete checks
4. **Completed checks cannot be modified** — they are permanent records
5. **Periodic re-checks** can be scheduled via NextReviewDate
6. **High-risk clients** are flagged for enhanced due diligence

## Risk Levels

| Level | Colour | Meaning |
|-------|--------|---------|
| Low | Green | Standard monitoring |
| Medium | Amber | Enhanced monitoring |
| High | Red | Enhanced due diligence required |
| Critical | Dark Red | Immediate escalation required |

## Impact on Other Workflows

- **Matter creation**: Blocked if client has failed/pending mandatory checks
- **Matter closure**: Blocked if unresolved compliance issues exist
- **Dashboard**: Compliance alerts shown for pending/failed items
- **Notifications**: Compliance team alerted for new checks and escalations
