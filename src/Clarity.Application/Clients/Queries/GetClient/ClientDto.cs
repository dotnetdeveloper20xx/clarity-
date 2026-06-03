using Clarity.Domain.Enums;

namespace Clarity.Application.Clients.Queries.GetClient;

public record ClientDto
{
    public Guid Id { get; init; }
    public string ReferenceNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ClientType ClientType { get; init; }
    public ClientStatus Status { get; init; }
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
    public DateTime CreatedAt { get; init; }
}
