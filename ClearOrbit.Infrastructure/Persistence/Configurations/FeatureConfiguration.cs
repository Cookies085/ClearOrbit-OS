using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(f => f.Code).IsUnique();

        builder.Property(f => f.Title).IsRequired().HasMaxLength(300);
        builder.Property(f => f.Description).HasColumnType("nvarchar(max)");

        builder.Property(f => f.Status).HasConversion<int>();
        builder.Property(f => f.Priority).HasConversion<int>();
        builder.Property(f => f.Source).HasConversion<int>();

        builder.HasOne(f => f.Division)
            .WithMany(d => d.Features)
            .HasForeignKey(f => f.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Project)
            .WithMany(p => p.Features)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.Client)
            .WithMany(c => c.RequestedFeatures)
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.AssignedToUser)
            .WithMany(u => u.AssignedFeatures)
            .HasForeignKey(f => f.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Release link will be added in Slice 4C
    }
}