using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid Id) : IRequest<Result>;
