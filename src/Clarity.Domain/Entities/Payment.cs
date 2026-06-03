using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public bool IsReversed { get; set; }
    public Guid? ReversedById { get; set; }
    public DateTime? ReversedAt { get; set; }
    public string? ReversalReason { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = null!;
}
