# Definition of Done

A feature is considered "done" only when ALL of the following are satisfied:

## Code Quality
- [ ] Code compiles without errors or warnings
- [ ] Code follows existing patterns and conventions
- [ ] No TODO comments left (unless tracked as technical debt)
- [ ] No dead or commented-out code

## Business Logic
- [ ] Business requirement is fully implemented
- [ ] Validation exists for all user inputs
- [ ] Authorization enforced on endpoints
- [ ] Audit logging for significant actions
- [ ] Error messages are user-friendly

## Testing
- [ ] Unit tests for business rules and validators
- [ ] All existing tests still pass
- [ ] Edge cases and failure paths tested
- [ ] Manual testing completed (developer self-test)

## UI (if applicable)
- [ ] Loading states shown during async operations
- [ ] Error states shown when operations fail
- [ ] Empty states shown when no data exists
- [ ] Responsive layout works on common screen sizes
- [ ] Role-based visibility enforced

## Documentation
- [ ] API endpoints documented (Swagger annotations or docs update)
- [ ] Complex logic has code comments explaining WHY
- [ ] README or relevant doc updated if significant

## Review
- [ ] Pull request created with clear description
- [ ] Code review completed and approved
- [ ] Review feedback addressed

## Deployment
- [ ] Database migration reviewed (if applicable)
- [ ] No breaking changes to existing API contracts (unless versioned)
- [ ] Feature flag used if feature needs gradual rollout
