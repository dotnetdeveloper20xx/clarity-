using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public ClientType ClientType { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? County { get; init; }
    public string? PostCode { get; init; }
    public string? Country { get; init; }
    public string? CompanyNumber { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Notes { get; init; }
}
