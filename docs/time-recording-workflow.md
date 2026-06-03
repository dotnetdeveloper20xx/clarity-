# Time Recording Workflow

## Status Flow

```
Draft → Submitted → Approved → Billed (terminal)
                  ↘ Rejected → Draft (re-editable)
         Approved → WrittenOff (terminal)
```

## Statuses

| Status | Meaning | Who Can Edit |
|--------|---------|-------------|
| Draft | Initial entry, not yet submitted | Owner |
| Submitted | Sent for approval | Nobody (locked) |
| Approved | Team leader has approved | Nobody |
| Rejected | Returned with reason | Owner (becomes Draft again) |
| Billed | Included on an invoice | Nobody (permanent) |
| WrittenOff | Discarded (with reason) | Nobody (permanent) |

## Business Rules

1. **Only Draft entries can be edited** by the owner
2. **Submitted entries are locked** — they await approval
3. **Rejection requires a reason** — the owner is notified
4. **Approved entries** become available for billing
5. **Billed entries are permanent** — tied to an issued invoice
6. **Time cannot be recorded against closed/archived matters**
7. **Future dates are not allowed**

## Permissions

| Action | Required Role |
|--------|--------------|
| Record time | Consultant, LegalAssistant, TeamLeader |
| Submit time | Owner of the entry |
| Approve time | TeamLeader, Admin |
| Reject time | TeamLeader, Admin |
| Bill time | Finance (via invoice creation) |
| Write off time | Finance, Admin |

## Notifications

- **Rejection**: Owner receives notification with reason
- **Approval**: Owner may optionally be notified
- **Bulk overdue**: TeamLeaders notified of un-submitted draft entries older than 7 days

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/timeentries | Create draft time entry |
| POST | /api/workflow/time-entries/{id}/submit | Submit for approval |
| PUT | /api/timeentries/{id}/approve | Approve entry |
| POST | /api/workflow/time-entries/{id}/reject | Reject with reason |
