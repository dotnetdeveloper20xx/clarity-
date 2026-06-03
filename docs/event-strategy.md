# Event Strategy

## Overview

Domain events are used in Clarity to decouple domains, trigger side effects, and power cross-cutting concerns like notifications, audit logging, and reporting updates.

When a significant business action occurs, the domain raises an event. Other parts of the system react to that event without the originating domain needing to know about them.

## Benefits

- **Loose coupling** — Domains don't reference each other directly
- **Extensibility** — New reactions can be added without modifying existing code
- **Audit trail** — Events naturally provide an audit history
- **Notifications** — Events trigger user notifications
- **Background processing** — Events can be processed asynchronously
- **Reporting** — Events update dashboards and reports

## Implementation Approach

Events are dispatched using MediatR's notification mechanism:

1. Domain entity raises an event (adds to a collection)
2. After the command handler completes (and transaction commits), events are dispatched
3. Event handlers execute (same process, or queued for async processing)

For Phase 1, events are processed in-process (synchronous). In later phases, events may be published to Azure Service Bus for true async processing.

---

## Domain Events

### Client Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| ClientCreatedEvent | New client is created | Trigger compliance check, notify compliance team, audit log |
| ClientActivatedEvent | Client status set to Active | Notify assigned team, audit log |
| ClientArchivedEvent | Client is archived | Notify admin, audit log |
| ClientPlacedOnHoldEvent | Client placed on hold | Notify assigned consultants, block matter creation, audit log |

### Matter Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| MatterCreatedEvent | New matter is opened | Notify team leader, create default tasks, audit log |
| MatterStatusChangedEvent | Matter status transitions | Notify team, audit log, update dashboard |
| MatterClosedEvent | Matter is closed | Notify client, archive documents, audit log, update reports |
| MatterReopenedEvent | Closed matter is reopened | Notify compliance, audit log |
| TeamMemberAssignedEvent | Team member added to matter | Notify the assigned user, audit log |
| TeamMemberRemovedEvent | Team member removed | Notify the removed user, audit log |

### Time Recording Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| TimeEntryRecordedEvent | Time entry created | Update WIP totals, audit log |
| TimeEntryApprovedEvent | Time entry approved | Mark available for billing, notify finance, audit log |
| TimeEntryRevertedEvent | Approved entry reverted to draft | Notify fee earner, audit log |

### Billing Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| InvoiceGeneratedEvent | Draft invoice created | Audit log |
| InvoiceIssuedEvent | Invoice sent to client | Notify client, start payment tracking, audit log |
| InvoicePaidEvent | Invoice fully paid | Update matter financials, notify consultant, audit log |
| InvoiceWrittenOffEvent | Invoice written off | Update financials, notify finance manager, audit log |
| CreditNoteIssuedEvent | Credit note created | Update invoice balance, audit log |

### Payment Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| PaymentReceivedEvent | Payment recorded | Update invoice status, update client balance, audit log |
| PaymentReversedEvent | Payment reversed | Revert invoice status, update client balance, audit log |

### Document Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| DocumentUploadedEvent | Document uploaded | Notify matter team, trigger virus scan, audit log |
| DocumentArchivedEvent | Document soft-deleted | Audit log |
| DocumentVersionCreatedEvent | New version uploaded | Notify matter team, audit log |

### Compliance Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| ComplianceCheckCompletedEvent | Check recorded | Update client status (if pass/fail), notify relevant parties, audit log |
| ComplianceCheckFailedEvent | Check result is Fail | Block client/matter, notify management, audit log |

### Security Events

| Event | Raised When | Typical Reactions |
|-------|-------------|-------------------|
| UserCreatedEvent | New user account created | Send welcome email, audit log |
| UserDisabledEvent | User account disabled | Terminate sessions, audit log |
| RoleAssignedEvent | Role assigned to user | Audit log |
| LoginFailedEvent | Failed login attempt | Increment lockout counter, audit log |
| UserLockedOutEvent | Account locked | Notify admin, audit log |

---

## Event Handling Patterns

### Synchronous (In-Process)

Used for reactions that must complete as part of the same request:

- Audit logging (must be consistent with the action)
- Status updates on related entities

### Asynchronous (Background)

Used for reactions that can be eventually consistent:

- Email notifications
- Report regeneration
- Document scanning
- Dashboard updates

### Event Handler Example (Conceptual)

```
Event: MatterCreatedEvent
  → Handler 1: AuditLogHandler (records audit entry)
  → Handler 2: NotificationHandler (notifies team leader)
  → Handler 3: DefaultTasksHandler (creates template tasks)
```

---

## Event Envelope

All events carry standard metadata:

| Field | Description |
|-------|-------------|
| EventId | Unique event identifier |
| Timestamp | When the event occurred (UTC) |
| UserId | Who triggered the action |
| CorrelationId | Request correlation ID |
| EntityType | Type of entity involved |
| EntityId | ID of entity involved |

---

## Future Considerations

- **Event Store**: For full event sourcing if required for specific domains (e.g., financial audit trail)
- **Azure Service Bus**: For distributed async processing across services
- **Event Replay**: Ability to replay events for rebuilding state or fixing data
- **Saga Pattern**: For complex multi-step workflows spanning multiple domains
