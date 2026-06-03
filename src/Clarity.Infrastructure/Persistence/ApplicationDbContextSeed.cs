using System.Security.Cryptography;
using System.Text;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return; // Already seeded

        // Roles
        var adminRole = new ApplicationRole { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", Description = "Full system access", IsSystemRole = true };
        var consultantRole = new ApplicationRole { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Consultant", Description = "Legal consultant / solicitor", IsSystemRole = true };
        var assistantRole = new ApplicationRole { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "LegalAssistant", Description = "Legal assistant", IsSystemRole = true };
        var teamLeadRole = new ApplicationRole { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "TeamLeader", Description = "Team leader", IsSystemRole = true };
        var financeRole = new ApplicationRole { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Finance", Description = "Finance team", IsSystemRole = true };
        var complianceRole = new ApplicationRole { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Compliance", Description = "Compliance officer", IsSystemRole = true };
        var supportRole = new ApplicationRole { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Support", Description = "Support user", IsSystemRole = true };
        var clientRole = new ApplicationRole { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Client", Description = "External client", IsSystemRole = true };

        context.Roles.AddRange(adminRole, consultantRole, assistantRole, teamLeadRole, financeRole, complianceRole, supportRole, clientRole);

        // Users
        var adminUser = new ApplicationUser
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Email = "admin@clarity.local",
            PasswordHash = HashPassword("Admin123!"),
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true
        };

        var consultant1 = new ApplicationUser
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Email = "sarah.johnson@clarity.local",
            PasswordHash = HashPassword("Password1!"),
            FirstName = "Sarah",
            LastName = "Johnson",
            IsActive = true
        };

        var consultant2 = new ApplicationUser
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Email = "james.wilson@clarity.local",
            PasswordHash = HashPassword("Password1!"),
            FirstName = "James",
            LastName = "Wilson",
            IsActive = true
        };

        var financeUser = new ApplicationUser
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            Email = "finance@clarity.local",
            PasswordHash = HashPassword("Password1!"),
            FirstName = "Emma",
            LastName = "Thompson",
            IsActive = true
        };

        var complianceUser = new ApplicationUser
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            Email = "compliance@clarity.local",
            PasswordHash = HashPassword("Password1!"),
            FirstName = "David",
            LastName = "Brown",
            IsActive = true
        };

        context.Users.AddRange(adminUser, consultant1, consultant2, financeUser, complianceUser);

        // User Roles
        context.UserRoles.AddRange(
            new ApplicationUserRole { UserId = adminUser.Id, RoleId = adminRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = consultant1.Id, RoleId = consultantRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = consultant1.Id, RoleId = teamLeadRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = consultant2.Id, RoleId = consultantRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = financeUser.Id, RoleId = financeRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = complianceUser.Id, RoleId = complianceRole.Id, AssignedBy = adminUser.Id }
        );

        // Billing Rates
        var seniorRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000001"), Name = "Senior Solicitor", HourlyRate = 350m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };
        var midRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000002"), Name = "Solicitor", HourlyRate = 250m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };
        var juniorRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000003"), Name = "Trainee", HourlyRate = 150m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };

        context.BillingRates.AddRange(seniorRate, midRate, juniorRate);

        // Clients
        var clients = new List<Client>();
        for (int i = 1; i <= 10; i++)
        {
            clients.Add(new Client
            {
                ReferenceNumber = $"CLI-{i:D5}",
                Name = $"Client {i} Ltd",
                ClientType = i % 3 == 0 ? ClientType.Individual : ClientType.Organisation,
                Status = ClientStatus.Active,
                Email = $"contact@client{i}.com",
                Phone = $"0207 123 {i:D4}",
                AddressLine1 = $"{i} High Street",
                City = "London",
                PostCode = $"EC{i}A 1BB",
                Country = "United Kingdom",
                CreatedBy = adminUser.Id
            });
        }
        context.Clients.AddRange(clients);

        // Matters
        var matters = new List<Matter>();
        var matterTypes = Enum.GetValues<MatterType>();
        for (int i = 1; i <= 20; i++)
        {
            var client = clients[(i - 1) % clients.Count];
            matters.Add(new Matter
            {
                ReferenceNumber = $"MAT-{i:D5}",
                ClientId = client.Id,
                Title = $"Matter {i} - {matterTypes[i % matterTypes.Length]}",
                Description = $"Legal matter {i} for {client.Name}",
                MatterType = matterTypes[i % matterTypes.Length],
                Status = i <= 15 ? MatterStatus.InProgress : MatterStatus.Closed,
                FeeArrangement = i % 2 == 0 ? FeeArrangement.Hourly : FeeArrangement.FixedFee,
                EstimatedValue = 5000m + (i * 1000m),
                OpenedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90 + i)),
                ClosedDate = i > 15 ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)) : null,
                LeadConsultantId = i % 2 == 0 ? consultant1.Id : consultant2.Id,
                CreatedBy = adminUser.Id
            });
        }
        context.Matters.AddRange(matters);

        // Time entries
        var timeEntries = new List<TimeEntry>();
        for (int i = 1; i <= 50; i++)
        {
            var matter = matters[(i - 1) % matters.Count];
            var user = i % 2 == 0 ? consultant1 : consultant2;
            timeEntries.Add(new TimeEntry
            {
                MatterId = matter.Id,
                UserId = user.Id,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)),
                DurationMinutes = 30 + (i * 10 % 180),
                Description = $"Work on matter - activity {i}",
                IsBillable = i % 5 != 0,
                BillingRateId = seniorRate.Id,
                RateAmount = seniorRate.HourlyRate,
                Status = i <= 30 ? TimeEntryStatus.Approved : TimeEntryStatus.Draft,
                ApprovedById = i <= 30 ? consultant1.Id : null,
                ApprovedAt = i <= 30 ? DateTime.UtcNow.AddDays(-i + 1) : null,
                CreatedBy = user.Id
            });
        }
        context.TimeEntries.AddRange(timeEntries);

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
