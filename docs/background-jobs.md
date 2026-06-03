# Background Jobs

## Purpose

Some operations should not block the user's request. Background jobs handle async processing like sending notifications, generating reports, and checking overdue items.

## Job Entity

```
BackgroundJob
├── Id (Guid)
├── JobType (string) — e.g., "SendNotification", "CheckOverdueTasks"
├── Payload (JSON) — job-specific data
├── Status — Pending → Processing → Completed / Failed → DeadLetter
├── RetryCount / MaxRetries
├── ErrorMessage — last failure reason
├── CreatedAt / StartedAt / CompletedAt
└── CorrelationId — trace back to originating request
```

## Job Statuses

| Status | Meaning |
|--------|---------|
| Pending | Queued, waiting for worker to pick up |
| Processing | Currently being executed |
| Completed | Successfully finished |
| Failed | Execution failed (will be retried if under max retries) |
| DeadLetter | Failed permanently after all retry attempts |

## Retry Strategy

- Jobs start with `RetryCount = 0`, `MaxRetries = 3`
- On failure: increment RetryCount, set status back to Pending
- When RetryCount >= MaxRetries: move to DeadLetter
- Dead-lettered jobs require manual investigation

## Job Types

| Job Type | Trigger | Description |
|----------|---------|-------------|
| SendNotification | Workflow events | Deliver in-app notification |
| CheckOverdueTasks | Scheduled (hourly) | Flag overdue tasks |
| CheckOverdueInvoices | Scheduled (daily) | Send reminders for overdue invoices |
| ArchiveClosedMatters | Scheduled (nightly) | Archive matters closed > X days |
| GenerateReport | User request | Async report generation |

## Worker Service (Future)

A separate `Clarity.Worker` console app will:
1. Poll the BackgroundJobs table for Pending jobs
2. Mark job as Processing
3. Execute the handler based on JobType
4. Mark as Completed or Failed
5. Retry or dead-letter as appropriate

For development, jobs can be processed inline or via a simple hosted service in the API.

## Monitoring

Admin/support users can view:
- Pending job count
- Failed jobs
- Dead-letter jobs (require attention)
- Job execution history
