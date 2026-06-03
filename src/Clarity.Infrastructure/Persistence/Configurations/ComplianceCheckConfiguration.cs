using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class ComplianceCheckConfiguration : IEntityTypeConfiguration<ComplianceCheck>
{
    public void Configure(EntityTypeBuilder<ComplianceCheck> builder)
    {
        builder.ToTable("ComplianceChecks");
        builder.HasKey(cc => cc.Id);
        builder.HasIndex(cc => cc.ClientId);
        builder.HasIndex(cc => cc.Status);

        builder.HasOne(cc => cc.PerformedBy).WithMany().HasForeignKey(cc => cc.PerformedById).OnDelete(DeleteBehavior.Restrict);
    }
}
