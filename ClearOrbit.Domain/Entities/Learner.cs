using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Learner : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-LRN-00001
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public GradeLevel GradeLevel { get; set; } = GradeLevel.NotApplicable;
    public LearnerStatus Status { get; set; } = LearnerStatus.Active;

    // Contact
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    // Guardian (for minors)
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }

    public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}