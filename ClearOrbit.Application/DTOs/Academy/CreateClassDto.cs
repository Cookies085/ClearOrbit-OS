using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Academy;

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Subject { get; set; } = string.Empty;
    public GradeLevel GradeLevel { get; set; } = GradeLevel.NotApplicable;
    public Guid TutorId { get; set; }
    public DayOfWeekOption DayOfWeek { get; set; } = DayOfWeekOption.Monday;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public int MaxCapacity { get; set; } = 20;
    public decimal FeePerLearner { get; set; }
    public string Currency { get; set; } = "ZAR";
}