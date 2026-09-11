using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Code).IsUnique();

        builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Category).HasConversion<int>();
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Currency).IsRequired().HasMaxLength(3);
        builder.Property(e => e.Vendor).HasMaxLength(200);
        builder.Property(e => e.Reference).HasMaxLength(200);
        builder.Property(e => e.Notes).HasColumnType("nvarchar(max)");

        builder.HasOne(e => e.Project)
            .WithMany(p => p.Expenses)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Division)
            .WithMany(d => d.Expenses)
            .HasForeignKey(e => e.DivisionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}