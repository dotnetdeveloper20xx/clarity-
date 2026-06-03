using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Reporting.Queries.GetAuditLog;

public record GetAuditLogQuery : IRequest<Result<PaginatedList<AuditLogDto>>>
{
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }
    public Guid? UserId { get; init; }
    public string? Action { get; init; }
    public string? CorrelationId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record AuditLogDto
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? CorrelationId { get; init; }
}

public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, Result<PaginatedList<AuditLogDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<AuditLogDto>>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.EntityType))
            query = query.Where(a => a.EntityType == request.EntityType);
        if (request.EntityId.HasValue)
            query = query.Where(a => a.EntityId == request.EntityId.Value);
        if (request.UserId.HasValue)
            query = query.Where(a => a.UserId == request.UserId.Value);
        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action);
        if (!string.IsNullOrWhiteSpace(request.CorrelationId))
            query = query.Where(a => a.CorrelationId == request.CorrelationId);
        if (request.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= request.ToDate.Value);

        var projected = query.OrderByDescending(a => a.Timestamp).Select(a => new AuditLogDto
        {
            Id = a.Id,
            Timestamp = a.Timestamp,
            UserId = a.UserId,
            UserEmail = a.UserEmail,
            Action = a.Action,
            EntityType = a.EntityType,
            EntityId = a.EntityId,
            OldValues = a.OldValues,
            NewValues = a.NewValues,
            CorrelationId = a.CorrelationId
        });

        var result = await PaginatedList<AuditLogDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
        return Result<PaginatedList<AuditLogDto>>.Success(result);
    }
}
