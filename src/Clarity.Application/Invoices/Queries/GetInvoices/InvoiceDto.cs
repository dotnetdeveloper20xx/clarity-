using Clarity.Domain.Enums;

namespace Clarity.Application.Invoices.Queries.GetInvoices;

public record InvoiceDto
{
    public Guid Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public Guid MatterId { get; init; }
    public string MatterReference { get; init; } = string.Empty;
    public InvoiceStatus Status { get; init; }
    public DateOnly? IssueDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount => TotalAmount - PaidAmount;
}
