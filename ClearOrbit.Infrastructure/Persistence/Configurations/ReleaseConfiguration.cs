using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable("Releases");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.Code).IsUnique();

        builder.Property(r => r.Version).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Description).HasColumnType("nvarchar(max)");
        builder.Property(r => r.ReleaseNotes).HasColumnType("nvarchar(max)");

        builder.Property(r => r.Status).HasConversion<int>();

        // One version per project
        builder.HasIndex(r => new { r.ProjectId, r.Version })
            .IsUnique()
            .HasFilter("[ProjectId] IS NOT NULL");

        builder.HasOne(r => r.Division)
            .WithMany(d => d.Releases)
            .HasForeignKey(r => r.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Project)
            .WithMany(p => p.Releases)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.ReleaseManagerUser)
            .WithMany(u => u.ManagedReleases)
            .HasForeignKey(r => r.ReleaseManagerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(r => r.Features)
            .WithOne(f => f.Release)
            .HasForeignKey(f => f.ReleaseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}