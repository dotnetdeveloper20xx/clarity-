namespace Clarity.Application.Reporting.Queries.GetDashboard;

public record DashboardDto
{
    public int OpenMattersCount { get; init; }
    public int OverdueTasksCount { get; init; }
    public int PendingComplianceCount { get; init; }
    public int UnreadNotificationsCount { get; init; }
    public decimal UnbilledTimeValue { get; init; }
    public decimal OutstandingInvoicesTotal { get; init; }
    public decimal PaidThisMonthTotal { get; init; }
    public int ComplianceAlertsCount { get; init; }
    public int DraftTimeEntriesCount { get; init; }
    public int PendingApprovalsCount { get; init; }
}
