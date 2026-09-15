using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Software;

public class UpdateBugDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }

    public BugStatus Status { get; set; }
    public BugSeverity Severity { get; set; }
    public TaskPriority Priority { get; set; }

    public Guid DivisionId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? FeatureId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}