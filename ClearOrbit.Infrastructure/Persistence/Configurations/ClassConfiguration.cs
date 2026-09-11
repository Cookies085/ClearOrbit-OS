using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasColumnType("nvarchar(max)");
        builder.Property(c => c.Subject).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Location).HasMaxLength(300);
        builder.Property(c => c.Currency).IsRequired().HasMaxLength(3);
        builder.Property(c => c.FeePerLearner).HasPrecision(18, 2);

        builder.Property(c => c.GradeLevel).HasConversion<int>();
        builder.Property(c => c.DayOfWeek).HasConversion<int>();
        builder.Property(c => c.Status).HasConversion<int>();

        builder.HasOne(c => c.Tutor)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.Class)
            .HasForeignKey(e => e.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}