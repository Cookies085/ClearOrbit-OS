using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class AssessmentViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid ClassId { get; set; }
    public string? ClassCode { get; set; }
    public string? ClassName { get; set; }

    [Required]
    public int Type { get; set; } = 1;
    public int Status { get; set; } = 1;

    [DataType(DataType.Date)]
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow.Date;

    [Range(0.01, double.MaxValue)]
    public decimal MaxScore { get; set; } = 100;

    [Range(0, 100)]
    public decimal Weight { get; set; } = 0;

    // Dropdown (only when creating from a form without a preset class)
    public List<ClassOption> Classes { get; set; } = new();
}

public class ClassOption
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class AssessmentListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ClassId { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }
    public int ResultsRecorded { get; set; }
    public decimal AverageScore { get; set; }
    public decimal AveragePercentage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ResultListItem
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Percentage { get; set; }
    public string? Notes { get; set; }
}

public class RecordResultsViewModel
{
    public Guid AssessmentId { get; set; }
    public string AssessmentCode { get; set; } = string.Empty;
    public string AssessmentTitle { get; set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public List<RecordResultRow> Rows { get; set; } = new();
}

public class RecordResultRow
{
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public string? Notes { get; set; }
}