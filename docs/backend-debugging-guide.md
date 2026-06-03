# Backend Debugging Guide

## Logs

Clarity uses Serilog with structured logging. Logs are written to:

- **Console** — visible during development
- **File** — `logs/clarity-YYYYMMDD.log` (rolling daily)

### Log Levels

| Level | Usage |
|-------|-------|
| Debug | Detailed diagnostic information |
| Information | Normal request flow, handler execution |
| Warning | Handled exceptions, slow requests (>500ms) |
| Error | Unhandled exceptions, critical failures |

### Correlation ID

Every request gets a unique correlation ID. It appears in:
- Response header: `X-Correlation-Id`
- All log entries for that request
- Error responses

To trace a request: search logs for the correlation ID.

## Common Issues

### "Entity not found" (404)

- Check the ID is correct (GUID format)
- Check the record hasn't been soft-deleted
- If using query filters, deleted records are automatically excluded

### Validation Errors (400)

- Check the `errors` object in the response
- Each key is a property name, values are the error messages
- Validators are in the same folder as the command

### "Unauthorized" (401)

- Token has expired (8-hour lifetime)
- Token is missing from the Authorization header
- Token format: `Bearer <token>` (note the space)

### "Forbidden" (403)

- User is authenticated but lacks the required role
- Check endpoint's `[Authorize(Roles = "...")]` attribute
- Check user's role assignments in the database

### Database Connection Issues

- Verify LocalDB is running: `sqllocaldb info mssqllocaldb`
- Verify connection string in `appsettings.json`
- Check that migrations have been applied

## Debugging Steps

1. **Reproduce the issue** — note the correlation ID
2. **Search logs** — filter by correlation ID
3. **Check the request** — path, method, user, body
4. **Check the handler** — is the logic correct for this case?
5. **Check the database** — is the data in the expected state?
6. **Check permissions** — does the user have the required role?

## Performance

The `LoggingBehaviour` flags requests taking longer than 500ms. Search logs for "Long running request" to find slow operations.

## Database Queries

To see EF Core SQL queries in logs, set the minimum level for `Microsoft.EntityFrameworkCore.Database.Command` to `Information` in `appsettings.Development.json`.
