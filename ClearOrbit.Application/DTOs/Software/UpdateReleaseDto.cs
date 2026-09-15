using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Software;

public class UpdateReleaseDto
{
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReleaseNotes { get; set; }

    public ReleaseStatus Status { get; set; }

    public Guid DivisionId { get; set; }
    public Guid? ProjectId { get; set; }

    public DateTime? PlannedDate { get; set; }
    public Guid? ReleaseManagerUserId { get; set; }

    public List<Guid> FeatureIds { get; set; } = new();
}