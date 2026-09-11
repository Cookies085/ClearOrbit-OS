using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder.ToTable("Divisions");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.AccentColor)
            .HasMaxLength(50);

        // Ensure unique division names per organization
        builder.HasIndex(d => new { d.OrganizationId, d.Name }).IsUnique();
    }
}