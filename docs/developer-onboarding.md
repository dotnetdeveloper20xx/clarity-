# Developer Onboarding Guide

Welcome to Clarity. This guide will get you productive within your first week.

## Prerequisites

- [x] .NET 10 SDK installed (`dotnet --version`)
- [x] Node.js 20+ installed (`node --version`)
- [x] SQL Server available (local or Docker)
- [x] Git configured
- [x] IDE: Visual Studio 2022+ or VS Code with C# extension

## Step 1: Clone and Build

```bash
git clone <repository-url>
cd clarity-
dotnet restore
dotnet build
dotnet test
```

All 59+ tests should pass. If they don't, check your .NET SDK version.

## Step 2: Database Setup

The connection string in `src/Clarity.Api/appsettings.json` points to `DESKTOP-VVJN96B` with Windows Auth. Update this to your local SQL Server instance.

```bash
cd src/Clarity.Api
dotnet ef database update
```

This creates the database and applies all migrations.

## Step 3: Run the API

```bash
cd src/Clarity.Api
dotnet run
```

Navigate to `https://localhost:5001/swagger` to see the API documentation.

## Step 4: Test Login

POST to `/api/auth/login`:
```json
{ "email": "admin@clarity.local", "password": "Admin123!" }
```

Copy the returned token. Use it in Swagger's "Authorize" button as `Bearer <token>`.

## Step 5: Run the Frontend

```bash
cd src/Clarity.Web
npm install
npm start
```

Navigate to `http://localhost:4200`. Login with the same credentials.

## Step 6: Understand the Architecture

Read these docs in order:
1. `docs/business-overview.md` — what the platform does
2. `docs/architecture-overview.md` — how it's structured
3. `docs/project-structure.md` — where things live
4. `docs/backend-architecture.md` — backend patterns
5. `docs/frontend-architecture.md` — frontend patterns

## Key Concepts

| Concept | Location | Purpose |
|---------|----------|---------|
| Entity | `Domain/Entities/` | Business object with identity |
| Command | `Application/{Feature}/Commands/` | Write operation |
| Query | `Application/{Feature}/Queries/` | Read operation |
| Handler | Same folder as Command/Query | Business logic execution |
| Validator | Same folder as Command | Input validation rules |
| Controller | `Api/Controllers/` | Thin HTTP endpoint |
| Store | `Web/src/app/core/stores/` | Angular feature state |
| API Service | `Web/src/app/core/services/` | Angular HTTP client |

## Development Workflow

1. Pick a task/feature
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Implement across all layers (see `docs/how-to-add-a-feature.md`)
4. Write/update tests
5. Run `dotnet test` to verify
6. Commit and push
7. Create a Pull Request
8. Get code review
9. Merge to develop

## Useful Commands

```bash
dotnet build                          # Build entire solution
dotnet test                           # Run all tests
dotnet run --project src/Clarity.Api  # Start API
dotnet ef migrations add <Name> --project src/Clarity.Infrastructure --startup-project src/Clarity.Api
dotnet ef database update --project src/Clarity.Infrastructure --startup-project src/Clarity.Api
```

## Getting Help

- Check `docs/` folder for detailed guides
- Check `docs/backend-debugging-guide.md` for troubleshooting
- Check `docs/error-handling.md` for API error patterns
- Ask the team in the dev channel
