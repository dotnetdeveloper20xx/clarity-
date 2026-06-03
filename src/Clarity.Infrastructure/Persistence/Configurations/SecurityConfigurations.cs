using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).HasMaxLength(200).IsRequired();
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.UserId);
        builder.Property(rt => rt.DeviceInfo).HasMaxLength(500);
        builder.Property(rt => rt.IpAddress).HasMaxLength(50);
        builder.Property(rt => rt.ReplacedByToken).HasMaxLength(200);
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);

        builder.HasOne(rt => rt.User).WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId);
        builder.Property(s => s.DeviceInfo).HasMaxLength(500);
        builder.Property(s => s.IpAddress).HasMaxLength(50);

        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.RefreshToken).WithMany().HasForeignKey(s => s.RefreshTokenId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.Name).IsUnique();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Category).HasMaxLength(50).IsRequired();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(rp => rp.Id);
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

        builder.HasOne(rp => rp.Role).WithMany().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MatterAccessConfiguration : IEntityTypeConfiguration<MatterAccess>
{
    public void Configure(EntityTypeBuilder<MatterAccess> builder)
    {
        builder.ToTable("MatterAccesses");
        builder.HasKey(ma => ma.Id);
        builder.HasIndex(ma => new { ma.MatterId, ma.UserId }).IsUnique();

        builder.HasOne(ma => ma.Matter).WithMany().HasForeignKey(ma => ma.MatterId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ma => ma.User).WithMany().HasForeignKey(ma => ma.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SecurityAuditLogConfiguration : IEntityTypeConfiguration<SecurityAuditLog>
{
    public void Configure(EntityTypeBuilder<SecurityAuditLog> builder)
    {
        builder.ToTable("SecurityAuditLogs");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.EventType).HasMaxLength(50).IsRequired();
        builder.Property(s => s.UserEmail).HasMaxLength(256);
        builder.Property(s => s.IpAddress).HasMaxLength(50);
        builder.Property(s => s.DeviceInfo).HasMaxLength(500);
        builder.Property(s => s.Details).HasMaxLength(1000);

        builder.HasIndex(s => s.EventType);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Timestamp);
    }
}
