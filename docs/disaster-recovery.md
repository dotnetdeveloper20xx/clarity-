# Disaster Recovery Plan

## Recovery Objectives

| Metric | Target | Meaning |
|--------|--------|---------|
| RPO (Recovery Point Objective) | 1 hour | Maximum acceptable data loss |
| RTO (Recovery Time Objective) | 4 hours | Maximum acceptable downtime |

## Scenarios

### 1. Database Failure

**Impact**: All operations unavailable.

**Recovery Steps**:
1. Azure SQL automatic failover engages (if configured)
2. If no auto-failover: restore from most recent backup
3. Verify data integrity
4. Resume application connections
5. Investigate root cause

**Backups**: Daily full + hourly differential + 5-minute transaction log backups.

### 2. Application Hosting Failure

**Impact**: API unavailable, frontend cannot load.

**Recovery Steps**:
1. Azure App Service auto-heals (restarts instance)
2. If persistent: scale to new instance
3. If region-wide: deploy to secondary region (if configured)
4. Verify health endpoint returns Healthy
5. Investigate root cause

### 3. Document Storage Failure

**Impact**: Documents cannot be uploaded or downloaded.

**Recovery Steps**:
1. Azure Blob Storage has geo-redundant replication
2. If primary region unavailable: switch to secondary read endpoint
3. Queue document operations until storage recovers
4. Verify all documents accessible after recovery

### 4. Deployment Failure

**Impact**: New release causes errors.

**Recovery Steps**:
1. Immediately roll back to previous deployment artifact
2. Verify health endpoint and error rates return to baseline
3. Investigate what went wrong in the release
4. Fix, re-test, and re-deploy

### 5. Security Breach

**Impact**: Potential data exposure.

**Recovery Steps**:
1. Revoke all active sessions immediately
2. Rotate JWT signing key
3. Rotate database credentials
4. Lock affected accounts
5. Review security audit logs for scope of breach
6. Notify affected parties as required by regulation
7. Engage incident response team

## Contact List

| Role | Responsibility |
|------|---------------|
| On-call Engineer | First response, initial diagnosis |
| Technical Lead | Escalation, architecture decisions |
| DBA | Database recovery, performance |
| Release Manager | Deployment rollback |
| Security Lead | Breach response |
| Business Owner | Stakeholder communication |

## Testing

- Disaster recovery procedures should be tested quarterly
- Database restore tested monthly
- Deployment rollback tested before every major release
