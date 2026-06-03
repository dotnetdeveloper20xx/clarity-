using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class MatterConfiguration : IEntityTypeConfiguration<Matter>
{
    public void Configure(EntityTypeBuilder<Matter> builder)
    {
        builder.ToTable("Matters");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.ReferenceNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(m => m.ReferenceNumber).IsUnique();
        builder.Property(m => m.Title).HasMaxLength(300).IsRequired();
        builder.Property(m => m.EstimatedValue).HasColumnType("decimal(18,2)");
        builder.Property(m => m.FixedFeeAmount).HasColumnType("decimal(18,2)");
        builder.Property(m => m.RowVersion).IsRowVersion();
        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.HasIndex(m => m.ClientId);
        builder.HasIndex(m => m.LeadConsultantId);
        builder.HasIndex(m => m.Status);

        builder.HasOne(m => m.LeadConsultant).WithMany().HasForeignKey(m => m.LeadConsultantId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(m => m.Notes).WithOne(n => n.Matter).HasForeignKey(n => n.MatterId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.Tasks).WithOne(t => t.Matter).HasForeignKey(t => t.MatterId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.TeamMembers).WithOne(tm => tm.Matter).HasForeignKey(tm => tm.MatterId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(m => m.TimeEntries).WithOne(te => te.Matter).HasForeignKey(te => te.MatterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(m => m.Documents).WithOne(d => d.Matter).HasForeignKey(d => d.MatterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(m => m.Invoices).WithOne(i => i.Matter).HasForeignKey(i => i.MatterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(m => m.ComplianceChecks).WithOne(cc => cc.Matter).HasForeignKey(cc => cc.MatterId).OnDelete(DeleteBehavior.Restrict);
    }
}
