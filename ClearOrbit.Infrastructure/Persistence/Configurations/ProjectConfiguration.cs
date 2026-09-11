using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
        builder.Property(p => p.Budget).HasPrecision(18, 2);
        builder.Property(p => p.Type).HasConversion<int>();
        builder.Property(p => p.Status).HasConversion<int>();

        // Client (optional)
        builder.HasOne(p => p.Client)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Delivering Division (required)
        builder.HasOne(p => p.Division)
            .WithMany(d => d.DeliveredProjects)
            .HasForeignKey(p => p.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Service (optional)
        builder.HasOne(p => p.Service)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Requesting Division (optional, internal projects only)
        builder.HasOne(p => p.RequestingDivision)
            .WithMany(d => d.RequestedProjects)
            .HasForeignKey(p => p.RequestingDivisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}