# Reporting & Dashboards Guide

## Dashboard API

### GET /api/dashboard

Returns real-time KPIs for the authenticated user:

```json
{
  "openMattersCount": 15,
  "overdueTasksCount": 3,
  "pendingComplianceCount": 2,
  "unreadNotificationsCount": 5,
  "unbilledTimeValue": 12450.00,
  "outstandingInvoicesTotal": 28750.00,
  "paidThisMonthTotal": 15000.00,
  "complianceAlertsCount": 2,
  "draftTimeEntriesCount": 8,
  "pendingApprovalsCount": 4
}
```

### GET /api/dashboard/financial

Returns financial summary (Finance and Admin roles):

```json
{
  "totalBilledThisMonth": 25000.00,
  "totalPaidThisMonth": 15000.00,
  "totalOutstanding": 28750.00,
  "totalWip": 12450.00,
  "agedDebt": [
    { "band": "Current", "amount": 10000, "invoiceCount": 3 },
    { "band": "1-30 Days", "amount": 8000, "invoiceCount": 2 },
    { "band": "31-60 Days", "amount": 5000, "invoiceCount": 1 },
    { "band": "90+ Days", "amount": 5750, "invoiceCount": 2 }
  ],
  "topClients": [
    { "clientId": "...", "clientName": "Client 3 Ltd", "revenue": 12000 }
  ]
}
```

## Global Search

### GET /api/search?q={term}

Searches across clients, matters, and invoices. Returns grouped results (max 5 per group):

```json
{
  "clients": [{ "id": "...", "title": "Client Name", "subtitle": "CLI-00001", "entityType": "Client" }],
  "matters": [{ "id": "...", "title": "Matter Title", "subtitle": "MAT-00001", "entityType": "Matter" }],
  "invoices": [{ "id": "...", "title": "INV-00001", "subtitle": "£5,000", "entityType": "Invoice" }]
}
```

## Exports

| Endpoint | Format | Content |
|----------|--------|---------|
| GET /api/export/clients | CSV | All active clients |
| GET /api/export/invoices | CSV | All invoices with status and amounts |
| GET /api/export/time-entries?fromDate=&toDate= | CSV | Time entries with optional date filter |

## Audit Log Search

### GET /api/audit

Filters: entityType, entityId, userId, action, correlationId, fromDate, toDate, pageNumber, pageSize.

Restricted to Admin, Compliance, and Support roles.

## Diagnostics

### GET /api/diagnostics/jobs — Background job list
### GET /api/diagnostics/jobs/summary — Job counts by status
### POST /api/diagnostics/jobs/{id}/retry — Retry a failed/dead-letter job
### GET /api/diagnostics/recent-errors — Recent error audit entries
### GET /api/diagnostics/system-info — Environment, runtime, machine info

Restricted to Admin and Support roles.

## Performance Approach

- All reporting queries use `AsNoTracking()` for better performance
- Dashboard uses direct aggregate queries (COUNT, SUM) instead of loading entities
- Financial summary uses efficient LINQ with server-side computation
- Pagination is enforced on all list queries
- Exports generate CSV server-side (no browser memory issues)
