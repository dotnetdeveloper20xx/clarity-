using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Client : AuditableEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.Pending;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? CompanyNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<Matter> Matters { get; set; } = new List<Matter>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
