namespace ClearOrbit.Application.DTOs.Academy;

public class EnrollmentResponseDto
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

public class CreateEnrollmentDto
{
    public Guid ClassId { get; set; }
    public Guid LearnerId { get; set; }
    public string? Notes { get; set; }
}