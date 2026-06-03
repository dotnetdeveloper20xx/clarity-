# Error Handling

## Principle

Clarity uses a centralized error handling strategy:

- **Never leak internal details** to API consumers
- **Always log full details** internally for debugging
- **Always include a correlation ID** for support traceability
- **Return consistent JSON** error responses

## Error Response Format

All errors return this structure:

```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "correlationId": "abc123def456",
  "errors": {
    "PropertyName": ["Error message"]
  }
}
```

## HTTP Status Codes

| Code | When Used |
|------|-----------|
| 200 | Successful GET or action |
| 201 | Successful POST (resource created) |
| 204 | Successful PUT/DELETE (no content returned) |
| 400 | Validation failure or business rule violation |
| 401 | Not authenticated (missing or invalid token) |
| 403 | Authenticated but not authorized |
| 404 | Resource not found |
| 500 | Unexpected server error |

## Exception Hierarchy

| Exception Type | HTTP Code | Meaning |
|---------------|-----------|---------|
| ValidationException | 400 | FluentValidation rules failed |
| NotFoundException | 404 | Entity not found by ID |
| ForbiddenException | 403 | User lacks permission |
| Any other exception | 500 | Unexpected error |

## Implementation

### ExceptionHandlingMiddleware

Located in `Clarity.Api/Middleware/ExceptionHandlingMiddleware.cs`. Wraps the entire request pipeline and catches all unhandled exceptions.

### ValidationBehaviour

Located in `Clarity.Application/Common/Behaviours/ValidationBehaviour.cs`. Runs FluentValidation rules before the handler executes. Throws `ValidationException` if any rule fails.

## For Support Teams

When a user reports an error:

1. Ask for the correlation ID (shown in the error response)
2. Search logs for that correlation ID
3. The logs will show: request path, user, timing, and full exception details

## For Developers

To add validation to a new command:

1. Create a validator class inheriting `AbstractValidator<YourCommand>`
2. Place it in the same folder as the command
3. It will be auto-discovered and executed by the pipeline
