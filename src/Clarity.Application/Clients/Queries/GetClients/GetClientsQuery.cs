using Clarity.Application.Common.Models;
using Clarity.Application.Clients.Queries.GetClient;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Clients.Queries.GetClients;

public record GetClientsQuery : IRequest<Result<PaginatedList<ClientDto>>>
{
    public string? SearchTerm { get; init; }
    public ClientStatus? Status { get; init; }
    public ClientType? ClientType { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
