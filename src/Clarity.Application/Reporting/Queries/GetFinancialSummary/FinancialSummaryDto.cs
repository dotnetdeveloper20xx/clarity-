namespace Clarity.Application.Reporting.Queries.GetFinancialSummary;

public record FinancialSummaryDto
{
    public decimal TotalBilledThisMonth { get; init; }
    public decimal TotalPaidThisMonth { get; init; }
    public decimal TotalOutstanding { get; init; }
    public decimal TotalWip { get; init; } // Work in progress (unbilled approved time)
    public List<AgedDebtBand> AgedDebt { get; init; } = new();
    public List<TopClientRevenue> TopClients { get; init; } = new();
}

public record AgedDebtBand
{
    public string Band { get; init; } = string.Empty; // "Current", "1-30 Days", etc.
    public decimal Amount { get; init; }
    public int InvoiceCount { get; init; }
}

public record TopClientRevenue
{
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
}
