namespace Clarity.Api.Authorization;

/// <summary>
/// Defines named authorization policy constants used across the API.
/// </summary>
public static class Policies
{
    // Client policies
    public const string CanViewClients = "CanViewClients";
    public const string CanManageClients = "CanManageClients";

    // Matter policies
    public const string CanViewMatters = "CanViewMatters";
    public const string CanManageMatters = "CanManageMatters";
    public const string CanCloseMatters = "CanCloseMatters";

    // Billing policies
    public const string CanViewBilling = "CanViewBilling";
    public const string CanManageBilling = "CanManageBilling";
    public const string CanIssueInvoices = "CanIssueInvoices";
    public const string CanRecordPayments = "CanRecordPayments";

    // Compliance policies
    public const string CanViewCompliance = "CanViewCompliance";
    public const string CanManageCompliance = "CanManageCompliance";

    // Admin policies
    public const string CanManageUsers = "CanManageUsers";
    public const string CanViewDiagnostics = "CanViewDiagnostics";
    public const string CanViewAudit = "CanViewAudit";
}
