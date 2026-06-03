using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Description).HasMaxLength(1000).IsRequired();
        builder.Property(t => t.RateAmount).HasColumnType("decimal(18,2)");
        builder.Property(t => t.RejectionReason).HasMaxLength(500);
        builder.Property(t => t.RowVersion).IsRowVersion();
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasIndex(t => t.MatterId);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Date);

        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.BillingRate).WithMany().HasForeignKey(t => t.BillingRateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Invoice).WithMany(i => i.TimeEntries).HasForeignKey(t => t.InvoiceId).OnDelete(DeleteBehavior.Restrict);
    }
}
