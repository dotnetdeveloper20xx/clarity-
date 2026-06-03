using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Clients.Queries.GetClient;

public record GetClientQuery(Guid Id) : IRequest<Result<ClientDto>>;
