using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status).HasConversion<int>();
        builder.Property(e => e.Notes).HasColumnType("nvarchar(max)");

        // One learner can be enrolled in a given class only once
        builder.HasIndex(e => new { e.ClassId, e.LearnerId }).IsUnique();

        builder.HasOne(e => e.Learner)
            .WithMany(l => l.Enrollments)
            .HasForeignKey(e => e.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}