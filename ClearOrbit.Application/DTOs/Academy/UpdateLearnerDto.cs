using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Academy;

public class UpdateLearnerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public GradeLevel GradeLevel { get; set; } = GradeLevel.NotApplicable;
    public LearnerStatus Status { get; set; } = LearnerStatus.Active;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }
    public string? Notes { get; set; }
}