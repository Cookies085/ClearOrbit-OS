using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class LearnerViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public int GradeLevel { get; set; } = 0;

    public int Status { get; set; } = 1;

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }

    [EmailAddress]
    public string? GuardianEmail { get; set; }

    public string? Notes { get; set; }
}

public class LearnerListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime? DateOfBirth { get; set; }
    public int Age { get; set; }
    public int GradeLevel { get; set; }
    public string GradeLevelName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public DateTime EnrolledOn { get; set; }
}

public class TutorViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }

    [Required(ErrorMessage = "At least one specialization is required.")]
    [StringLength(500)]
    public string Specializations { get; set; } = string.Empty;

    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TutorListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Specializations { get; set; } = string.Empty;
    public DateTime JoinedOn { get; set; }
    public bool IsActive { get; set; }
}

public class AcademyWorkspaceViewModel
{
    public int LearnerCount { get; set; }
    public int TutorCount { get; set; }
    public int ClassCount { get; set; }
    public int TotalEnrollments { get; set; }
    public int SessionCount { get; set; }
    public List<LearnerListItem> RecentLearners { get; set; } = new();
    public List<TutorListItem> RecentTutors { get; set; } = new();
    public List<ClassListItem> RecentClasses { get; set; } = new();
}

public class ClassViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Class name is required.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Subject is required.")]
    public string Subject { get; set; } = string.Empty;

    public int GradeLevel { get; set; } = 0;

    [Required(ErrorMessage = "Please select a tutor.")]
    public Guid TutorId { get; set; }

    public int DayOfWeek { get; set; } = 1;
    public TimeOnly StartTime { get; set; } = new TimeOnly(16, 0);
    public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0);

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    public string? Location { get; set; }
    public int MaxCapacity { get; set; } = 20;
    public decimal FeePerLearner { get; set; }
    public string Currency { get; set; } = "ZAR";
    public int Status { get; set; } = 1;

    // Dropdowns
    public List<TutorOption> Tutors { get; set; } = new();
}

public class TutorOption
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specializations { get; set; } = string.Empty;
}

public class ClassListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public string GradeLevelName { get; set; } = string.Empty;
    public Guid TutorId { get; set; }
    public string TutorName { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public int MaxCapacity { get; set; }
    public int EnrolledCount { get; set; }
    public int AvailableSeats { get; set; }
    public decimal FeePerLearner { get; set; }
    public string Currency { get; set; } = "ZAR";
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class EnrollmentListItem
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public string? LearnerEmail { get; set; }
    public DateTime EnrolledOn { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class AttendanceListItem
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class SessionSummaryItem
{
    public DateTime SessionDate { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public int TotalExcused { get; set; }
    public int TotalRecords { get; set; }
    public decimal AttendanceRate { get; set; }
}

public class TakeAttendanceViewModel
{
    public Guid ClassId { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; } = DateTime.UtcNow.Date;
    public List<TakeAttendanceRow> Rows { get; set; } = new();
}

public class TakeAttendanceRow
{
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public string? GradeLevelName { get; set; }
    public int Status { get; set; } = 1; // default Present
    public string? Notes { get; set; }
}

public class AttendanceHubRow
{
    public Guid ClassId { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string GradeLevelName { get; set; } = string.Empty;
    public string TutorName { get; set; } = string.Empty;
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public int EnrolledCount { get; set; }
    public int SessionCount { get; set; }
    public decimal AttendanceRate { get; set; }
    public DateTime? LastSessionDate { get; set; }
}