using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Feature : BaseEntity
{
    public string Code { get; set; } = string.Empty;  // CO-FTR-00001
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public FeatureStatus Status { get; set; } = FeatureStatus.Proposed;
    public FeaturePriority Priority { get; set; } = FeaturePriority.Normal;
    public FeatureSource Source { get; set; } = FeatureSource.Client;

    // Where it belongs
    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    // Optional: tied to a specific project
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    // Optional: requested by a specific client
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }

    // Optional: assigned to a user
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    // Effort estimate (story points, days — whatever you prefer)
    public int? EstimatedEffort { get; set; }
    public int? ActualEffort { get; set; }

    public DateTime? TargetDate { get; set; }
    public DateTime? ShippedAt { get; set; }

    public Guid? ReleaseId { get; set; }
    public Release? Release { get; set; }

    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}