using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Software;

public class CreateFeatureDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public FeaturePriority Priority { get; set; } = FeaturePriority.Normal;
    public FeatureSource Source { get; set; } = FeatureSource.Client;

    public Guid DivisionId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? AssignedToUserId { get; set; }

    public int? EstimatedEffort { get; set; }
    public DateTime? TargetDate { get; set; }
}