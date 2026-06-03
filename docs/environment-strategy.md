# Environment Strategy

## Environments

| Environment | Purpose | URL | Database | Deployment |
|-------------|---------|-----|----------|-----------|
| Local | Developer workstation | https://localhost:5001 | DESKTOP-VVJN96B\ClarityDb | Manual |
| Docker Local | Full stack in containers | http://localhost:5001 | Docker SQL Server | docker compose up |
| Development | Team integration | https://dev.clarity.internal | Dev SQL Server | Auto on develop merge |
| Test/QA | Quality assurance | https://test.clarity.internal | Test SQL Server | Auto on develop merge |
| UAT | Business acceptance | https://uat.clarity.internal | UAT SQL Server | Manual (approval gate) |
| Production | Live users | https://clarity.example.com | Azure SQL | Manual (approval gate) |

## Configuration per Environment

Each environment overrides settings via:
- `appsettings.{Environment}.json` — non-secret config
- Environment variables — secret overrides
- Azure Key Vault — production secrets

### What Changes Between Environments

| Setting | Local | Dev | Test | UAT | Production |
|---------|-------|-----|------|-----|-----------|
| Log Level | Debug | Information | Information | Information | Warning |
| DB Connection | Local SQL | Dev SQL | Test SQL | UAT SQL | Azure SQL (Key Vault) |
| JWT Key | Dev key (appsettings) | Dev key | Test key | UAT key | Key Vault |
| CORS Origins | localhost:4200 | dev domain | test domain | uat domain | prod domain |
| Swagger | Enabled | Enabled | Enabled | Enabled | Disabled |
| Feature Flags | All enabled | All enabled | All enabled | Selective | Production set |

## ASPNETCORE_ENVIRONMENT Values

- `Development` — local development, verbose logging, Swagger enabled
- `Staging` — UAT/pre-production, moderate logging
- `Production` — live, minimal logging, no Swagger, strict security

## Docker Compose (Local Full Stack)

```bash
docker compose up -d        # Start all services
docker compose logs -f api  # Follow API logs
docker compose down         # Stop all services
docker compose down -v      # Stop and remove data volumes
```

## Environment Variables (Production)

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<from Key Vault>
Jwt__Key=<from Key Vault>
Jwt__Issuer=Clarity
Jwt__Audience=ClarityUsers
```
