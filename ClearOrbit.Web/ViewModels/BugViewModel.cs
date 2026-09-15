using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class BugViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }

    [Required]
    public Guid DivisionId { get; set; }

    public Guid? ProjectId { get; set; }
    public Guid? FeatureId { get; set; }

    public int Status { get; set; } = 1;
    public int Severity { get; set; } = 3;
    public int Priority { get; set; } = 2;

    // Dropdowns
    public List<DivisionOption> Divisions { get; set; } = new();
    public List<ProjectOption> Projects { get; set; } = new();
    public List<FeatureOption> Features { get; set; } = new();

    public string? Code { get; set; }
}

public class FeatureOption
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class BugListItem
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