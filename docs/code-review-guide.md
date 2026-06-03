# Code Review Guide

## Purpose

Code review is not about criticism. It is team learning and quality protection.

## What Reviewers Should Check

### Architecture & Design
- [ ] Business logic is in the Application layer (not controllers)
- [ ] Controllers are thin (create command/query → send via MediatR → return result)
- [ ] Domain entities don't reference Infrastructure
- [ ] New dependencies are justified

### Security
- [ ] Authorization enforced on endpoints
- [ ] User inputs validated (FluentValidation)
- [ ] No secrets hardcoded
- [ ] Sensitive data not logged
- [ ] SQL injection not possible (parameterised queries via EF Core)

### Business Logic
- [ ] Business rules are correct
- [ ] Edge cases handled
- [ ] Audit logging for significant actions
- [ ] Notifications sent where expected

### Performance
- [ ] Queries use `AsNoTracking()` for reads
- [ ] No N+1 queries (use `Include()` or projection)
- [ ] Pagination on list endpoints
- [ ] No unbounded queries

### Readability
- [ ] Code is self-documenting (clear naming)
- [ ] Complex logic has comments explaining WHY
- [ ] Consistent with existing patterns
- [ ] No dead code or commented-out code

### Testing
- [ ] New business rules have tests
- [ ] Validators have tests
- [ ] Existing tests still pass
- [ ] Edge cases tested

### Error Handling
- [ ] Appropriate exceptions thrown (NotFoundException, ValidationException)
- [ ] User-facing messages are helpful
- [ ] Internal errors don't leak details

## PR Description Template

```markdown
## What
Brief description of the change.

## Why
Business reason or bug being fixed.

## How
Technical approach.

## Testing
How was this tested? What should reviewers verify?

## Checklist
- [ ] Tests pass
- [ ] No new warnings
- [ ] Documentation updated (if applicable)
```

## Review Etiquette

- Be specific: "This query could be slow with 100K records because..." not "This is bad"
- Suggest alternatives: "Consider using projection here to avoid loading the full entity"
- Distinguish blockers from suggestions: "Blocking: missing auth check" vs "Suggestion: could rename for clarity"
- Approve when satisfied — don't let perfect block good
