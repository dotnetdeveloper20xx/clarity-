# Kiro Command Permissions

This document anticipates every command that may reasonably be needed throughout the life of this project. Commands are categorised by purpose, risk level, and approval requirements.

---

## Solution Inspection

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `dir` / `Get-ChildItem` | List directory contents | Low | No | Discover files and folders |
| `type` / `Get-Content` | View file contents | Low | No | Read configuration, inspect code |
| `dotnet sln list` | List projects in solution | Low | No | Verify solution structure |
| `dotnet list package` | List NuGet packages | Low | No | Check dependencies |
| `dotnet list reference` | List project references | Low | No | Verify dependency direction |
| `tree` | Show directory tree | Low | No | Visualise project structure |

---

## .NET Development

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `dotnet new sln` | Create solution file | Low | No | Initial project setup |
| `dotnet new webapi` | Create Web API project | Low | No | Create API project |
| `dotnet new classlib` | Create class library | Low | No | Create Domain/Application/Infrastructure projects |
| `dotnet new xunit` / `dotnet new nunit` | Create test project | Low | No | Create test projects |
| `dotnet sln add` | Add project to solution | Low | No | Wire up solution |
| `dotnet add reference` | Add project reference | Low | No | Configure dependency direction |
| `dotnet add package` | Add NuGet package | Low | No | Install dependencies |
| `dotnet remove package` | Remove NuGet package | Low | No | Clean up unused packages |
| `dotnet restore` | Restore packages | Low | No | Resolve dependencies |
| `dotnet build` | Build solution | Low | No | Compile and verify code |
| `dotnet clean` | Clean build output | Low | No | Remove cached binaries |
| `dotnet test` | Run tests | Low | No | Execute test suites |
| `dotnet run` | Run application | Low | No | Start API for testing |
| `dotnet watch` | Run with hot reload | Low | No | Development iteration |
| `dotnet format` | Format code | Low | No | Enforce code style |
| `dotnet publish` | Publish for deployment | Low | No | Create deployment artifact |
| `dotnet user-secrets init` | Initialize user secrets | Low | No | Set up local secret storage |
| `dotnet user-secrets set` | Set a secret value | Low | No | Configure local secrets |
| `dotnet tool install` | Install .NET tool | Low | No | Add global/local tools |
| `dotnet tool restore` | Restore .NET tools | Low | No | Restore tools from manifest |
| `dotnet ef migrations add` | Create EF migration | Low | No | Schema change management |
| `dotnet ef migrations remove` | Remove last migration | Low | No | Undo unapplied migration |
| `dotnet ef migrations list` | List migrations | Low | No | View migration history |
| `dotnet ef migrations script` | Generate SQL script | Low | No | Review migration SQL |
| `dotnet ef database update` | Apply migrations | Medium | No | Apply schema to local DB |
| `dotnet ef database drop` | Drop database | Medium | No | Reset local database |
| `dotnet ef dbcontext info` | Show DbContext info | Low | No | Verify EF configuration |
| `dotnet ef dbcontext scaffold` | Scaffold from DB | Low | No | Reverse-engineer schema |

---

## Frontend Development

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `npm init` | Initialize package.json | Low | No | Create new Node project |
| `npm install` | Install all packages | Low | No | Restore node_modules |
| `npm install <package>` | Install specific package | Low | No | Add dependency |
| `npm uninstall <package>` | Remove package | Low | No | Remove dependency |
| `npm run build` | Build frontend | Low | No | Production build |
| `npm run start` / `ng serve` | Start dev server | Low | No | Local development |
| `npm run test` / `ng test` | Run frontend tests | Low | No | Execute test suite |
| `npm run lint` / `ng lint` | Lint code | Low | No | Check code quality |
| `npm audit` | Security audit | Low | No | Check for vulnerabilities |
| `npm ci` | Clean install | Low | No | Reproducible builds |
| `npx` | Execute package binary | Low | No | Run one-off commands |
| `ng new` | Create Angular project | Low | No | Initialize Angular app |
| `ng generate component` | Generate component | Low | No | Scaffold component |
| `ng generate service` | Generate service | Low | No | Scaffold service |
| `ng generate module` | Generate module | Low | No | Scaffold module |
| `ng generate guard` | Generate guard | Low | No | Scaffold route guard |
| `ng generate interceptor` | Generate interceptor | Low | No | Scaffold HTTP interceptor |
| `ng generate pipe` | Generate pipe | Low | No | Scaffold pipe |
| `ng generate directive` | Generate directive | Low | No | Scaffold directive |
| `ng generate interface` | Generate interface | Low | No | Scaffold interface |
| `ng generate enum` | Generate enum | Low | No | Scaffold enum |
| `ng add` | Add library/schematic | Low | No | Add Angular library |
| `ng update` | Update Angular packages | Medium | No | Upgrade framework |
| `ng build --configuration=production` | Production build | Low | No | Optimised build |
| `npx tailwindcss init` | Initialize Tailwind | Low | No | Configure Tailwind CSS |

---

