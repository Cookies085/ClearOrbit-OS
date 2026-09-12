using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid LearnerId { get; set; }
    public Learner Learner { get; set; } = null!;

    public DateTime SessionDate { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }

    public Guid? RecordedByUserId { get; set; }
    public User? RecordedByUser { get; set; }
}