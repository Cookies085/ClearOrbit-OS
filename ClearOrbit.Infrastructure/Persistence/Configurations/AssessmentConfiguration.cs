using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(a => a.Code).IsUnique();

        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Description).HasColumnType("nvarchar(max)");
        builder.Property(a => a.MaxScore).HasPrecision(18, 2);
        builder.Property(a => a.Weight).HasPrecision(5, 2);

        builder.Property(a => a.Type).HasConversion<int>();
        builder.Property(a => a.Status).HasConversion<int>();

        builder.HasOne(a => a.Class)
            .WithMany(c => c.Assessments)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Results)
            .WithOne(r => r.Assessment)
            .HasForeignKey(r => r.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}