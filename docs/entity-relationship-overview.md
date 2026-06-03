# Entity Relationship Overview

## Diagram (Text Representation)

```
┌──────────────────┐          ┌──────────────────────────┐
│     Clients      │          │       Matters            │
│──────────────────│          │──────────────────────────│
│ Id (PK)          │◄────┐    │ Id (PK)                  │
│ ReferenceNumber  │     │    │ ReferenceNumber          │
│ Name             │     ├────│ ClientId (FK)            │
│ ClientType       │     │    │ LeadConsultantId (FK)────┼──────────────┐
│ Status           │     │    │ MatterTypeId (FK)        │              │
│ ...              │     │    │ Status                   │              │
└────────┬─────────┘     │    │ ...                      │              │
         │               │    └────────┬──────┬──────────┘              │
         │               │             │      │                         │
         │               │             │      │                         ▼
┌────────▼─────────┐     │    ┌────────▼───┐  │              ┌─────────────────┐
│ ClientContacts   │     │    │MatterNotes │  │              │     Users       │
│──────────────────│     │    │────────────│  │              │─────────────────│
│ Id (PK)          │     │    │ Id (PK)    │  │              │ Id (PK)         │
│ ClientId (FK)    │     │    │ MatterId   │  │              │ Email           │
│ Name             │     │    │ Content    │  │              │ FirstName       │
│ Email            │     │    │ IsVisible  │  │              │ LastName        │
│ Phone            │     │    └────────────┘  │              │ IsActive        │
└──────────────────┘     │                    │              └────────┬────────┘
                         │    ┌───────────────▼──────┐               │
                         │    │   MatterTasks        │               │
                         │    │──────────────────────│      ┌────────▼────────┐
                         │    │ Id (PK)              │      │   UserRoles     │
                         │    │ MatterId (FK)        │      │─────────────────│
                         │    │ AssigneeId (FK)──────┼──┐   │ UserId (FK)     │
                         │    │ Status               │  │   │ RoleId (FK)     │
                         │    │ DueDate              │  │   └─────────────────┘
                         │    └──────────────────────┘  │
                         │                              │
                         │    ┌──────────────────────┐  │
                         │    │   TimeEntries        │  │
                         │    │──────────────────────│  │
                         │    │ Id (PK)              │  │
                         │    │ MatterId (FK)────────┼──┼───► Matters
                         │    │ UserId (FK)──────────┼──┘
                         │    │ Date                 │
                         │    │ DurationMinutes      │
                         │    │ IsBillable           │
                         │    │ Status               │
                         │    │ InvoiceId (FK)───────┼───────┐
                         │    └──────────────────────┘       │
                         │                                   │
                         │    ┌──────────────────────┐       │
                         │    │     Invoices         │◄──────┘
                         │    │──────────────────────│
                         │    │ Id (PK)              │
                         ├────│ ClientId (FK)        │
                         │    │ MatterId (FK)────────┼──────► Matters
                         │    │ InvoiceNumber        │
                         │    │ Status               │
                         │    │ TotalAmount          │
                         │    └──────────┬───────────┘
                         │               │
                         │    ┌──────────▼───────────┐
                         │    │  InvoiceLineItems    │
                         │    │──────────────────────│
                         │    │ Id (PK)              │
                         │    │ InvoiceId (FK)       │
                         │    │ Description          │
                         │    │ Amount               │
                         │    └──────────────────────┘
                         │
                         │    ┌──────────────────────┐
                         │    │     Payments         │
                         │    │──────────────────────│
                         │    │ Id (PK)              │
                         │    │ InvoiceId (FK)───────┼──────► Invoices
                         │    │ Amount               │
                         │    │ PaymentDate          │
                         │    │ PaymentMethod        │
                         │    └──────────────────────┘
                         │
                         │    ┌──────────────────────┐
                         │    │    Documents         │
                         │    │──────────────────────│
                         │    │ Id (PK)              │
                         ├────│ ClientId (FK)        │ (nullable)
                              │ MatterId (FK)────────┼──────► Matters (nullable)
                              │ FileName             │
                              │ StoragePath          │
                              │ Version              │
                              └──────────────────────┘

                         ┌──────────────────────┐
                         │  ComplianceChecks    │
                         │──────────────────────│
                         │ Id (PK)              │
                         │ ClientId (FK)────────┼──────► Clients
                         │ MatterId (FK)────────┼──────► Matters (nullable)
                         │ CheckType            │
                         │ Status               │
                         │ PerformedById (FK)───┼──────► Users
                         └──────────────────────┘

                         ┌──────────────────────┐
                         │    AuditEntries      │
                         │──────────────────────│
                         │ Id (PK)              │
                         │ UserId (FK)          │
                         │ Action               │
                         │ EntityType           │
                         │ EntityId             │
                         │ OldValues (JSON)     │
                         │ NewValues (JSON)     │
                         │ Timestamp            │
                         └──────────────────────┘
```

---

## Relationship Summary

| Parent Entity | Child Entity | Relationship | Cardinality |
|---------------|-------------|--------------|-------------|
| Client | Matter | One-to-Many | A client has many matters |
| Client | ClientContact | One-to-Many | A client has many contacts |
| Client | Document | One-to-Many | A client has many general documents |
| Client | ComplianceCheck | One-to-Many | A client has many compliance checks |
| Client | Invoice | One-to-Many | A client has many invoices |
| Matter | MatterNote | One-to-Many | A matter has many notes |
| Matter | MatterTask | One-to-Many | A matter has many tasks |
| Matter | MatterTeamMember | One-to-Many | A matter has many team members |
| Matter | TimeEntry | One-to-Many | A matter has many time entries |
| Matter | Document | One-to-Many | A matter has many documents |
| Matter | Invoice | One-to-Many | A matter has many invoices |
| Matter | ComplianceCheck | One-to-Many | A matter has many compliance checks |
| Invoice | InvoiceLineItem | One-to-Many | An invoice has many line items |
| Invoice | Payment | One-to-Many | An invoice has many payments |
| Invoice | TimeEntry | One-to-Many | An invoice has many billed time entries |
| User | TimeEntry | One-to-Many | A user records many time entries |
| User | MatterTask | One-to-Many | A user is assigned many tasks |
| User | UserRole | One-to-Many | A user has many role assignments |
| Role | UserRole | One-to-Many | A role is assigned to many users |
| Role | RolePermission | One-to-Many | A role has many permissions |
| User | AuditEntry | One-to-Many | A user generates many audit entries |
| BillingRate | TimeEntry | One-to-Many | A rate applies to many time entries |

---

## Key Constraints

| Constraint | Description |
|-----------|-------------|
| Client.ReferenceNumber | UNIQUE, NOT NULL |
| Matter.ReferenceNumber | UNIQUE, NOT NULL |
| Invoice.InvoiceNumber | UNIQUE, NOT NULL |
| User.Email | UNIQUE, NOT NULL |
| Role.Name | UNIQUE, NOT NULL |
| Permission.Name | UNIQUE, NOT NULL |
| MatterTeamMember (MatterId + UserId) | Composite UNIQUE (no duplicate assignments) |
| UserRole (UserId + RoleId) | Composite UNIQUE (no duplicate role assignments) |
| RolePermission (RoleId + PermissionId) | Composite UNIQUE |
