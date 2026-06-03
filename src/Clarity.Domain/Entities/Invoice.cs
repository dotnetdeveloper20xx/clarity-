using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Invoice : AuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Guid MatterId { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateOnly? IssueDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; } = 20.00m;
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Client Client { get; set; } = null!;
    public Matter Matter { get; set; } = null!;
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
