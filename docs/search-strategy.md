# Search Strategy

## Global Search

Clarity provides a unified search across multiple entity types from a single search box.

### How It Works

1. User types at least 2 characters in the search box
2. Frontend calls `GET /api/search?q={term}`
3. Backend searches across Clients, Matters, and Invoices
4. Results are grouped by type and capped at 5 per group
5. User clicks a result to navigate to the entity detail page

### Searched Fields

| Entity | Fields Searched |
|--------|----------------|
| Client | Name, ReferenceNumber, Email |
| Matter | Title, ReferenceNumber |
| Invoice | InvoiceNumber |

### Performance

- Uses `LIKE` pattern matching via EF Core `.Contains()`
- Indexes on Name, ReferenceNumber, Email, InvoiceNumber ensure fast lookups
- Results capped at 5 per entity type (15 total maximum)
- Soft-deleted records are excluded automatically via query filters

### Security

- Results are filtered by query filters (soft-delete)
- Future: Add permission-based filtering (users only see entities they have access to)

## List-Level Filters

Every major list screen supports server-side filtering:

| Screen | Available Filters |
|--------|------------------|
| Clients | searchTerm, status, clientType |
| Matters | searchTerm, status, matterType, clientId |
| Time Entries | matterId, userId, status, fromDate, toDate |
| Invoices | clientId, matterId, status |
| Audit Log | entityType, entityId, userId, action, correlationId, dateRange |

## Future Enhancements

- Full-text search using SQL Server Full-Text Index
- Elasticsearch integration for complex search scenarios
- Search history and suggestions
- Permission-aware search results
- Document content search (OCR + full-text)
