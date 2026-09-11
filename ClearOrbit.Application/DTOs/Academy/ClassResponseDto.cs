namespace ClearOrbit.Application.DTOs.Academy;

public class ClassResponseDto
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
    public string StartTime { get; set; } = string.Empty;   // "16:00"
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