using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ReferenceNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(c => c.ReferenceNumber).IsUnique();
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.AddressLine1).HasMaxLength(200);
        builder.Property(c => c.AddressLine2).HasMaxLength(200);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.County).HasMaxLength(100);
        builder.Property(c => c.PostCode).HasMaxLength(20);
        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(c => c.CompanyNumber).HasMaxLength(50);
        builder.Property(c => c.RowVersion).IsRowVersion();
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasMany(c => c.Matters).WithOne(m => m.Client).HasForeignKey(m => m.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Documents).WithOne(d => d.Client).HasForeignKey(d => d.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.ComplianceChecks).WithOne(cc => cc.Client).HasForeignKey(cc => cc.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Invoices).WithOne(i => i.Client).HasForeignKey(i => i.ClientId).OnDelete(DeleteBehavior.Restrict);
    }
}
