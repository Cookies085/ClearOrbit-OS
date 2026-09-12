using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.ToTable("Results");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Score).HasPrecision(18, 2);
        builder.Property(r => r.Notes).HasColumnType("nvarchar(max)");

        // One result per learner per assessment
        builder.HasIndex(r => new { r.AssessmentId, r.LearnerId }).IsUnique();

        builder.HasOne(r => r.Learner)
            .WithMany(l => l.Results)
            .HasForeignKey(r => r.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RecordedByUser)
            .WithMany(u => u.RecordedResults)
            .HasForeignKey(r => r.RecordedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}