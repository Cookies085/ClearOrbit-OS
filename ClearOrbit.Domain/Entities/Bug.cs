using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Bug : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-BUG-00001
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }

    public BugStatus Status { get; set; } = BugStatus.Open;
    public BugSeverity Severity { get; set; } = BugSeverity.Major;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid? FeatureId { get; set; }
    public Feature? Feature { get; set; }

    public Guid? ReportedByUserId { get; set; }
    public User? ReportedByUser { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FixedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}