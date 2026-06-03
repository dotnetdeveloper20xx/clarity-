using Clarity.Domain.Common;

namespace Clarity.Domain.Entities;

public class BillingRate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}
