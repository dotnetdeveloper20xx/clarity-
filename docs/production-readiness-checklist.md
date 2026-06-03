# Production Readiness Checklist

## Before First Production Release

### Security
- [ ] JWT signing key moved to Azure Key Vault (not in appsettings)
- [ ] Database connection string in Key Vault
- [ ] Password hashing upgraded from SHA256 to BCrypt/Argon2
- [ ] HTTPS enforced (HSTS headers)
- [ ] CORS restricted to production domain only
- [ ] Rate limiting on login endpoint
- [ ] Security headers configured (X-Content-Type-Options, X-Frame-Options, CSP)
- [ ] Swagger disabled in Production environment

### Database
- [ ] All migrations reviewed and tested against realistic data
- [ ] Indexes from Phase 3 design applied
- [ ] Backup strategy configured (daily automated backups)
- [ ] Point-in-time restore verified
- [ ] Connection pool sizing configured

### Monitoring & Observability
- [ ] Application Insights connected (or equivalent APM)
- [ ] Structured logs shipping to centralised logging
- [ ] Health check endpoint monitored externally
- [ ] Error rate alerting configured
- [ ] Slow request alerting configured (>2s)
- [ ] Failed background job alerting configured

### Deployment
- [ ] CI pipeline passing (build + test)
- [ ] Release pipeline tested end-to-end
- [ ] Rollback procedure documented and tested
- [ ] Environment variables set for production
- [ ] Feature flags reviewed (nothing accidentally enabled/disabled)
- [ ] DNS and SSL certificates configured

### Data
- [ ] Seed data removed or replaced with production-appropriate data
- [ ] Data retention policies configured
- [ ] GDPR data handling documented
- [ ] Backup encryption enabled

### Testing
- [ ] All unit tests passing
- [ ] Integration tests passing against staging database
- [ ] UAT signed off by business stakeholders
- [ ] Load testing completed (target: 500 concurrent users)
- [ ] Security scan completed (no critical/high vulnerabilities)

### Documentation
- [ ] Runbooks for common support scenarios
- [ ] Incident response procedure documented
- [ ] On-call rotation defined
- [ ] Stakeholder communication plan for outages

### Sign-off
- [ ] Technical lead approved
- [ ] Release manager approved
- [ ] Business owner approved

---

## Before Every Release

- [ ] All tests passing
- [ ] No critical/high security vulnerabilities
- [ ] Database migrations reviewed
- [ ] Release notes prepared
- [ ] Rollback plan documented
- [ ] UAT sign-off (if feature release)
- [ ] Monitoring dashboards checked (baseline metrics noted)
- [ ] Post-deployment verification steps defined
