using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Release : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-REL-00001
    public string Version { get; set; } = string.Empty; // e.g., v1.2.0
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReleaseNotes { get; set; }

    public ReleaseStatus Status { get; set; } = ReleaseStatus.Planning;

    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public DateTime? PlannedDate { get; set; }
    public DateTime? ReleasedAt { get; set; }

    public Guid? ReleaseManagerUserId { get; set; }
    public User? ReleaseManagerUser { get; set; }

    public ICollection<Feature> Features { get; set; } = new List<Feature>();
}