using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Search;

public record GlobalSearchQuery(string Term) : IRequest<Result<GlobalSearchResultDto>>;

public record GlobalSearchResultDto
{
    public List<SearchResultItem> Clients { get; init; } = new();
    public List<SearchResultItem> Matters { get; init; } = new();
    public List<SearchResultItem> Invoices { get; init; } = new();
}

public record SearchResultItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
}

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<GlobalSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public GlobalSearchQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Term) || request.Term.Length < 2)
            return Result<GlobalSearchResultDto>.Failure("Search term must be at least 2 characters.");

        var term = request.Term.ToLower();

        var clients = await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted && (c.Name.ToLower().Contains(term) || c.ReferenceNumber.ToLower().Contains(term) || (c.Email != null && c.Email.ToLower().Contains(term))))
            .Take(5)
            .Select(c => new SearchResultItem { Id = c.Id, Title = c.Name, Subtitle = c.ReferenceNumber, EntityType = "Client" })
            .ToListAsync(cancellationToken);

        var matters = await _context.Matters
            .AsNoTracking()
            .Where(m => !m.IsDeleted && (m.Title.ToLower().Contains(term) || m.ReferenceNumber.ToLower().Contains(term)))
            .Take(5)
            .Select(m => new SearchResultItem { Id = m.Id, Title = m.Title, Subtitle = m.ReferenceNumber, EntityType = "Matter" })
            .ToListAsync(cancellationToken);

        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => !i.IsDeleted && i.InvoiceNumber.ToLower().Contains(term))
            .Take(5)
            .Select(i => new SearchResultItem { Id = i.Id, Title = i.InvoiceNumber, Subtitle = $"£{i.TotalAmount}", EntityType = "Invoice" })
            .ToListAsync(cancellationToken);

        return Result<GlobalSearchResultDto>.Success(new GlobalSearchResultDto
        {
            Clients = clients,
            Matters = matters,
            Invoices = invoices
        });
    }
}
