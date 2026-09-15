using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class BugConfiguration : IEntityTypeConfiguration<Bug>
{
    public void Configure(EntityTypeBuilder<Bug> builder)
    {
        builder.ToTable("Bugs");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.Code).IsUnique();

        builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
        builder.Property(b => b.Description).HasColumnType("nvarchar(max)");
        builder.Property(b => b.StepsToReproduce).HasColumnType("nvarchar(max)");
        builder.Property(b => b.ExpectedBehavior).HasColumnType("nvarchar(max)");
        builder.Property(b => b.ActualBehavior).HasColumnType("nvarchar(max)");

        builder.Property(b => b.Status).HasConversion<int>();
        builder.Property(b => b.Severity).HasConversion<int>();
        builder.Property(b => b.Priority).HasConversion<int>();

        builder.HasOne(b => b.Division)
            .WithMany(d => d.Bugs)
            .HasForeignKey(b => b.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Project)
            .WithMany(p => p.Bugs)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Feature)
            .WithMany(f => f.Bugs)
            .HasForeignKey(b => b.FeatureId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.ReportedByUser)
            .WithMany(u => u.ReportedBugs)
            .HasForeignKey(b => b.ReportedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.AssignedToUser)
            .WithMany(u => u.AssignedBugs)
            .HasForeignKey(b => b.AssignedToUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}