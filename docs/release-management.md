# Release Management

## Versioning

Clarity uses Semantic Versioning: `MAJOR.MINOR.PATCH`

- **MAJOR** — Breaking changes (API contract changes, major redesigns)
- **MINOR** — New features (backwards compatible)
- **PATCH** — Bug fixes, security patches

Example: `v1.3.2`

## Release Process

### 1. Prepare Release

```
git checkout develop
git checkout -b release/1.3.0
```

- Stabilise: fix remaining bugs, update version numbers
- No new features in release branches
- Update CHANGELOG.md

### 2. Test

- CI pipeline runs automatically
- Deploy to UAT environment
- Business stakeholders perform acceptance testing
- Fix any issues found in the release branch

### 3. Approve

- Technical lead reviews and approves
- Business owner signs off UAT
- Release manager schedules deployment window

### 4. Deploy

```
git checkout main
git merge release/1.3.0
git tag v1.3.0
git push origin main --tags
```

The release pipeline triggers on the tag and:
- Builds artifacts
- Deploys to production (with approval gate)

### 5. Post-Deploy

- Verify health checks pass
- Monitor error rates for 30 minutes
- Confirm key user journeys work
- Merge release branch back to develop

## Hotfix Process

For urgent production fixes:

```
git checkout main
git checkout -b hotfix/fix-payment-calculation
# Fix the issue
git checkout main
git merge hotfix/fix-payment-calculation
git tag v1.3.1
git push origin main --tags
# Also merge back to develop
git checkout develop
git merge hotfix/fix-payment-calculation
```

## Release Notes Template

```markdown
# Release v1.3.0 — [Date]

## Features
- Added financial dashboard with aged debt breakdown
- Added global search across clients and matters

## Fixes
- Fixed time entry duration validation for entries over 8 hours
- Fixed invoice total calculation rounding issue

## Database Changes
- Added index on TimeEntries.Date column
- New table: SecurityAuditLogs

## Known Issues
- Document preview not available for PDF files over 50MB

## Rollback Notes
- Safe to rollback to v1.2.x (no destructive DB changes)
- SecurityAuditLogs table will remain (no data dependency)
```

## Feature Flags

New features can be deployed behind feature flags:

```json
{
  "FeatureFlags": {
    "EnableSignalR": false,
    "EnableEmailNotifications": false,
    "EnableAdvancedReporting": true
  }
}
```

This allows:
- Deploy code without enabling for users
- Gradual rollout (enable for admin first, then all)
- Quick disable if issues arise (no redeployment needed)
