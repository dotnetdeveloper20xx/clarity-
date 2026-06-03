# Testing Strategy

## Test Pyramid

```
         ╱╲
        ╱  ╲         UI/E2E Tests (few, slow, fragile)
       ╱────╲
      ╱      ╲       Integration Tests (moderate)
     ╱────────╲
    ╱          ╲     Unit Tests (many, fast, reliable)
   ╱────────────╲
```

## Test Categories

### Unit Tests (Fast, Isolated)

**What to test:**
- Validator rules (FluentValidation)
- Domain entity defaults and business rules
- Status transition validators
- Pure calculation logic

**Where:** `tests/Clarity.Tests/`

**Run:** `dotnet test` (all tests run in <1 second)

### Architecture Tests

**What to test:**
- Domain doesn't reference Infrastructure
- Application doesn't reference Api
- Dependency direction is correct
- Entities live in Domain project

**Where:** `tests/Clarity.Tests/Architecture/`

**Purpose:** Prevent architectural decay over time.

### Integration Tests (Future)

**What to test:**
- Full command execution against in-memory database
- Workflow sequences (create → submit → approve → bill)
- API endpoint responses with WebApplicationFactory

**Where:** `tests/Clarity.Tests/Integration/` (to be added)

### API Tests (Future)

**What to test:**
- Correct HTTP status codes
- Authentication enforcement
- Authorization policies
- Pagination and filtering
- Error response format

**Where:** `tests/Clarity.Tests/Api/` (to be added)

## Current Test Inventory

| Category | Count | Focus |
|----------|-------|-------|
| Domain entity defaults | 8 | Default values, enums |
| Client validation | 4 | FluentValidation rules |
| Time entry validation | 5 | Business rule validation |
| Matter workflow transitions | 14 | Valid/invalid state changes |
| Time entry workflow transitions | 11 | Valid/invalid state changes |
| Invoice workflow transitions | 9 | Valid/invalid state changes |
| Security (refresh tokens) | 5 | Token active/expired/revoked |
| Architecture rules | 11 | Dependency direction |
| Invoice calculations | 7 | Financial correctness |
| Payment allocation | 7 | Payment business logic |

**Total: 80+ tests** covering critical business behaviour.

## Running Tests

```bash
dotnet test                           # Run all tests
dotnet test --filter "Category=Unit"  # Run specific category
dotnet test --verbosity normal        # Detailed output
dotnet test --logger trx              # Generate TRX report
```

## Test Naming Convention

```
MethodUnderTest_Scenario_ExpectedResult
```

Examples:
- `Validator_ShouldFail_WhenNameIsEmpty`
- `MatterStatusTransition_ShouldValidateCorrectly`
- `RefreshToken_IsActive_WhenNotRevokedAndNotExpired`

## When to Write Tests

- **Always**: New validators, new workflow transitions, new business rules
- **Always**: Bug fixes (write a test that reproduces the bug first)
- **Usually**: New handlers with non-trivial logic
- **Skip**: Simple CRUD with no business rules, UI layout tests
