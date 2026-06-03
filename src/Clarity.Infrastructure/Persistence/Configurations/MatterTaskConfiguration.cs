using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class MatterTaskConfiguration : IEntityTypeConfiguration<MatterTask>
{
    public void Configure(EntityTypeBuilder<MatterTask> builder)
    {
        builder.ToTable("MatterTasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasOne(t => t.Assignee).WithMany().HasForeignKey(t => t.AssigneeId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class MatterTeamMemberConfiguration : IEntityTypeConfiguration<MatterTeamMember>
{
    public void Configure(EntityTypeBuilder<MatterTeamMember> builder)
    {
        builder.ToTable("MatterTeamMembers");
        builder.HasKey(tm => tm.Id);
        builder.HasIndex(tm => new { tm.MatterId, tm.UserId }).IsUnique();

        builder.HasOne(tm => tm.User).WithMany().HasForeignKey(tm => tm.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}

public class MatterNoteConfiguration : IEntityTypeConfiguration<MatterNote>
{
    public void Configure(EntityTypeBuilder<MatterNote> builder)
    {
        builder.ToTable("MatterNotes");
        builder.HasKey(n => n.Id);
        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
