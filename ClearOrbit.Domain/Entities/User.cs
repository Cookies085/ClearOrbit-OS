using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.ValueObjects;

namespace ClearOrbit.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public EmailAddress Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public string SystemPrefix { get; set; } = "CO";
    public Tutor? TutorProfile { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<AttendanceRecord> RecordedAttendance { get; set; } = new List<AttendanceRecord>();
    public ICollection<Result> RecordedResults { get; set; } = new List<Result>();
}