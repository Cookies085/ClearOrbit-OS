namespace ClearOrbit.Application.DTOs.Software;

public class ReleaseResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReleaseNotes { get; set; }

    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;

    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }

    public DateTime? PlannedDate { get; set; }
    public DateTime? ReleasedAt { get; set; }

    public Guid? ReleaseManagerUserId { get; set; }
    public string? ReleaseManagerName { get; set; }

    public int FeatureCount { get; set; }
    public List<FeatureSummaryDto> Features { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}

public class FeatureSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
}