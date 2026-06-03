# Non-Functional Requirements

This document defines the enterprise expectations for the Clarity platform beyond functional features.

---

## 1. Security

| Requirement | Detail |
|-------------|--------|
| Authentication | All users must authenticate before accessing the platform. Support for username/password with optional multi-factor authentication (MFA). |
| Authorisation | Role-Based Access Control (RBAC) enforced on every request. Users can only perform actions allowed by their assigned roles. |
| Encryption in Transit | All communication uses HTTPS/TLS 1.2+. No unencrypted traffic. |
| Encryption at Rest | Sensitive data (passwords, payment references, personal information) is encrypted in the database. |
| Secret Management | API keys, connection strings, and secrets are stored in a secure vault (Azure Key Vault or equivalent). Never in source code. |
| Session Management | Sessions expire after configurable inactivity (default: 30 minutes). Concurrent session limits may apply. |
| Input Validation | All inputs are validated server-side. No trust of client-side validation alone. |
| OWASP Compliance | The platform must address the OWASP Top 10 vulnerabilities. |

---

## 2. Auditability

| Requirement | Detail |
|-------------|--------|
| Audit Trail | Every significant action produces an immutable audit log entry. |
| Content | Audit entries include: timestamp, user, action, entity, before state, after state. |
| Retention | Audit logs retained for a minimum of 7 years (configurable). |
| Searchability | Audit logs are searchable by user, date range, entity, and action type. |
| Integrity | Audit logs cannot be modified or deleted by any user. |

---

## 3. Performance

| Requirement | Detail |
|-------------|--------|
| API Response Time | 95th percentile response time under 500ms for standard operations. |
| Page Load Time | Initial page load under 3 seconds. Subsequent navigation under 1 second. |
| Database Queries | No single query should exceed 2 seconds under normal load. |
| Concurrent Users | Support 500+ concurrent users without degradation. |
| Search | Full-text search results returned within 2 seconds. |
| Reporting | Standard reports generated within 10 seconds. Complex reports may be queued for background processing. |

---

## 4. Reliability

| Requirement | Detail |
|-------------|--------|
| Availability | Target 99.5% uptime during business hours (Monday–Friday, 08:00–20:00). |
| Data Durability | No data loss under any normal operational scenario. |
| Graceful Degradation | If a non-critical service fails (e.g., notifications), core features remain operational. |
| Recovery | Recovery Point Objective (RPO): 1 hour. Recovery Time Objective (RTO): 4 hours. |
| Backup | Automated daily backups with point-in-time restore capability. |

---

## 5. Scalability

| Requirement | Detail |
|-------------|--------|
| Horizontal Scaling | API layer can scale horizontally behind a load balancer. |
| Database Scaling | Database supports read replicas for reporting workloads. |
| Storage Scaling | Document storage scales independently (blob storage). |
| Background Processing | Background job processing scales independently of the API. |
| Growth | Designed to support 10x current user base without architectural changes. |

---

## 6. Role-Based Access Control

| Requirement | Detail |
|-------------|--------|
| Granularity | Permissions defined at action level (create, read, update, delete per entity). |
| Hierarchy | Roles inherit from base roles where appropriate. |
| Dynamic | Roles and permissions are configurable without code changes. |
| Enforcement | Enforced at API level (not just UI). |
| Audit | Role changes and permission modifications are audited. |

---

## 7. Data Protection

| Requirement | Detail |
|-------------|--------|
| GDPR Compliance | Support for data subject access requests and right to erasure (where legally permissible). |
| Data Classification | Data categorised by sensitivity (public, internal, confidential, restricted). |
| Retention Policies | Configurable data retention and archival policies per data type. |
| Anonymisation | Support for anonymising data when retention periods expire. |
| Consent | Client consent recorded where required. |

---

## 8. Searchability

| Requirement | Detail |
|-------------|--------|
| Global Search | Users can search across clients, matters, documents, and notes from a single search bar. |
| Contextual Search | Search within a specific matter or client context. |
| Filters | Advanced filtering by date, status, type, assignee, etc. |
| Relevance | Results ranked by relevance. |
| Speed | Search results within 2 seconds for standard queries. |

---

## 9. Reporting

| Requirement | Detail |
|-------------|--------|
| Real-Time Dashboards | Key metrics displayed in real-time (or near real-time). |
| Scheduled Reports | Support for scheduled report generation and distribution. |
| Export | Reports exportable to PDF, Excel, and CSV. |
| Custom Filters | Users can filter reports by date range, team, client, matter type, etc. |
| Historical Data | Reports can query historical data for trend analysis. |
| Performance | Reports should not impact operational database performance (use read replicas or separate reporting store). |

---

## 10. Supportability

| Requirement | Detail |
|-------------|--------|
| Error Messages | User-friendly error messages with correlation IDs for support reference. |
| Diagnostics | Support users can view logs and trace requests by correlation ID. |
| Health Checks | Automated health check endpoints for monitoring. |
| Documentation | Runbooks for common support scenarios. |
| Impersonation | Support users can impersonate other users (audited) for troubleshooting. |

---

## 11. Maintainability

| Requirement | Detail |
|-------------|--------|
| Clean Architecture | Separation of concerns with clear boundaries between layers. |
| Code Standards | Consistent naming conventions, folder structure, and coding patterns. |
| Dependency Management | Dependencies are pinned and regularly updated. |
| Documentation | Code, API, and architecture documentation maintained alongside the codebase. |
| Testability | All business logic is unit-testable. Integration tests cover critical paths. |
| Onboarding | A new developer should be productive within 1 week using documentation alone. |

---

## 12. Observability

| Requirement | Detail |
|-------------|--------|
| Structured Logging | All logs are structured (JSON) with correlation IDs. |
| Metrics | Key metrics (request rate, error rate, latency, queue depth) are collected. |
| Distributed Tracing | Requests are traceable across API, database, and background services. |
| Alerting | Alerts configured for critical thresholds (error rate spikes, high latency, disk usage). |
| Dashboards | Operational dashboards for platform health and performance. |

---

## 13. Accessibility

| Requirement | Detail |
|-------------|--------|
| Standard | WCAG 2.1 Level AA compliance. |
| Keyboard Navigation | All features accessible via keyboard. |
| Screen Readers | Semantic HTML and ARIA labels for screen reader compatibility. |
| Contrast | Sufficient colour contrast ratios. |
| Focus Management | Clear focus indicators throughout the application. |

---

## 14. Internationalisation (Future Consideration)

| Requirement | Detail |
|-------------|--------|
| Locale | Support for multiple locales (date format, number format, currency). |
| Language | UI text extracted into resource files for potential future translation. |
| Timezone | All dates stored in UTC, displayed in user's local timezone. |
