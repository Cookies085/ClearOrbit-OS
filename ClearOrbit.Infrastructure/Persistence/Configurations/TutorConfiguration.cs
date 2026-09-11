using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("Tutors");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.Code).IsUnique();

        builder.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.LastName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Email).HasMaxLength(255);
        builder.Property(t => t.Phone).HasMaxLength(50);
        builder.Property(t => t.Specializations).HasMaxLength(500);
        builder.Property(t => t.Bio).HasColumnType("nvarchar(max)");

        builder.HasOne(t => t.User)
            .WithOne(u => u.TutorProfile)
            .HasForeignKey<Tutor>(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}