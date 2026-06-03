# Reporting Data Strategy

## Challenge

Reporting queries in a legal platform can be expensive:

- "Show me all unbilled time across all matters for this month"
- "What is the aged debt broken down by client?"
- "What is each consultant's utilisation rate?"
- "How many matters were opened vs closed this quarter?"

These queries aggregate data across millions of rows. Running them against the transactional database can degrade performance for day-to-day operations.

---

## Strategy: Phased Approach

### Phase 1 (Initial) — Optimised Queries

For the initial implementation:

- Use optimised SQL queries with proper indexes
- Create SQL views for common report patterns
- Accept that some complex reports may take a few seconds
- This is acceptable for a first release with moderate data volumes

### Phase 2 (Scale) — Read Models

When data volumes grow:

- Create materialised views or summary tables
- Update summaries via background jobs (event-driven)
- Reports query the pre-computed data instead of raw tables

### Phase 3 (Enterprise) — Dedicated Reporting

For large-scale deployments:

- Read replica database for reporting queries
- Reporting queries never hit the primary database
- Real-time sync via SQL Server replication or change data capture

---

## Key Reports and Their Data Sources

### Operational Dashboard

| Metric | Source Tables | Update Frequency |
|--------|-------------|-----------------|
| Open matters count | matter.Matters (Status != Closed) | Real-time |
| Overdue tasks count | matter.MatterTasks (DueDate < Today, Status != Complete) | Real-time |
| Pending compliance checks | compliance.ComplianceChecks (Status = Pending) | Real-time |
| Today's time entries | time.TimeEntries (Date = Today) | Real-time |
| Unread notifications | notification.Notifications (IsRead = 0) | Real-time |

### Financial Dashboard

| Metric | Source Tables | Update Frequency |
|--------|-------------|-----------------|
| Total WIP (unbilled time) | time.TimeEntries (Status = Approved, InvoiceId = NULL) | Real-time |
| Outstanding invoices total | billing.Invoices (Status = Issued or PartiallyPaid) | Real-time |
| Aged debt breakdown | billing.Invoices (grouped by overdue bands) | Hourly/Daily |
| Revenue this month | payment.Payments (PaymentDate in current month) | Real-time |
| Cash collected this month | payment.Payments (sum of Amount) | Real-time |

### Consultant Productivity

| Metric | Source Tables | Update Frequency |
|--------|-------------|-----------------|
| Hours recorded today/week/month | time.TimeEntries (grouped by user, date range) | Real-time |
| Billable vs non-billable ratio | time.TimeEntries (grouped by IsBillable) | Daily |
| Utilisation rate | TimeEntries hours / available hours | Daily |
| Matters handled | matter.MatterTeamMembers + Matters | Real-time |

### Compliance Dashboard

| Metric | Source Tables | Update Frequency |
|--------|-------------|-----------------|
| Pending checks | compliance.ComplianceChecks (Status = Pending) | Real-time |
| Failed checks this month | compliance.ComplianceChecks (Status = Fail) | Real-time |
| Overdue re-checks | compliance.ComplianceChecks (NextReviewDate < Today) | Daily |
| Clients on hold | client.Clients (Status = OnHold) | Real-time |

---

## SQL Views

The following views will be created to simplify common report queries:

### vw_MatterSummary

```sql
-- Combines matter data with calculated fields
SELECT 
    m.Id, m.ReferenceNumber, m.Title, m.Status,
    c.Name AS ClientName,
    u.FirstName + ' ' + u.LastName AS LeadConsultant,
    (SELECT COUNT(*) FROM time.TimeEntries t WHERE t.MatterId = m.Id AND t.IsDeleted = 0) AS TimeEntryCount,
    (SELECT SUM(t.DurationMinutes) FROM time.TimeEntries t WHERE t.MatterId = m.Id AND t.IsBillable = 1 AND t.IsDeleted = 0) AS TotalBillableMinutes,
    (SELECT SUM(i.TotalAmount) FROM billing.Invoices i WHERE i.MatterId = m.Id AND i.IsDeleted = 0) AS TotalInvoiced,
    (SELECT SUM(p.Amount) FROM payment.Payments p 
     INNER JOIN billing.Invoices i ON p.InvoiceId = i.Id 
     WHERE i.MatterId = m.Id AND p.IsReversed = 0) AS TotalPaid
FROM matter.Matters m
INNER JOIN client.Clients c ON m.ClientId = c.Id
INNER JOIN security.Users u ON m.LeadConsultantId = u.Id
WHERE m.IsDeleted = 0
```

### vw_AgedDebt

```sql
-- Invoices grouped by age bands
SELECT
    i.ClientId, c.Name AS ClientName,
    i.InvoiceNumber, i.TotalAmount, i.PaidAmount,
    (i.TotalAmount - i.PaidAmount) AS Outstanding,
    DATEDIFF(DAY, i.DueDate, GETUTCDATE()) AS DaysOverdue,
    CASE 
        WHEN DATEDIFF(DAY, i.DueDate, GETUTCDATE()) <= 0 THEN 'Current'
        WHEN DATEDIFF(DAY, i.DueDate, GETUTCDATE()) <= 30 THEN '1-30 Days'
        WHEN DATEDIFF(DAY, i.DueDate, GETUTCDATE()) <= 60 THEN '31-60 Days'
        WHEN DATEDIFF(DAY, i.DueDate, GETUTCDATE()) <= 90 THEN '61-90 Days'
        ELSE '90+ Days'
    END AS AgeBand
FROM billing.Invoices i
INNER JOIN client.Clients c ON i.ClientId = c.Id
WHERE i.Status IN (1, 2) -- Issued or PartiallyPaid
AND i.IsDeleted = 0
```

### vw_ConsultantTimesheet

```sql
-- Daily time entry summary per consultant
SELECT 
    t.UserId, u.FirstName + ' ' + u.LastName AS ConsultantName,
    t.Date, 
    SUM(t.DurationMinutes) AS TotalMinutes,
    SUM(CASE WHEN t.IsBillable = 1 THEN t.DurationMinutes ELSE 0 END) AS BillableMinutes,
    SUM(CASE WHEN t.IsBillable = 0 THEN t.DurationMinutes ELSE 0 END) AS NonBillableMinutes,
    COUNT(*) AS EntryCount
FROM time.TimeEntries t
INNER JOIN security.Users u ON t.UserId = u.Id
WHERE t.IsDeleted = 0
GROUP BY t.UserId, u.FirstName, u.LastName, t.Date
```

---

## Performance Guidelines

| Guideline | Detail |
|-----------|--------|
| Use indexes | All WHERE and JOIN columns must be indexed |
| Avoid SELECT * | Only retrieve needed columns |
| Use pagination | Never return unbounded result sets |
| Set query timeout | Reports have a 30-second timeout; background reports have 5 minutes |
| Cache where safe | Cache dashboard data for 60 seconds (configurable) |
| Background for heavy reports | Complex reports generate async and notify user when ready |

---

## Export Formats

Reports should be exportable to:

| Format | Use Case |
|--------|----------|
| PDF | Formal reports for management |
| Excel (.xlsx) | Data analysis by finance team |
| CSV | Data import into other systems |

Export is generated server-side to handle large datasets without browser memory issues.
