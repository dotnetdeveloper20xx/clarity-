using Clarity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clarity.Infrastructure.Persistence.Configurations;

public class InvoiceLineItemConfiguration : IEntityTypeConfiguration<InvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
    {
        builder.ToTable("InvoiceLineItems");
        builder.HasKey(li => li.Id);
        builder.Property(li => li.Description).HasMaxLength(500).IsRequired();
        builder.Property(li => li.Quantity).HasColumnType("decimal(10,2)");
        builder.Property(li => li.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(li => li.Amount).HasColumnType("decimal(18,2)");
    }
}
