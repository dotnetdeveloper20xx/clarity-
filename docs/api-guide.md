# API Guide

## Overview

The Clarity API is a RESTful web API built with ASP.NET Core. It serves as the backend for the Angular frontend and exposes endpoints for managing clients, matters, time entries, invoices, payments, documents, and compliance checks.

## Base URL

- Local Development: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Authentication

All endpoints (except `/api/auth/login`) require a Bearer JWT token in the Authorization header:

```
Authorization: Bearer <token>
```

### Login

```
POST /api/auth/login
{
  "email": "admin@clarity.local",
  "password": "Admin123!"
}
```

Returns a JWT token, user email, full name, and roles.

## API Endpoints

### Clients

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/clients | List clients (paginated, filterable) |
| GET | /api/clients/{id} | Get client by ID |
| POST | /api/clients | Create a new client |
| PUT | /api/clients/{id} | Update a client |
| DELETE | /api/clients/{id} | Soft-delete a client |

### Matters

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/matters | List matters (paginated, filterable) |
| GET | /api/matters/{id} | Get matter by ID |
| POST | /api/matters | Create a new matter |
| PUT | /api/matters/{id}/status | Update matter status |

### Time Entries

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/timeentries | List time entries (filterable) |
| POST | /api/timeentries | Record a time entry |
| PUT | /api/timeentries/{id}/approve | Approve a time entry |

### Invoices

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/invoices | List invoices (filterable) |
| POST | /api/invoices | Create invoice from approved time |
| PUT | /api/invoices/{id}/issue | Issue a draft invoice |

### Payments

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/payments | Record a payment |

### Documents

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/documents | Upload a document (multipart form) |
| GET | /api/documents/{id}/download | Download a document |

### Compliance

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/compliance/checks | Create a compliance check |

### Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /health | API and database health check |

## Query Parameters

Most list endpoints support:

- `pageNumber` (default: 1)
- `pageSize` (default: 20)
- `searchTerm` — text search
- `status` — filter by status enum value

## Error Responses

All errors return a consistent JSON structure:

```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "correlationId": "abc123def456",
  "errors": {
    "Name": ["Client name is required."]
  }
}
```

## Correlation ID

Every request includes a `X-Correlation-Id` header in the response. Use this ID when contacting support to trace issues through logs.

## Seed Data Accounts

| Email | Password | Role |
|-------|----------|------|
| admin@clarity.local | Admin123! | Admin |
| sarah.johnson@clarity.local | Password1! | Consultant, TeamLeader |
| james.wilson@clarity.local | Password1! | Consultant |
| finance@clarity.local | Password1! | Finance |
| compliance@clarity.local | Password1! | Compliance |
