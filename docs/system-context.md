# System Context

This document describes how Clarity fits within the broader ecosystem of users, systems, and services.

---

## System Context Diagram

```
┌───────────────────────────────────────────────────────────────────────────────────┐
│                              External Actors                                       │
│                                                                                   │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────┐  │
│  │   Clients   │  │ Solicitors  │  │  Finance    │  │  Admin / Compliance     │  │
│  │  (Portal)   │  │  & Assts    │  │   Team      │  │  & Support              │  │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └────────────┬────────────┘  │
│         │                 │                 │                      │               │
└─────────┼─────────────────┼─────────────────┼──────────────────────┼───────────────┘
          │                 │                 │                      │
          ▼                 ▼                 ▼                      ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│                                                                                   │
│                            CLARITY PLATFORM                                        │
│                                                                                   │
│  ┌─────────────────────────────────────────────────────────────────────────────┐  │
│  │                         Angular Frontend (SPA)                               │  │
│  │         Dashboard │ Clients │ Matters │ Billing │ Reports │ Admin           │  │
│  └──────────────────────────────────────┬──────────────────────────────────────┘  │
│                                         │ HTTPS / REST API                        │
│  ┌──────────────────────────────────────▼──────────────────────────────────────┐  │
│  │                         ASP.NET Core Web API                                 │  │
│  │    Authentication │ Authorization │ Routing │ Validation │ Swagger           │  │
│  └──────────────────────────────────────┬──────────────────────────────────────┘  │
│                                         │                                         │
│  ┌──────────────────────────────────────▼──────────────────────────────────────┐  │
│  │                      Application Layer (CQRS)                                │  │
│  │         Commands │ Queries │ Handlers │ Validators │ Events                  │  │
│  └──────────────────────────────────────┬──────────────────────────────────────┘  │
│                                         │                                         │
│  ┌──────────────────────────────────────▼──────────────────────────────────────┐  │
│  │                      Infrastructure Layer                                    │  │
│  │      EF Core │ Repositories │ File Storage │ Email │ External APIs           │  │
│  └──────────────────────────────────────┬──────────────────────────────────────┘  │
│                                         │                                         │
└─────────────────────────────────────────┼─────────────────────────────────────────┘
                                          │
          ┌───────────────────────────────┼───────────────────────────────┐
          │                               │                               │
          ▼                               ▼                               ▼
┌──────────────────┐     ┌──────────────────────┐     ┌──────────────────────────┐
│   SQL Server     │     │   Blob Storage       │     │   Email Service          │
│   (Database)     │     │   (Documents)        │     │   (SMTP / SendGrid)      │
└──────────────────┘     └──────────────────────┘     └──────────────────────────┘
```

---

## External Systems

| System | Purpose | Integration |
|--------|---------|-------------|
| SQL Server | Primary data store for all business data | Entity Framework Core |
| Blob Storage | Document and file storage | Azure Blob Storage or local file system |
| Email Service | Sending notifications and correspondence | SMTP or cloud email provider |
| Identity Provider | User authentication (future: Azure AD, OAuth) | ASP.NET Core Identity |
| Payment Gateway | Client payments (future phase) | REST API integration |
| Azure Service Bus | Async message processing (future phase) | MassTransit or Azure SDK |
| Azure Key Vault | Secrets management (production) | Azure SDK |
| Application Insights | Monitoring and telemetry | OpenTelemetry exporter |

---

## Communication Patterns

| From | To | Protocol | Pattern |
|------|----|----------|---------|
| Browser | Angular SPA | HTTPS | Static file serving |
| Angular SPA | API | HTTPS | REST (JSON) |
| API | Database | TCP | Entity Framework Core (connection pool) |
| API | Blob Storage | HTTPS | Azure SDK or file system |
| API | Email Service | SMTP/HTTPS | Async via background job |
| API | Service Bus | AMQP | Publish events (future) |

---

## Deployment Context

### Local Development

```
Developer Machine
├── Clarity.Api (Kestrel, HTTPS, port 5001)
├── Clarity.Web (Angular dev server, port 4200)
├── SQL Server (LocalDB or Docker)
└── Local file system (document storage)
```

### Production (Azure)

```
Azure
├── Azure App Service (API)
├── Azure Static Web Apps or App Service (Angular)
├── Azure SQL Database
├── Azure Blob Storage (documents)
├── Azure Key Vault (secrets)
├── Azure Application Insights (monitoring)
├── Azure Service Bus (async messaging) [future]
└── Azure Redis Cache (distributed cache) [future]
```

---

## Security Boundaries

| Boundary | Protection |
|----------|-----------|
| Internet → Frontend | HTTPS, CSP headers, XSS protection |
| Frontend → API | JWT bearer tokens, CORS policy |
| API → Database | Connection string in Key Vault, encrypted connection |
| API → Blob Storage | Managed identity or SAS tokens |
| Internal services | Network isolation (VNET in Azure) |

---

## Data Flow Summary

### Write Path (Command)
```
User Action → Angular → HTTP POST/PUT/DELETE → API Controller → MediatR Command
→ Validation → Authorization → Handler → Domain Logic → Repository → SQL Server
→ Domain Events → Event Handlers (audit, notifications, etc.)
→ HTTP Response → Angular → UI Update
```

### Read Path (Query)
```
User Navigation → Angular → HTTP GET → API Controller → MediatR Query
→ Authorization → Handler → Optimized Query (EF Core / Dapper) → SQL Server
→ DTO Mapping → HTTP Response → Angular → UI Render
```
