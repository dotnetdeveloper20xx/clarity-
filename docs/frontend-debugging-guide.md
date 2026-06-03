# Frontend Debugging Guide

## Common Issues

### Login Not Working

1. Check the API is running (`https://localhost:5001/health`)
2. Check the browser console for CORS errors
3. Verify credentials match seed data (see `docs/api-guide.md`)
4. Check Network tab — is the login request reaching the backend?

### "Unauthorized" After Login

- Token may have expired (8-hour lifetime)
- Check `localStorage` → `clarity_token` exists
- Check the token is being attached (Network tab → Authorization header)
- The auth interceptor skips the login endpoint itself

### Data Not Loading

1. Check the API is running
2. Check browser Network tab for failed requests
3. Look at the response body for error details
4. Check the correlation ID in the response header `X-Correlation-Id`
5. Search backend logs for that correlation ID

### Blank Screen / White Page

- Open browser DevTools Console for JavaScript errors
- Check Angular compilation errors in the terminal running `ng serve`
- Verify routes in `app.routes.ts`

## Debugging Tools

| Tool | Purpose |
|------|---------|
| Browser DevTools Console | JavaScript errors, component logs |
| Network Tab | API requests, response codes, headers |
| Application Tab → Local Storage | Token and user data |
| Angular DevTools extension | Component tree, signals |

## Development Server

Run from `src/Clarity.Web`:
```
npm start
```
This serves the app at `http://localhost:4200` with hot reload.

## API Proxy (Alternative)

If CORS is problematic during development, you can configure a proxy:

Create `proxy.conf.json`:
```json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false,
    "changeOrigin": true
  }
}
```

Update `angular.json` serve options to include `"proxyConfig": "proxy.conf.json"`.

## Correlation IDs

Every API request includes an `X-Correlation-Id` header (added by the correlation interceptor). If you see an error:

1. Open Network tab
2. Find the failed request
3. Copy the `X-Correlation-Id` from the response headers
4. Give this to backend support — they can trace the full request in logs
