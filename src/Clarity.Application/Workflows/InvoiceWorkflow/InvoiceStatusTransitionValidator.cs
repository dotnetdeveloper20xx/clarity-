using Clarity.Domain.Enums;

namespace Clarity.Application.Workflows.InvoiceWorkflow;

public static class InvoiceStatusTransitionValidator
{
    private static readonly Dictionary<InvoiceStatus, InvoiceStatus[]> AllowedTransitions = new()
    {
        [InvoiceStatus.Draft] = [InvoiceStatus.Issued, InvoiceStatus.Cancelled],
        [InvoiceStatus.Issued] = [InvoiceStatus.PartiallyPaid, InvoiceStatus.Paid, InvoiceStatus.Cancelled, InvoiceStatus.WrittenOff],
        [InvoiceStatus.PartiallyPaid] = [InvoiceStatus.Paid, InvoiceStatus.WrittenOff],
        [InvoiceStatus.Paid] = [], // Terminal
        [InvoiceStatus.Cancelled] = [], // Terminal
        [InvoiceStatus.WrittenOff] = [] // Terminal
    };

    public static bool IsValidTransition(InvoiceStatus from, InvoiceStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
