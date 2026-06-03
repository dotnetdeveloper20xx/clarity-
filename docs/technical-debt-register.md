# Technical Debt Register

This document tracks known technical debt in the Clarity platform. Each item includes impact, risk, and target resolution timeline.

## Active Debt

| # | Item | Risk | Impact | Owner | Target |
|---|------|------|--------|-------|--------|
| TD-001 | SHA256 password hashing (should be BCrypt/Argon2) | High | Security vulnerability if database is compromised | Backend | Before production |
| TD-002 | AutoMapper 13 has known vulnerability | Medium | Security advisory, no known exploit path | Backend | Next sprint |
| TD-003 | No rate limiting on login endpoint | Medium | Brute force attack possible (mitigated by lockout) | Backend | Before production |
| TD-004 | Matter-level access not enforced in query handlers | Medium | Users may see matters they shouldn't (mitigated by role checks) | Backend | Phase 8b |
| TD-005 | Dashboard data not cached | Low | Each request hits database (acceptable at current scale) | Backend | When >100 concurrent users |
| TD-006 | No EF Core migrations created yet (schema exists in code only) | High | Database must be created manually | Backend | Before first deployment |
| TD-007 | Angular `angular.json` not created (system restriction) | Medium | Frontend requires manual `ng new` or manual creation | Frontend | Immediate |
| TD-008 | No email sending implementation | Low | Notifications are in-app only (email is future feature) | Backend | Phase 11 |
| TD-009 | No SignalR real-time updates | Low | Notifications require page refresh (polling) | Backend | Phase 11 |
| TD-010 | Export endpoints load all data to memory | Low | Works fine for <50K records, need streaming for scale | Backend | When data exceeds 50K |
| TD-011 | No HTTPS certificate configured for Docker | Low | Development only, not production issue | DevOps | Before production |
| TD-012 | Swagger enabled in all environments (should be disabled in Prod) | Low | Configuration exists but needs env-specific toggle in code | Backend | Before production |

## Resolved Debt

| # | Item | Resolved | How |
|---|------|----------|-----|
| — | None resolved yet | — | — |

## Process

When adding new debt:
1. Create entry with description, risk level, and impact
2. Assign an owner (team or individual)
3. Set a target resolution date
4. Review register monthly in sprint planning

When resolving debt:
1. Move to "Resolved" section with date and method
2. Update any related documentation
