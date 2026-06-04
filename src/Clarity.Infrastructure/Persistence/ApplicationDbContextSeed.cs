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

        // ============================================================
        // ROLES
        // ============================================================
        var adminRole = new ApplicationRole { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", Description = "Full system access", IsSystemRole = true };
        var consultantRole = new ApplicationRole { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Consultant", Description = "Legal consultant / solicitor", IsSystemRole = true };
        var assistantRole = new ApplicationRole { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "LegalAssistant", Description = "Legal assistant", IsSystemRole = true };
        var teamLeadRole = new ApplicationRole { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "TeamLeader", Description = "Team leader", IsSystemRole = true };
        var financeRole = new ApplicationRole { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Finance", Description = "Finance team", IsSystemRole = true };
        var complianceRole = new ApplicationRole { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Compliance", Description = "Compliance officer", IsSystemRole = true };
        var supportRole = new ApplicationRole { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Support", Description = "Support user", IsSystemRole = true };
        var clientRole = new ApplicationRole { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Client", Description = "External client", IsSystemRole = true };

        context.Roles.AddRange(adminRole, consultantRole, assistantRole, teamLeadRole, financeRole, complianceRole, supportRole, clientRole);

        // ============================================================
        // USERS
        // ============================================================
        var adminUser = new ApplicationUser { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Email = "admin@clarity.local", PasswordHash = HashPassword("Admin123!"), FirstName = "System", LastName = "Administrator", IsActive = true };
        var sarah = new ApplicationUser { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Email = "sarah.johnson@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "Sarah", LastName = "Johnson", IsActive = true };
        var james = new ApplicationUser { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Email = "james.wilson@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "James", LastName = "Wilson", IsActive = true };
        var emma = new ApplicationUser { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Email = "finance@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "Emma", LastName = "Thompson", IsActive = true };
        var david = new ApplicationUser { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Email = "compliance@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "David", LastName = "Brown", IsActive = true };
        var lisa = new ApplicationUser { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Email = "lisa.martinez@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "Lisa", LastName = "Martinez", IsActive = true };
        var michael = new ApplicationUser { Id = Guid.Parse("11111111-aaaa-bbbb-cccc-dddddddddddd"), Email = "michael.chen@clarity.local", PasswordHash = HashPassword("Password1!"), FirstName = "Michael", LastName = "Chen", IsActive = true };

        context.Users.AddRange(adminUser, sarah, james, emma, david, lisa, michael);

        context.UserRoles.AddRange(
            new ApplicationUserRole { UserId = adminUser.Id, RoleId = adminRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = sarah.Id, RoleId = consultantRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = sarah.Id, RoleId = teamLeadRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = james.Id, RoleId = consultantRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = emma.Id, RoleId = financeRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = david.Id, RoleId = complianceRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = lisa.Id, RoleId = assistantRole.Id, AssignedBy = adminUser.Id },
            new ApplicationUserRole { UserId = michael.Id, RoleId = consultantRole.Id, AssignedBy = adminUser.Id }
        );

        // ============================================================
        // BILLING RATES
        // ============================================================
        var seniorRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000001"), Name = "Senior Solicitor", HourlyRate = 350m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };
        var midRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000002"), Name = "Solicitor", HourlyRate = 250m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };
        var juniorRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000003"), Name = "Trainee", HourlyRate = 150m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };
        var partnerRate = new BillingRate { Id = Guid.Parse("11110000-0000-0000-0000-000000000004"), Name = "Partner", HourlyRate = 500m, EffectiveFrom = new DateOnly(2024, 1, 1), IsActive = true, CreatedBy = adminUser.Id };

        context.BillingRates.AddRange(seniorRate, midRate, juniorRate, partnerRate);

        // ============================================================
        // CLIENTS — realistic names with varied statuses
        // ============================================================
        var clientData = new[]
        {
            ("Harrington & Partners LLP", ClientType.Organisation, ClientStatus.Active, "info@harringtonllp.co.uk", "0207 456 7890", "45 Chancery Lane", "London", "WC2A 1JE"),
            ("Oakwood Developments Ltd", ClientType.Organisation, ClientStatus.Active, "legal@oakwooddev.com", "0161 234 5678", "12 Deansgate", "Manchester", "M3 2BY"),
            ("Margaret Thornton", ClientType.Individual, ClientStatus.Active, "m.thornton@email.com", "07700 900123", "8 Rose Cottage", "Bristol", "BS1 4DJ"),
            ("TechVentures Group PLC", ClientType.Organisation, ClientStatus.Active, "corporate@techventures.io", "0203 987 6543", "Level 30, The Shard", "London", "SE1 9SG"),
            ("Robert & Catherine Williams", ClientType.Individual, ClientStatus.Active, "rwilliams@gmail.com", "07700 900456", "22 Elm Drive", "Oxford", "OX1 2JD"),
            ("Meridian Construction Co", ClientType.Organisation, ClientStatus.OnHold, "accounts@meridian-build.co.uk", "0121 456 7890", "Unit 5, Industrial Park", "Birmingham", "B1 2RA"),
            ("Dr Aisha Patel", ClientType.Individual, ClientStatus.Active, "a.patel@nhs.net", "07700 900789", "14 Harley Street", "London", "W1G 9PF"),
            ("Sterling Financial Services", ClientType.Organisation, ClientStatus.Active, "compliance@sterlingfs.com", "0207 789 0123", "One Canada Square", "London", "E14 5AB"),
            ("Greenfield Estates", ClientType.Organisation, ClientStatus.Pending, "hello@greenfieldestates.co.uk", "01onal 234 567", "The Old Mill", "Cambridge", "CB2 1TN"),
            ("Thompson Family Trust", ClientType.Organisation, ClientStatus.Active, "trustees@thompsontrust.org", "0207 345 6789", "3 Bedford Row", "London", "WC1R 4BU"),
            ("Jasper Moore", ClientType.Individual, ClientStatus.Active, "jasper.moore@outlook.com", "07700 901234", "55 Park Avenue", "Leeds", "LS1 3HE"),
            ("Nexus Pharmaceuticals", ClientType.Organisation, ClientStatus.Active, "legal@nexuspharma.com", "01onal 987 654", "Science Park", "Edinburgh", "EH16 4UX"),
            ("Sandra & Marcus Chen", ClientType.Individual, ClientStatus.Archived, "chen.family@email.com", "07700 902345", "9 Cherry Lane", "Surrey", "GU1 3RW"),
            ("Atlas Logistics International", ClientType.Organisation, ClientStatus.Active, "uk.legal@atlaslogistics.com", "0203 111 2222", "Harbour Exchange", "London", "E14 9GE"),
            ("David Blackwell QC", ClientType.Individual, ClientStatus.Active, "d.blackwell@chambers.co.uk", "0207 222 3333", "4 Paper Buildings", "London", "EC4Y 7EX"),
        };

        var clients = new List<Client>();
        for (int i = 0; i < clientData.Length; i++)
        {
            var (name, type, status, email, phone, addr, city, postCode) = clientData[i];
            clients.Add(new Client
            {
                ReferenceNumber = $"CLI-{(i + 1):D5}",
                Name = name, ClientType = type, Status = status,
                Email = email, Phone = phone, AddressLine1 = addr, City = city, PostCode = postCode,
                Country = "United Kingdom",
                CompanyNumber = type == ClientType.Organisation ? $"{10000000 + i}" : null,
                CreatedBy = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-180 + (i * 10))
            });
        }
        context.Clients.AddRange(clients);

        // ============================================================
        // MATTERS — varied statuses, types, realistic titles
        // ============================================================
        var matterData = new[]
        {
            (0, "Residential Purchase - 45 Oak Lane", MatterType.Conveyancing, MatterStatus.InProgress, FeeArrangement.FixedFee, 3500m, sarah.Id, -60),
            (0, "Commercial Lease Review", MatterType.Commercial, MatterStatus.Open, FeeArrangement.Hourly, 15000m, james.Id, -5),
            (1, "Planning Permission Appeal", MatterType.Litigation, MatterStatus.AwaitingThirdParty, FeeArrangement.Hourly, 25000m, sarah.Id, -45),
            (1, "Construction Dispute - Phase 2", MatterType.Litigation, MatterStatus.InProgress, FeeArrangement.Hourly, 80000m, michael.Id, -120),
            (2, "Will Preparation & Estate Planning", MatterType.WillsAndProbate, MatterStatus.Closed, FeeArrangement.FixedFee, 2500m, james.Id, -90),
            (3, "Series B Funding Agreement", MatterType.Commercial, MatterStatus.InProgress, FeeArrangement.Hourly, 45000m, sarah.Id, -30),
            (3, "Employment Contract Templates", MatterType.Employment, MatterStatus.Closed, FeeArrangement.FixedFee, 8000m, james.Id, -150),
            (4, "Divorce Proceedings", MatterType.FamilyLaw, MatterStatus.AwaitingClient, FeeArrangement.Hourly, 12000m, michael.Id, -75),
            (4, "Property Transfer - Joint Ownership", MatterType.Conveyancing, MatterStatus.BillingReview, FeeArrangement.FixedFee, 4000m, sarah.Id, -40),
            (5, "HSE Investigation Defence", MatterType.Litigation, MatterStatus.OnHold, FeeArrangement.Hourly, 50000m, james.Id, -100),
            (6, "Medical Negligence Claim", MatterType.PersonalInjury, MatterStatus.InProgress, FeeArrangement.Hourly, 35000m, michael.Id, -55),
            (7, "FCA Regulatory Compliance Review", MatterType.Commercial, MatterStatus.InProgress, FeeArrangement.Hourly, 60000m, sarah.Id, -25),
            (7, "Client Money Audit Preparation", MatterType.Commercial, MatterStatus.Closed, FeeArrangement.FixedFee, 12000m, james.Id, -200),
            (8, "Land Registry Application", MatterType.Conveyancing, MatterStatus.Open, FeeArrangement.FixedFee, 2000m, sarah.Id, -3),
            (9, "Trust Deed Amendment", MatterType.WillsAndProbate, MatterStatus.InProgress, FeeArrangement.Hourly, 8000m, james.Id, -20),
            (10, "Employment Tribunal - Unfair Dismissal", MatterType.Employment, MatterStatus.InProgress, FeeArrangement.Hourly, 18000m, michael.Id, -65),
            (11, "Patent Infringement Defence", MatterType.Litigation, MatterStatus.InProgress, FeeArrangement.Hourly, 120000m, sarah.Id, -80),
            (11, "Clinical Trial Agreement", MatterType.Commercial, MatterStatus.Closed, FeeArrangement.FixedFee, 15000m, james.Id, -180),
            (12, "Probate Application - Chen Estate", MatterType.WillsAndProbate, MatterStatus.Archived, FeeArrangement.FixedFee, 5000m, james.Id, -300),
            (13, "Customs & Import Licensing", MatterType.Commercial, MatterStatus.InProgress, FeeArrangement.Hourly, 22000m, michael.Id, -35),
            (14, "Chambers Lease Renewal", MatterType.Conveyancing, MatterStatus.Closed, FeeArrangement.FixedFee, 3000m, sarah.Id, -160),
            (3, "IP Assignment Agreement", MatterType.Commercial, MatterStatus.Open, FeeArrangement.Hourly, 10000m, michael.Id, -2),
            (0, "Remortgage - 45 Oak Lane", MatterType.Conveyancing, MatterStatus.AwaitingClient, FeeArrangement.FixedFee, 1800m, james.Id, -15),
            (7, "Director Disqualification Defence", MatterType.Litigation, MatterStatus.InProgress, FeeArrangement.Hourly, 75000m, sarah.Id, -50),
            (4, "Child Custody Arrangement", MatterType.FamilyLaw, MatterStatus.AwaitingThirdParty, FeeArrangement.Hourly, 9000m, michael.Id, -28),
        };

        var matters = new List<Matter>();
        for (int i = 0; i < matterData.Length; i++)
        {
            var (clientIdx, title, type, status, fee, value, leadId, daysAgo) = matterData[i];
            var m = new Matter
            {
                ReferenceNumber = $"MAT-{(i + 1):D5}",
                ClientId = clients[clientIdx].Id,
                Title = title,
                Description = $"Legal matter for {clients[clientIdx].Name} regarding {title.ToLower()}.",
                MatterType = type, Status = status, FeeArrangement = fee,
                EstimatedValue = value,
                OpenedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysAgo)),
                ClosedDate = status == MatterStatus.Closed || status == MatterStatus.Archived ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)) : null,
                LeadConsultantId = leadId,
                CreatedBy = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(daysAgo)
            };
            matters.Add(m);
        }
        context.Matters.AddRange(matters);

        // ============================================================
        // MATTER NOTES
        // ============================================================
        var notes = new List<MatterNote>
        {
            new() { MatterId = matters[0].Id, Content = "Client confirmed budget of £3,500 for conveyancing. Searches ordered.", IsClientVisible = false, CreatedBy = sarah.Id, CreatedAt = DateTime.UtcNow.AddDays(-55) },
            new() { MatterId = matters[0].Id, Content = "Local authority search results received. No issues identified.", IsClientVisible = true, CreatedBy = lisa.Id, CreatedAt = DateTime.UtcNow.AddDays(-40) },
            new() { MatterId = matters[3].Id, Content = "Expert witness report received. Favourable to our client's position.", IsClientVisible = false, CreatedBy = michael.Id, CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { MatterId = matters[5].Id, Content = "Term sheet reviewed. Recommending amendments to clause 4.2 (anti-dilution).", IsClientVisible = false, CreatedBy = sarah.Id, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { MatterId = matters[5].Id, Content = "Investor counsel has agreed to all proposed amendments. Proceeding to final draft.", IsClientVisible = true, CreatedBy = sarah.Id, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { MatterId = matters[10].Id, Content = "Medical records obtained from NHS Trust. Reviewing for breach of duty evidence.", IsClientVisible = false, CreatedBy = michael.Id, CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new() { MatterId = matters[11].Id, Content = "FCA deadline for response is 15th of next month. All materials being compiled.", IsClientVisible = true, CreatedBy = sarah.Id, CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new() { MatterId = matters[15].Id, Content = "Tribunal hearing date set for 3 weeks. Preparing witness statements.", IsClientVisible = true, CreatedBy = michael.Id, CreatedAt = DateTime.UtcNow.AddDays(-8) },
        };
        context.MatterNotes.AddRange(notes);

        // ============================================================
        // MATTER TASKS
        // ============================================================
        var tasks = new List<MatterTask>
        {
            new() { MatterId = matters[0].Id, Title = "Order environmental search", AssigneeId = lisa.Id, Status = TaskItemStatus.Complete, Priority = TaskPriority.Medium, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)), CompletedAt = DateTime.UtcNow.AddDays(-48), CreatedBy = sarah.Id },
            new() { MatterId = matters[0].Id, Title = "Review title deeds", AssigneeId = sarah.Id, Status = TaskItemStatus.InProgress, Priority = TaskPriority.High, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), CreatedBy = sarah.Id },
            new() { MatterId = matters[0].Id, Title = "Send completion statement to client", AssigneeId = lisa.Id, Status = TaskItemStatus.ToDo, Priority = TaskPriority.Medium, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)), CreatedBy = sarah.Id },
            new() { MatterId = matters[3].Id, Title = "File defence statement", AssigneeId = michael.Id, Status = TaskItemStatus.Complete, Priority = TaskPriority.Urgent, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)), CompletedAt = DateTime.UtcNow.AddDays(-22), CreatedBy = james.Id },
            new() { MatterId = matters[3].Id, Title = "Prepare disclosure bundle", AssigneeId = lisa.Id, Status = TaskItemStatus.InProgress, Priority = TaskPriority.High, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), CreatedBy = michael.Id },
            new() { MatterId = matters[5].Id, Title = "Draft shareholders agreement", AssigneeId = sarah.Id, Status = TaskItemStatus.InProgress, Priority = TaskPriority.High, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), CreatedBy = sarah.Id },
            new() { MatterId = matters[7].Id, Title = "Chase client for financial disclosure", AssigneeId = lisa.Id, Status = TaskItemStatus.Blocked, Priority = TaskPriority.High, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), CreatedBy = michael.Id },
            new() { MatterId = matters[10].Id, Title = "Instruct medical expert", AssigneeId = michael.Id, Status = TaskItemStatus.Complete, Priority = TaskPriority.High, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-40)), CompletedAt = DateTime.UtcNow.AddDays(-38), CreatedBy = michael.Id },
            new() { MatterId = matters[11].Id, Title = "Compile FCA response documentation", AssigneeId = sarah.Id, Status = TaskItemStatus.InProgress, Priority = TaskPriority.Urgent, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), CreatedBy = sarah.Id },
            new() { MatterId = matters[15].Id, Title = "Prepare witness statements", AssigneeId = michael.Id, Status = TaskItemStatus.InProgress, Priority = TaskPriority.Urgent, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), CreatedBy = michael.Id },
            new() { MatterId = matters[16].Id, Title = "Review prior art search results", AssigneeId = james.Id, Status = TaskItemStatus.ToDo, Priority = TaskPriority.Medium, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)), CreatedBy = sarah.Id },
            new() { MatterId = matters[19].Id, Title = "Draft customs licensing application", AssigneeId = michael.Id, Status = TaskItemStatus.ToDo, Priority = TaskPriority.Medium, DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(8)), CreatedBy = michael.Id },
        };
        context.MatterTasks.AddRange(tasks);

        // ============================================================
        // TIME ENTRIES — varied statuses, descriptions, users
        // ============================================================
        var timeDescriptions = new[]
        {
            "Reviewed contract and drafted amendments",
            "Client telephone conference",
            "Prepared correspondence to opposing counsel",
            "Legal research - case law review",
            "Drafted witness statement",
            "Attended court hearing",
            "Reviewed disclosure documents",
            "Prepared bundle for counsel",
            "Conference with barrister",
            "Drafted letter of claim",
            "Reviewed medical evidence",
            "Prepared skeleton argument",
            "Client meeting - case update",
            "Reviewed title documents",
            "Drafted heads of terms",
            "Prepared completion statement",
            "Reviewed lease agreement",
            "Filed application at court",
            "Attendance at mediation",
            "Prepared costs schedule"
        };

        var timeEntries = new List<TimeEntry>();
        var rng = new Random(42); // Deterministic for consistent seed
        for (int i = 0; i < 120; i++)
        {
            var matterIdx = i % matters.Count;
            if (matters[matterIdx].Status == MatterStatus.Archived) matterIdx = 0; // Skip archived
            var users = new[] { sarah, james, michael };
            var user = users[i % 3];
            var rate = i % 3 == 0 ? seniorRate : (i % 3 == 1 ? midRate : midRate);
            var daysAgo = rng.Next(1, 90);
            var duration = new[] { 15, 30, 45, 60, 90, 120, 150, 180 }[i % 8];

            // Vary statuses realistically
            TimeEntryStatus status;
            if (i < 40) status = TimeEntryStatus.Approved;
            else if (i < 55) status = TimeEntryStatus.Billed;
            else if (i < 75) status = TimeEntryStatus.Draft;
            else if (i < 90) status = TimeEntryStatus.Submitted;
            else if (i < 100) status = TimeEntryStatus.Approved;
            else status = TimeEntryStatus.Draft;

            timeEntries.Add(new TimeEntry
            {
                MatterId = matters[matterIdx].Id,
                UserId = user.Id,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-daysAgo)),
                DurationMinutes = duration,
                Description = timeDescriptions[i % timeDescriptions.Length],
                IsBillable = i % 6 != 0,
                BillingRateId = rate.Id,
                RateAmount = rate.HourlyRate,
                Status = status,
                ApprovedById = status == TimeEntryStatus.Approved || status == TimeEntryStatus.Billed ? sarah.Id : null,
                ApprovedAt = status == TimeEntryStatus.Approved || status == TimeEntryStatus.Billed ? DateTime.UtcNow.AddDays(-daysAgo + 1) : null,
                CreatedBy = user.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-daysAgo)
            });
        }
        context.TimeEntries.AddRange(timeEntries);

        // ============================================================
        // INVOICES — some draft, issued, paid, partially paid
        // ============================================================
        var invoices = new List<Invoice>();

        // Invoice 1 - Paid
        var inv1 = new Invoice { InvoiceNumber = "INV-00001", ClientId = clients[2].Id, MatterId = matters[4].Id, Status = InvoiceStatus.Paid, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-60)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)), SubTotal = 2500m, TaxRate = 20m, TaxAmount = 500m, TotalAmount = 3000m, PaidAmount = 3000m, Notes = "Will preparation - fixed fee", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-60) };
        invoices.Add(inv1);

        // Invoice 2 - Issued (overdue)
        var inv2 = new Invoice { InvoiceNumber = "INV-00002", ClientId = clients[3].Id, MatterId = matters[5].Id, Status = InvoiceStatus.Issued, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-45)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)), SubTotal = 8750m, TaxRate = 20m, TaxAmount = 1750m, TotalAmount = 10500m, PaidAmount = 0m, Notes = "Series B - interim billing", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-45) };
        invoices.Add(inv2);

        // Invoice 3 - Partially paid
        var inv3 = new Invoice { InvoiceNumber = "INV-00003", ClientId = clients[1].Id, MatterId = matters[3].Id, Status = InvoiceStatus.PartiallyPaid, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow), SubTotal = 12000m, TaxRate = 20m, TaxAmount = 2400m, TotalAmount = 14400m, PaidAmount = 5000m, Notes = "Construction dispute - phase 1 billing", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-30) };
        invoices.Add(inv3);

        // Invoice 4 - Draft
        var inv4 = new Invoice { InvoiceNumber = "INV-00004", ClientId = clients[7].Id, MatterId = matters[11].Id, Status = InvoiceStatus.Draft, SubTotal = 15000m, TaxRate = 20m, TaxAmount = 3000m, TotalAmount = 18000m, PaidAmount = 0m, Notes = "FCA compliance review - draft for review", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-3) };
        invoices.Add(inv4);

        // Invoice 5 - Paid
        var inv5 = new Invoice { InvoiceNumber = "INV-00005", ClientId = clients[3].Id, MatterId = matters[6].Id, Status = InvoiceStatus.Paid, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-120)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90)), SubTotal = 8000m, TaxRate = 20m, TaxAmount = 1600m, TotalAmount = 9600m, PaidAmount = 9600m, Notes = "Employment contracts - fixed fee", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-120) };
        invoices.Add(inv5);

        // Invoice 6 - Issued (not yet due)
        var inv6 = new Invoice { InvoiceNumber = "INV-00006", ClientId = clients[10].Id, MatterId = matters[15].Id, Status = InvoiceStatus.Issued, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(25)), SubTotal = 4500m, TaxRate = 20m, TaxAmount = 900m, TotalAmount = 5400m, PaidAmount = 0m, Notes = "Employment tribunal - interim", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-5) };
        invoices.Add(inv6);

        // Invoice 7 - Written Off
        var inv7 = new Invoice { InvoiceNumber = "INV-00007", ClientId = clients[12].Id, MatterId = matters[18].Id, Status = InvoiceStatus.WrittenOff, IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-250)), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-220)), SubTotal = 5000m, TaxRate = 20m, TaxAmount = 1000m, TotalAmount = 6000m, PaidAmount = 0m, Notes = "Chen estate - written off per partner decision", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-250) };
        invoices.Add(inv7);

        context.Invoices.AddRange(invoices);

        // ============================================================
        // PAYMENTS
        // ============================================================
        context.Payments.AddRange(
            new Payment { InvoiceId = inv1.Id, Amount = 3000m, PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-28)), PaymentMethod = PaymentMethod.BankTransfer, Reference = "BACS-TH-001", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-28) },
            new Payment { InvoiceId = inv3.Id, Amount = 5000m, PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), PaymentMethod = PaymentMethod.BankTransfer, Reference = "BACS-OAK-002", Notes = "Partial payment - remainder due on completion", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Payment { InvoiceId = inv5.Id, Amount = 9600m, PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-85)), PaymentMethod = PaymentMethod.BankTransfer, Reference = "BACS-TV-003", CreatedBy = emma.Id, CreatedAt = DateTime.UtcNow.AddDays(-85) }
        );

        // ============================================================
        // COMPLIANCE CHECKS
        // ============================================================
        context.ComplianceChecks.AddRange(
            new ComplianceCheck { ClientId = clients[0].Id, CheckType = ComplianceCheckType.AML, Status = ComplianceCheckStatus.Pass, RiskLevel = RiskLevel.Low, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-170), Notes = "Standard AML - passed. Low risk individual client.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-170) },
            new ComplianceCheck { ClientId = clients[1].Id, CheckType = ComplianceCheckType.KYC, Status = ComplianceCheckStatus.Pass, RiskLevel = RiskLevel.Low, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-160), Notes = "KYC completed. Directors verified via Companies House.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-160) },
            new ComplianceCheck { ClientId = clients[3].Id, CheckType = ComplianceCheckType.AML, Status = ComplianceCheckStatus.Pass, RiskLevel = RiskLevel.Medium, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-140), Notes = "Medium risk due to venture capital funding structure. Enhanced due diligence completed.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-140) },
            new ComplianceCheck { ClientId = clients[5].Id, CheckType = ComplianceCheckType.AML, Status = ComplianceCheckStatus.Fail, RiskLevel = RiskLevel.High, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-95), Notes = "FAILED - Director previously involved in dissolved company with outstanding HMRC debt. Matter placed on hold.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-95) },
            new ComplianceCheck { ClientId = clients[7].Id, CheckType = ComplianceCheckType.ConflictOfInterest, Status = ComplianceCheckStatus.Pass, RiskLevel = RiskLevel.Low, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-130), Notes = "No conflicts identified. Firm does not act for any competitors.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-130) },
            new ComplianceCheck { ClientId = clients[8].Id, CheckType = ComplianceCheckType.KYC, Status = ComplianceCheckStatus.Pending, RiskLevel = null, PerformedById = null, PerformedAt = null, Notes = "Awaiting ID documents from client.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new ComplianceCheck { ClientId = clients[11].Id, CheckType = ComplianceCheckType.RiskAssessment, Status = ComplianceCheckStatus.RequiresInvestigation, RiskLevel = RiskLevel.High, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-20), Notes = "Unusual payment patterns detected. Escalated for further investigation.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new ComplianceCheck { ClientId = clients[13].Id, CheckType = ComplianceCheckType.AML, Status = ComplianceCheckStatus.Pass, RiskLevel = RiskLevel.Medium, PerformedById = david.Id, PerformedAt = DateTime.UtcNow.AddDays(-30), Notes = "International logistics company. Enhanced checks completed - satisfactory.", CreatedBy = david.Id, CreatedAt = DateTime.UtcNow.AddDays(-30) }
        );

        // ============================================================
        // NOTIFICATIONS
        // ============================================================
        context.Notifications.AddRange(
            new Notification { UserId = sarah.Id, Title = "Time Entry Approved", Message = "Your time entry for MAT-00004 (3 hours) has been approved.", Type = NotificationType.Info, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Notification { UserId = michael.Id, Title = "Task Overdue", Message = "Task 'Chase client for financial disclosure' on MAT-00008 is overdue.", Type = NotificationType.Warning, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Notification { UserId = emma.Id, Title = "Invoice Overdue", Message = "Invoice INV-00002 for TechVentures Group PLC is 15 days overdue (£10,500).", Type = NotificationType.Warning, CreatedAt = DateTime.UtcNow.AddHours(-6) },
            new Notification { UserId = sarah.Id, Title = "New Matter Assigned", Message = "You have been assigned as lead on MAT-00022 - IP Assignment Agreement.", Type = NotificationType.Info, CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new Notification { UserId = david.Id, Title = "Compliance Alert", Message = "Risk assessment for Nexus Pharmaceuticals requires investigation.", Type = NotificationType.Action, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Notification { UserId = james.Id, Title = "Matter Status Changed", Message = "MAT-00010 (HSE Investigation) has been placed On Hold.", Type = NotificationType.Info, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Notification { UserId = adminUser.Id, Title = "New Client Pending", Message = "Greenfield Estates (CLI-00009) requires compliance check before matters can proceed.", Type = NotificationType.Action, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Notification { UserId = sarah.Id, Title = "Payment Received", Message = "Payment of £5,000 received from Oakwood Developments against INV-00003.", Type = NotificationType.Info, IsRead = true, ReadAt = DateTime.UtcNow.AddDays(-1), CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );

        // ============================================================
        // AUDIT ENTRIES — sample history
        // ============================================================
        context.AuditEntries.AddRange(
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-60), UserId = sarah.Id, UserEmail = sarah.Email, Action = "Created", EntityType = "Matter", EntityId = matters[0].Id, NewValues = "{\"Title\":\"Residential Purchase - 45 Oak Lane\",\"Status\":\"Open\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-55), UserId = sarah.Id, UserEmail = sarah.Email, Action = "StatusChanged", EntityType = "Matter", EntityId = matters[0].Id, OldValues = "{\"Status\":\"Open\"}", NewValues = "{\"Status\":\"InProgress\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-45), UserId = emma.Id, UserEmail = emma.Email, Action = "Created", EntityType = "Invoice", EntityId = inv2.Id, NewValues = "{\"InvoiceNumber\":\"INV-00002\",\"TotalAmount\":10500}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-45), UserId = emma.Id, UserEmail = emma.Email, Action = "Issued", EntityType = "Invoice", EntityId = inv2.Id, OldValues = "{\"Status\":\"Draft\"}", NewValues = "{\"Status\":\"Issued\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-28), UserId = emma.Id, UserEmail = emma.Email, Action = "PaymentRecorded", EntityType = "Invoice", EntityId = inv1.Id, NewValues = "{\"Amount\":3000,\"Method\":\"BankTransfer\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-20), UserId = david.Id, UserEmail = david.Email, Action = "ComplianceCheckCompleted", EntityType = "Client", EntityId = clients[11].Id, NewValues = "{\"Result\":\"RequiresInvestigation\",\"RiskLevel\":\"High\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-10), UserId = emma.Id, UserEmail = emma.Email, Action = "PaymentRecorded", EntityType = "Invoice", EntityId = inv3.Id, NewValues = "{\"Amount\":5000,\"Method\":\"BankTransfer\",\"Note\":\"Partial payment\"}" },
            new AuditEntry { Timestamp = DateTime.UtcNow.AddDays(-5), UserId = sarah.Id, UserEmail = sarah.Email, Action = "StatusChanged", EntityType = "Matter", EntityId = matters[8].Id, OldValues = "{\"Status\":\"InProgress\"}", NewValues = "{\"Status\":\"BillingReview\"}" }
        );

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
