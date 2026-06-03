# Diagnostics Guide

## Purpose

The diagnostics system provides Admin and Support users with visibility into platform health, failed operations, and production issues.

## Accessing Diagnostics

API endpoints under `/api/diagnostics` are restricted to **Admin** and **Support** roles.

## Available Endpoints

### System Information

```
GET /api/diagnostics/system-info
```

Returns: environment name, machine name, .NET runtime version, server time, uptime.

### Background Job Monitoring

```
GET /api/diagnostics/jobs?status=Failed&take=20
```

View jobs by status (Pending, Processing, Completed, Failed, DeadLetter).

```
GET /api/diagnostics/jobs/summary
```

Quick count of jobs in each status — useful for dashboard widgets.

```
POST /api/diagnostics/jobs/{id}/retry
```

Reset a failed/dead-letter job back to Pending for re-processing.

### Recent Errors

```
GET /api/diagnostics/recent-errors?take=20
```

Shows recent error/exception audit entries with correlation IDs for tracing.

## Troubleshooting Workflow

### 1. User Reports an Error

1. Ask for the **Correlation ID** (shown in the error message)
2. Search audit log: `GET /api/audit?correlationId={id}`
3. Check Serilog file logs for the same correlation ID
4. Identify the failed operation and root cause

### 2. Dashboard Shows Issues

1. Check `/api/diagnostics/jobs/summary` for failed/dead-letter jobs
2. Check `/api/diagnostics/recent-errors` for patterns
3. Check `/health` endpoint for database connectivity
4. Check Serilog logs for "Long running request" warnings

### 3. Background Job Failures

1. List failed jobs: `/api/diagnostics/jobs?status=Failed`
2. Review error message in the response
3. Fix the underlying issue (data, network, config)
4. Retry the job: `POST /api/diagnostics/jobs/{id}/retry`

## Health Check

```
GET /health
```

Returns "Healthy" or "Unhealthy" with checks for:
- API alive
- SQL Server connection
- (Future: file storage, Redis, external services)

## Production Monitoring Checklist

| Check | Frequency | Action if Unhealthy |
|-------|-----------|---------------------|
| /health endpoint | Every 60s | Alert on-call |
| Failed jobs count | Every 5 min | Investigate if > 0 |
| Dead-letter count | Hourly | Manual investigation |
| Error rate in logs | Continuous | Alert if spike |
| Slow request warnings | Daily review | Performance investigation |
