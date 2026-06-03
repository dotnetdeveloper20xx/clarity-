# Domain Boundaries

This document defines the bounded contexts in Clarity and the rules governing interactions between them.

---

## Principle

Each domain owns its data. Other domains reference entities by ID but never own or directly modify another domain's data.

---

## Bounded Contexts

### 1. Client Domain

**Owns:**
- Client
- Contact (client contacts)

**Responsible For:**
- Creating and managing client records
- Client status lifecycle (Pending → Active → Archived)
- Client search and lookup

**Exposes:**
- Client ID for reference by other domains
- Client status (for compliance gating)

**Consumes From:**
- Compliance Domain (compliance check results affect client status)

---

### 2. Matter Domain

**Owns:**
- Matter
- MatterNote
- MatterTask
- MatterTeamMember

**Responsible For:**
- Creating and managing legal matters
- Matter lifecycle (Open → In Progress → On Hold → Closed)
- Task management within matters
- Note management within matters
- Team assignment

**Exposes:**
- Matter ID for reference by other domains
- Matter status (for gating time recording, billing, etc.)

**Consumes From:**
- Client Domain (client reference, compliance status check)
- Security Domain (user assignments)

---

### 3. Document Domain

**Owns:**
- Document
- DocumentVersion

**Responsible For:**
- File upload and storage
- Versioning
- Metadata management
- Virus scanning integration

**Exposes:**
- Document ID for reference
- Download capability

**Consumes From:**
- Client Domain (client-level documents)
- Matter Domain (matter-level documents)
- Security Domain (access control)

---

### 4. Time Recording Domain

**Owns:**
- TimeEntry

**Responsible For:**
- Capturing time worked
- Time entry lifecycle (Draft → Approved → Billed)
- Validation (date rules, maximum hours)

**Exposes:**
- Approved unbilled time entries (for Billing Domain)
- Time summaries (for Reporting Domain)

**Consumes From:**
- Matter Domain (matter reference, status check)
- Security Domain (user reference, approval permissions)
- Billing Domain (marks entries as Billed when invoiced)

---

### 5. Billing Domain

**Owns:**
- Invoice
- InvoiceLineItem
- BillingRate
- Disbursement

**Responsible For:**
- Invoice generation from approved time/disbursements
- Invoice lifecycle (Draft → Issued → Paid → Written Off)
- Billing rate management
- Tax calculations

**Exposes:**
- Invoice status (for Payment Domain)
- Financial summaries (for Reporting Domain)

**Consumes From:**
- Time Recording Domain (approved time entries)
- Matter Domain (matter reference)
- Client Domain (client reference, billing address)

---

### 6. Payment Domain

**Owns:**
- Payment
- ClientAccountBalance

**Responsible For:**
- Recording payments received
- Allocating payments to invoices
- Managing client account balances
- Payment reversals

**Exposes:**
- Payment status updates (invoice paid/partially paid)

**Consumes From:**
- Billing Domain (invoice reference)
- Client Domain (client reference)

---

### 7. Compliance Domain

**Owns:**
- ComplianceCheck

**Responsible For:**
- AML, KYC, and conflict of interest checks
- Compliance status per client/matter
- Periodic re-check scheduling
- Compliance reporting

**Exposes:**
- Compliance status (gates client activation and matter creation)

**Consumes From:**
- Client Domain (client to check)
- Matter Domain (matter to check)

---

### 8. Security Domain

**Owns:**
- User
- Role
- Permission
- UserRole
- RolePermission

**Responsible For:**
- Authentication
- Authorisation
- Role management
- Permission enforcement
- Session management
- Password policies

**Exposes:**
- User identity and roles (consumed by all domains)
- Permission checks

**Consumes From:**
- None (foundational domain)

---

### 9. Audit Domain

**Owns:**
- AuditEntry

**Responsible For:**
- Recording all significant actions
- Providing audit trail queries
- Retention management

**Exposes:**
- Audit history per entity, user, or date range

**Consumes From:**
- All domains (receives audit events)

---

### 10. Notification Domain

**Owns:**
- Notification
- NotificationPreference

**Responsible For:**
- Sending in-app notifications
- Email notifications
- Notification preferences

**Exposes:**
- Notification delivery

**Consumes From:**
- All domains (receives notification triggers via events)

---

### 11. Reporting Domain

**Owns:**
- ReportDefinition
- SavedReport

**Responsible For:**
- Operational dashboards
- Financial reports
- Productivity reports
- Compliance reports
- Data export

**Exposes:**
- Dashboard data
- Report generation

**Consumes From:**
- All domains (read-only queries across domain data)

---

## Interaction Rules

| Rule | Description |
|------|-------------|
| Reference by ID | Domains reference other domains' entities by ID only, never by direct object reference. |
| No cross-domain writes | A domain never directly modifies another domain's data. |
| Events for coordination | When one domain needs to trigger action in another, it publishes a domain event. |
| Queries for reading | The Reporting Domain and dashboards may query across boundaries (read-only). |
| API composition | The API layer may compose data from multiple domains for a single response. |

---

## Domain Interaction Map

```
┌──────────────┐     references      ┌──────────────────┐
│   Client     │◄────────────────────│     Matter       │
│   Domain     │                     │     Domain       │
└──────┬───────┘                     └────────┬─────────┘
       │                                      │
       │ compliance gates                     │ matter reference
       │                                      │
┌──────▼───────┐                     ┌────────▼─────────┐
│  Compliance  │                     │  Time Recording  │
│   Domain     │                     │     Domain       │
└──────────────┘                     └────────┬─────────┘
                                              │
                                              │ approved time
                                              │
                                     ┌────────▼─────────┐
                                     │    Billing       │
                                     │    Domain        │
                                     └────────┬─────────┘
                                              │
                                              │ invoice reference
                                              │
                                     ┌────────▼─────────┐
                                     │    Payment       │
                                     │    Domain        │
                                     └──────────────────┘

Cross-cutting (consumed by all):
┌──────────────┐  ┌──────────────┐  ┌──────────────────┐
│   Security   │  │    Audit     │  │   Notification   │
│   Domain     │  │    Domain    │  │     Domain       │
└──────────────┘  └──────────────┘  └──────────────────┘
```
