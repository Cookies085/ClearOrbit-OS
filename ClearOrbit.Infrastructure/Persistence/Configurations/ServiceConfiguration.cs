using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasColumnType("nvarchar(max)");
        builder.Property(s => s.BasePrice).HasPrecision(18, 2);
        builder.Property(s => s.Currency).IsRequired().HasMaxLength(3);
        builder.Property(s => s.Unit).HasConversion<int>();

        builder.HasIndex(s => new { s.DivisionId, s.Name }).IsUnique();

        builder.HasOne(s => s.Division)
            .WithMany(d => d.Services)
            .HasForeignKey(s => s.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}