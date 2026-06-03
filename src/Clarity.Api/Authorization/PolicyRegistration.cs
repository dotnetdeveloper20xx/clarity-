using Microsoft.AspNetCore.Authorization;

namespace Clarity.Api.Authorization;

public static class PolicyRegistration
{
    public static AuthorizationOptions AddClarityPolicies(this AuthorizationOptions options)
    {
        // Client
        options.AddPolicy(Policies.CanViewClients, p => p.RequireAuthenticatedUser());
        options.AddPolicy(Policies.CanManageClients, p => p.RequireRole("Admin", "Consultant", "TeamLeader", "LegalAssistant"));

        // Matter
        options.AddPolicy(Policies.CanViewMatters, p => p.RequireAuthenticatedUser());
        options.AddPolicy(Policies.CanManageMatters, p => p.RequireRole("Admin", "Consultant", "TeamLeader", "LegalAssistant"));
        options.AddPolicy(Policies.CanCloseMatters, p => p.RequireRole("Admin", "TeamLeader"));

        // Billing
        options.AddPolicy(Policies.CanViewBilling, p => p.RequireRole("Admin", "Finance", "TeamLeader"));
        options.AddPolicy(Policies.CanManageBilling, p => p.RequireRole("Admin", "Finance"));
        options.AddPolicy(Policies.CanIssueInvoices, p => p.RequireRole("Admin", "Finance"));
        options.AddPolicy(Policies.CanRecordPayments, p => p.RequireRole("Admin", "Finance"));

        // Compliance
        options.AddPolicy(Policies.CanViewCompliance, p => p.RequireRole("Admin", "Compliance"));
        options.AddPolicy(Policies.CanManageCompliance, p => p.RequireRole("Admin", "Compliance"));

        // Admin
        options.AddPolicy(Policies.CanManageUsers, p => p.RequireRole("Admin"));
        options.AddPolicy(Policies.CanViewDiagnostics, p => p.RequireRole("Admin", "Support"));
        options.AddPolicy(Policies.CanViewAudit, p => p.RequireRole("Admin", "Compliance", "Support"));

        return options;
    }
}
