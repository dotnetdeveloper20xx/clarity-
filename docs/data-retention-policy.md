# Data Retention Policy

## Purpose

Legal practice data must be retained for specific periods based on regulatory requirements, business needs, and contractual obligations. This document defines how long data is kept and what happens when retention periods expire.

---

## Retention Periods

### Active Data (Primary Database)

| Data Type | Retention Period | Rationale |
|-----------|-----------------|-----------|
| Active clients | Indefinite (while active) | Ongoing business relationship |
| Active matters | Indefinite (while open) | Active work |
| Open invoices | Until paid + 7 years | Financial record keeping |
| Active users | Indefinite (while employed) | System access |
| Active compliance checks | Indefinite | Regulatory requirement |

### Closed/Historical Data

| Data Type | Retention After Closure | Rationale |
|-----------|------------------------|-----------|
| Closed matters | 7 years minimum | Solicitors Regulation Authority (SRA) requirements |
| Archived clients | 7 years after last matter closed | Regulatory and insurance |
| Paid invoices | 7 years | HMRC tax record requirements |
| Completed payments | 7 years | Financial regulations |
| Time entries (billed) | 7 years (with invoice) | Financial records |
| Documents (matter) | 7 years after matter closure | Legal obligations |
| Compliance checks | Permanent | Regulatory requirement |
| Audit logs | 7 years minimum | Compliance and investigation |

### Ephemeral Data

| Data Type | Retention Period | Rationale |
|-----------|-----------------|-----------|
| Read notifications | 90 days | Low value after reading |
| Session tokens | 24 hours after expiry | Security housekeeping |
| Temporary upload files | 24 hours | Cleanup incomplete uploads |
| Failed login attempts (detail) | 90 days | Security investigation period |

---

## Data Lifecycle

```
Created → Active → Closed/Archived → Retention Period → Purge Decision
                                                              │
                                                              ├── Move to Archive DB
                                                              ├── Anonymise
                                                              └── Hard Delete (with approval)
```

---

## Archival Strategy

When data exceeds its active retention period but must still be preserved:

### Option 1: Archive Flag (Default)

- Record remains in the primary database
- Marked with an Archive status
- Excluded from normal queries (similar to soft-delete)
- Available for compliance and audit queries

### Option 2: Archive Database (Future)

- Data moved to a separate archive database
- Cheaper storage (lower performance tier)
- Accessible via admin tools for investigation
- Not available for real-time queries

### Option 3: Cold Storage (Future)

- Data exported and stored in blob storage (Azure Archive tier)
- Extremely low cost
- Retrieval takes hours (acceptable for old records)
- Used for data older than 10 years

---

## Purge Process

When the retention period fully expires and there is no legal obligation to keep data:

1. **Automated identification**: Background job identifies records past retention
2. **Report generation**: List of candidates for purge produced for review
3. **Human approval**: A compliance officer or administrator must approve the purge
4. **Pre-purge backup**: A final backup of the data is taken
5. **Execution**: Records are permanently removed (hard delete)
6. **Audit**: The purge action itself is recorded in the audit log (which is never purged)

**Critical Rule**: Purging is never fully automated. A human must approve.

---

## GDPR Considerations

Under GDPR (and UK data protection law), data subjects have rights:

| Right | How Clarity Handles It |
|-------|----------------------|
| Right of Access (SAR) | Export all data related to a client/individual |
| Right to Erasure | Anonymise data where legal retention doesn't apply |
| Right to Rectification | Update incorrect data (audited) |
| Right to Restrict Processing | Flag record as restricted |

**Important**: The right to erasure does NOT override legal retention requirements. If the SRA requires 7-year retention, GDPR erasure requests for that data are declined with explanation.

### Anonymisation (instead of deletion)

When erasure is requested but retention applies:

- Personal identifiers are replaced with anonymised placeholders
- "John Smith" becomes "Client-00421"
- Email, phone, address are cleared
- The structural data (matter, time, invoices) remains intact for financial/legal purposes
- The individual is no longer identifiable

---

## Responsibilities

| Role | Responsibility |
|------|---------------|
| System Administrator | Configure retention periods |
| Compliance Officer | Approve purge requests, handle SARs |
| Background Job | Identify candidates for archival/purge |
| DBA | Execute archive migrations, manage backup |
| Legal Counsel | Advise on retention requirements |

---

## Configuration

Retention periods are configurable in system settings:

| Setting | Default | Unit |
|---------|---------|------|
| MatterRetentionYears | 7 | Years after closure |
| InvoiceRetentionYears | 7 | Years after payment |
| AuditLogRetentionYears | 7 | Years |
| NotificationRetentionDays | 90 | Days after read |
| SessionRetentionHours | 24 | Hours after expiry |

These values can be adjusted by administrators based on regulatory changes.
