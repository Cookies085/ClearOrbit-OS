using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Class : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-CLS-00001
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Subject { get; set; } = string.Empty;   
    public GradeLevel GradeLevel { get; set; } = GradeLevel.NotApplicable;

    public Guid TutorId { get; set; }
    public Tutor Tutor { get; set; } = null!;

    public DayOfWeekOption DayOfWeek { get; set; } = DayOfWeekOption.Monday;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EndDate { get; set; }

    public string? Location { get; set; }   // "Room 3" or Zoom link

    public int MaxCapacity { get; set; } = 20;
    public decimal FeePerLearner { get; set; }
    public string Currency { get; set; } = "ZAR";

    public ClassStatus Status { get; set; } = ClassStatus.Draft;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}