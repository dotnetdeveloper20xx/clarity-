using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class InvoiceLineItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public Guid? TimeEntryId { get; set; }
    public LineItemType LineType { get; set; } = LineItemType.Time;
    public int SortOrder { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = null!;
}
