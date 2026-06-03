# Notifications

## Overview

Clarity's notification system delivers in-app notifications to users when important events occur. Notifications provide awareness without requiring users to poll or check multiple screens.

## Notification Types

| Type | Usage | Example |
|------|-------|---------|
| Info | General information | "Matter MAT-00015 has been closed" |
| Warning | Attention needed | "Invoice INV-00003 is overdue" |
| Action | User action required | "Time entry awaiting your approval" |
| Reminder | Scheduled reminder | "Compliance review due for Client 7" |

## Triggers

Notifications are created by workflow handlers:

| Event | Recipients | Type |
|-------|-----------|------|
| Matter status changed | Lead consultant | Info |
| Time entry rejected | Time entry owner | Warning |
| Invoice overdue | Finance team | Warning |
| Compliance check failed | Compliance team | Warning |
| Task assigned | Assignee | Action |
| Document uploaded | Matter team | Info |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/notifications | Get current user's notifications |
| GET | /api/notifications?unreadOnly=true | Get unread only |
| GET | /api/notifications/unread-count | Get unread count (for badge) |
| PUT | /api/notifications/{id}/read | Mark single as read |
| PUT | /api/notifications/mark-all-read | Mark all as read |

## Frontend Integration

The Angular app shows:
- Notification bell icon with unread count badge
- Dropdown/drawer with recent notifications
- Click notification to navigate to related entity
- "Mark all as read" action

## Notification Service Interface

```csharp
interface INotificationService
{
    Task SendAsync(Guid userId, string title, string message, ...);
    Task SendToRoleAsync(string role, string title, string message, ...);
}
```

`SendToRoleAsync` sends to all users with the specified role — useful for team-wide alerts.

## Future Enhancements

- Email notifications (via background job)
- Push notifications (web push API)
- SignalR real-time delivery
- User notification preferences (opt-out per type)
- Scheduled digest emails
