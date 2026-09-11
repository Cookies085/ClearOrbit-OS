using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Industry).HasMaxLength(100);
        builder.Property(c => c.Website).HasMaxLength(500);
        builder.Property(c => c.Notes).HasColumnType("nvarchar(max)");

        // No two clients with same name in same org
        builder.HasIndex(c => new { c.OrganizationId, c.Name }).IsUnique();

        builder.HasMany(c => c.Contacts)
            .WithOne(ct => ct.Client)
            .HasForeignKey(ct => ct.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}