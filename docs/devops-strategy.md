# DevOps Strategy

## Overview

Clarity uses a structured DevOps approach to ensure safe, repeatable, and traceable delivery from development to production.

## Environment Strategy

| Environment | Purpose | Deployment | Database |
|-------------|---------|-----------|----------|
| Local | Individual developer work | Manual (dotnet run) | DESKTOP-VVJN96B (Windows Auth) |
| Development | Team integration testing | Automatic on develop push | Shared dev DB |
| Test/QA | Quality assurance | Automatic on develop push | Isolated test DB |
| UAT | Business user acceptance | Manual approval gate | UAT DB with realistic data |
| Production | Live users | Manual approval gate | Production DB (Azure SQL) |

## Running Locally

### Without Docker

```bash
# Backend
cd src/Clarity.Api
dotnet run

# Frontend
cd src/Clarity.Web
npm install
npm start
```

### With Docker

```bash
docker compose up
```

This starts SQL Server + API. Access API at `http://localhost:5001`.

## Branching Strategy

```
main ─────────────────────────────────── (production-ready)
  │
  └─ develop ─────────────────────────── (integration)
       │
       ├─ feature/client-search ──────── (new feature)
       ├─ feature/invoice-pdf ────────── (new feature)
       ├─ bugfix/time-entry-validation ─ (bug fix)
       │
       └─ release/1.2.0 ─────────────── (stabilisation)

hotfix/critical-fix ──────────────────── (urgent production fix)
```

### Rules

- `main` = production-ready code (never push directly)
- `develop` = integration branch (PRs required)
- `feature/*` = new functionality (branch from develop)
- `bugfix/*` = non-urgent fixes (branch from develop)
- `release/*` = stabilisation before production (branch from develop)
- `hotfix/*` = urgent production fixes (branch from main)

## CI/CD Pipelines

### CI Pipeline (`.github/workflows/ci.yml`)

Triggered on: push to main/develop, PRs to main/develop.

Steps:
1. Restore .NET packages
2. Build in Release mode
3. Run all tests
4. Build Angular frontend
5. Run frontend lint

### Release Pipeline (`.github/workflows/release.yml`)

Triggered on: Git tag `v*`.

Steps:
1. Build and test backend
2. Publish API artifact
3. Build frontend production
4. Upload artifacts

## Deployment Process

```
Developer → PR → CI passes → Code review → Merge to develop
                                              │
                                              ▼
                                    Auto-deploy to Dev/Test
                                              │
                                              ▼ (manual approval)
                                    Deploy to UAT
                                              │
                                              ▼ (manual approval)
                                    Deploy to Production
```

## Database Deployment

1. Generate migration script: `dotnet ef migrations script`
2. Review SQL in PR
3. Apply to Dev/Test automatically
4. Apply to UAT/Production during deployment window with approval

## Rollback Strategy

| Layer | Rollback Method |
|-------|----------------|
| Application | Redeploy previous artifact |
| Database | Forward-fix migration (new migration to undo) |
| Configuration | Revert config in Key Vault |

**Critical rule**: Never use `dotnet ef database update <previous>` in production. Always create a new forward migration.