## Database Commands

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `dotnet ef database update` | Apply pending migrations | Medium | No | Update local schema |
| `dotnet ef database drop` | Drop local database | Medium | No | Reset for fresh start |
| `dotnet ef migrations add` | Create new migration | Low | No | Evolve schema |
| `dotnet ef migrations remove` | Remove unapplied migration | Low | No | Fix migration mistake |
| `dotnet ef migrations script` | Generate SQL | Low | No | Review changes |
| `sqlcmd` (local) | Run SQL against local DB | Medium | No | Seed data, verify schema |
| SQL Server Management Studio | GUI database access | Low | No | Visual inspection |

---

## Git Commands

### Safe Commands (No Approval Required)

| Command | Purpose | Risk Level |
|---------|---------|-----------|
| `git status` | Check working state | Low |
| `git log` | View history | Low |
| `git diff` | View changes | Low |
| `git branch` | List branches | Low |
| `git branch <name>` | Create branch | Low |
| `git checkout <branch>` | Switch branch | Low |
| `git checkout -b <branch>` | Create and switch | Low |
| `git switch <branch>` | Switch branch (modern) | Low |
| `git add <files>` | Stage specific files | Low |
| `git commit -m "message"` | Commit staged changes | Low |
| `git pull` | Pull from remote | Low |
| `git push` | Push to remote | Low |
| `git push -u origin <branch>` | Push new branch | Low |
| `git fetch` | Fetch remote state | Low |
| `git stash` | Stash changes | Low |
| `git stash pop` | Restore stash | Low |
| `git merge <branch>` | Merge branch | Low |
| `git tag` | List/create tags | Low |
| `git remote -v` | List remotes | Low |
| `git show` | Show commit details | Low |

### Commands Requiring Approval

| Command | Purpose | Risk Level | Why Approval Needed |
|---------|---------|-----------|---------------------|
| `git push --force` | Force push | High | Rewrites remote history |
| `git reset --hard` | Hard reset | High | Discards local changes |
| `git clean -fd` | Remove untracked files | High | Deletes files irreversibly |
| `git branch -D` | Force delete branch | Medium | May lose unmerged work |
| `git rebase` (interactive) | Rewrite history | High | Alters commit history |
| `git push origin --delete` | Delete remote branch | Medium | Removes shared branch |
| `git filter-branch` | Rewrite all history | High | Destructive to entire repo |

---

## Tooling Commands

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `node --version` | Check Node version | Low | No | Verify environment |
| `npm --version` | Check npm version | Low | No | Verify environment |
| `dotnet --version` | Check .NET version | Low | No | Verify environment |
| `dotnet --list-sdks` | List installed SDKs | Low | No | Verify SDK availability |
| `docker build` | Build container image | Low | No | Create Docker image |
| `docker run` | Run container | Low | No | Start local service |
| `docker compose up` | Start all services | Low | No | Local development environment |
| `docker compose down` | Stop all services | Low | No | Teardown local environment |
| `docker compose down -v` | Stop and remove volumes | Medium | No | Full reset of Docker state |
| `docker ps` | List running containers | Low | No | Check running services |
| `docker logs` | View container logs | Low | No | Debug container issues |
| `az login` | Azure CLI login | Low | No | Authenticate to Azure |
| `az account show` | Show current subscription | Low | No | Verify Azure context |
| `gh pr create` | Create pull request | Low | No | Submit code for review |
| `gh pr list` | List pull requests | Low | No | Check PR status |
| `gh pr merge` | Merge pull request | Medium | No | Complete review |

---

## File System Operations

| Command | Purpose | Risk Level | Requires Approval | Typical Use Cases |
|---------|---------|-----------|-------------------|-------------------|
| `mkdir` / `New-Item -ItemType Directory` | Create directory | Low | No | Project structure |
| Create/write files | Create source files | Low | No | Development |
| Rename files | Rename source files | Low | No | Refactoring |
| Delete individual file | Remove a source file | Low | No | Cleanup |
| `rmdir /s /q` (large directories) | Remove directory tree | Medium | No | Clean rebuild folders |
| Delete data files (*.mdf, *.ldf) | Remove database files | Medium | No | Database reset |

---

## Commands That Always Require Approval

These commands are never auto-approved regardless of context:

| Command / Action | Risk Level | Reason |
|-----------------|-----------|--------|
| Any `--force` operation on git remote | High | Rewrites shared history |
| Deleting production database | Critical | Data loss |
| Modifying `.env` or secrets files with real credentials | High | Security sensitive |
| Any command affecting external paid services | High | Cost implications |
| Global machine configuration changes | High | Affects other projects |
| Registry modifications | High | System-wide impact |
| Installing global npm packages | Medium | Machine-wide impact |
| `git push` to `main` directly | High | Bypasses review process |
| Deleting migration files already applied | High | Schema inconsistency |
| Running commands against non-local databases | Critical | Production data risk |

---

## Summary

- **All Low-risk commands:** Execute without asking.
- **Medium-risk commands:** Execute without asking in development context; mention what was done.
- **High-risk commands:** Always ask for explicit approval before executing.
- **Critical commands:** Never execute without written confirmation.
