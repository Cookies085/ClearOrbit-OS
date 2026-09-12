using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status).HasConversion<int>();
        builder.Property(a => a.Notes).HasColumnType("nvarchar(max)");
        builder.Property(a => a.SessionDate).HasColumnType("date");

        // One attendance record per learner per class per session date
        builder.HasIndex(a => new { a.ClassId, a.LearnerId, a.SessionDate }).IsUnique();

        builder.HasOne(a => a.Class)
            .WithMany(c => c.AttendanceRecords)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Learner)
            .WithMany(l => l.AttendanceRecords)
            .HasForeignKey(a => a.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.RecordedByUser)
            .WithMany(u => u.RecordedAttendance)
            .HasForeignKey(a => a.RecordedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}