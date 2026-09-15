namespace ClearOrbit.Application.DTOs.Software;

public class BugResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }

    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Severity { get; set; }
    public string SeverityName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;

    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;

    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }

    public Guid? FeatureId { get; set; }
    public string? FeatureCode { get; set; }
    public string? FeatureTitle { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }

    public DateTime ReportedAt { get; set; }
    public DateTime? FixedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}