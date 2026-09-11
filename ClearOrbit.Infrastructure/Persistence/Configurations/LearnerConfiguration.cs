using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class LearnerConfiguration : IEntityTypeConfiguration<Learner>
{
    public void Configure(EntityTypeBuilder<Learner> builder)
    {
        builder.ToTable("Learners");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(l => l.Code).IsUnique();

        builder.Property(l => l.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(l => l.LastName).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Email).HasMaxLength(255);
        builder.Property(l => l.Phone).HasMaxLength(50);
        builder.Property(l => l.Address).HasMaxLength(500);
        builder.Property(l => l.GuardianName).HasMaxLength(200);
        builder.Property(l => l.GuardianPhone).HasMaxLength(50);
        builder.Property(l => l.GuardianEmail).HasMaxLength(255);
        builder.Property(l => l.Notes).HasColumnType("nvarchar(max)");

        builder.Property(l => l.GradeLevel).HasConversion<int>();
        builder.Property(l => l.Status).HasConversion<int>();
    }
}