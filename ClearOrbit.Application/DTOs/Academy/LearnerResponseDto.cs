namespace ClearOrbit.Application.DTOs.Academy;

public class LearnerResponseDto
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
    public string? Address { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }
    public string? Notes { get; set; }
    public DateTime EnrolledOn { get; set; }
    public DateTime CreatedAt { get; set; }
}