using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }
    DbSet<Matter> Matters { get; }
    DbSet<MatterNote> MatterNotes { get; }
    DbSet<MatterTask> MatterTasks { get; }
    DbSet<MatterTeamMember> MatterTeamMembers { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<BillingRate> BillingRates { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceLineItem> InvoiceLineItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Document> Documents { get; }
    DbSet<ComplianceCheck> ComplianceChecks { get; }
    DbSet<AuditEntry> AuditEntries { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<ApplicationRole> Roles { get; }
    DbSet<ApplicationUserRole> UserRoles { get; }

    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
