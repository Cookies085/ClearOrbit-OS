using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);

        // e.g., Invoice:Create - Unique
        builder.Property(p => p.Code).IsRequired().HasMaxLength(100);
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Description).HasMaxLength(500);
    }
}