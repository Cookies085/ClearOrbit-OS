using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Academy;

public class CreateAssessmentDto
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssessmentType Type { get; set; } = AssessmentType.Quiz;
    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow.Date;
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; } = 0;
}

public class UpdateAssessmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AssessmentType Type { get; set; }
    public AssessmentStatus Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }
}

public class AssessmentResponseDto
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

public class ResultResponseDto
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

public class SaveResultItemDto
{
    public Guid LearnerId { get; set; }
    public decimal? Score { get; set; }
    public string? Notes { get; set; }
}

public class SaveResultsDto
{
    public Guid AssessmentId { get; set; }
    public List<SaveResultItemDto> Results { get; set; } = new();
}