using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Assessment : BaseEntity
{
    public string Code { get; set; } = string.Empty;  // CO-ASM-00001
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public AssessmentType Type { get; set; } = AssessmentType.Quiz;
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;

    public DateTime ScheduledDate { get; set; } = DateTime.UtcNow.Date;
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; } = 0;   // % contribution to final grade (0 = unweighted)

    public ICollection<Result> Results { get; set; } = new List<Result>();
}