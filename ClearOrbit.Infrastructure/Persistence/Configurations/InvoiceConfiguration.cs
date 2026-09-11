using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(i => i.Code).IsUnique();

        builder.Property(i => i.Status).HasConversion<int>();
        builder.Property(i => i.Currency).IsRequired().HasMaxLength(3);
        builder.Property(i => i.Notes).HasColumnType("nvarchar(max)");

        builder.Property(i => i.SubTotal).HasPrecision(18, 2);
        builder.Property(i => i.TaxRate).HasPrecision(5, 2);
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2);
        builder.Property(i => i.Total).HasPrecision(18, 2);
        builder.Property(i => i.AmountPaid).HasPrecision(18, 2);

        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Project)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Division)
            .WithMany(d => d.IssuedInvoices)
            .HasForeignKey(i => i.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        builder.HasKey(it => it.Id);

        builder.Property(it => it.Description).IsRequired().HasMaxLength(500);
        builder.Property(it => it.Quantity).HasPrecision(18, 4);
        builder.Property(it => it.UnitPrice).HasPrecision(18, 2);
        builder.Property(it => it.LineTotal).HasPrecision(18, 2);
    }
}