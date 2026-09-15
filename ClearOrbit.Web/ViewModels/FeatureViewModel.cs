using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class FeatureViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Please select a division.")]
    public Guid DivisionId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? ClientId { get; set; }

    [Required]
    public int Source { get; set; } = 1;    // Client
    public int Priority { get; set; } = 2;  // Normal
    public int Status { get; set; } = 1;    // Proposed

    public int? EstimatedEffort { get; set; }
    public int? ActualEffort { get; set; }

    [DataType(DataType.Date)]
    public DateTime? TargetDate { get; set; }

    // Dropdowns
    public List<DivisionOption> Divisions { get; set; } = new();
    public List<ProjectOption> Projects { get; set; } = new();
    public List<ClientOption> Clients { get; set; } = new();

    // Display
    public string? Code { get; set; }
}

public class FeatureListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public int Source { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public Guid? ClientId { get; set; }
    public string? ClientName { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public int? EstimatedEffort { get; set; }
    public int? ActualEffort { get; set; }
    public DateTime? TargetDate { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SoftwareWorkspaceViewModel
{
    public int TotalFeatures { get; set; }
    public int ShippedFeatures { get; set; }
    public int InProgressFeatures { get; set; }
    public int CriticalFeatures { get; set; }
    public int TotalBugs { get; set; }
    public int OpenBugs { get; set; }
    public int CriticalBugs { get; set; }
    public int TotalReleases { get; set; }
    public int ReleasedCount { get; set; }
    public List<ReleaseListItem> RecentReleases { get; set; } = new();
    public List<BugListItem> RecentBugs { get; set; } = new();
    public List<FeatureListItem> RecentFeatures { get; set; } = new();
}

public class ReleaseViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Version is required (e.g., v1.0.0).")]
    [StringLength(50)]
    public string Version { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ReleaseNotes { get; set; }

    public int Status { get; set; } = 1;

    [Required]
    public Guid DivisionId { get; set; }

    public Guid? ProjectId { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PlannedDate { get; set; }

    public List<Guid> FeatureIds { get; set; } = new();

    // Dropdowns
    public List<DivisionOption> Divisions { get; set; } = new();
    public List<ProjectOption> Projects { get; set; } = new();
    public List<FeatureOption> AvailableFeatures { get; set; } = new();

    // Display
    public string? Code { get; set; }
}

public class ReleaseListItem
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
    public List<FeatureSummaryItem> Features { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class FeatureSummaryItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
}