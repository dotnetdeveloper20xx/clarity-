# Billing Workflow

## Status Flow

```
Draft → Issued → PartiallyPaid → Paid (terminal)
  │              ↘ Cancelled (terminal)
  │              ↘ WrittenOff (terminal)
  ↘ Cancelled (terminal)
```

## Business Rules

1. **Only approved, billable time entries** can be included on invoices
2. **Draft invoices can be edited** — add/remove line items, adjust notes
3. **Issued invoices are immutable** — they become a legal document
4. **Issuing sets the due date** (default: 30 days from issue date)
5. **Payments automatically update status** — Partially Paid or Paid
6. **Paid invoices cannot be cancelled** — must issue credit note instead
7. **Write-offs require reason** and are audited
8. **Only Finance or Admin** can create, issue, or cancel invoices

## Invoice Creation Process

1. Finance user selects a matter
2. System finds all approved, unbilled time entries
3. Each time entry becomes a line item (hours × rate)
4. SubTotal, TaxAmount, and TotalAmount are calculated
5. Invoice is created in Draft status
6. Finance reviews and issues

## Payment Recording

1. Payment is linked to an invoice
2. Invoice `PaidAmount` is updated
3. If `PaidAmount >= TotalAmount` → status = Paid
4. If `PaidAmount < TotalAmount` → status = PartiallyPaid
5. Overpayments create credit on client account (future)

## Notifications

- Client notified when invoice is issued
- Finance notified when payment is received
- Finance alerted for overdue invoices (scheduled job)
