using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Result : BaseEntity
{
    public Guid AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;

    public Guid LearnerId { get; set; }
    public Learner Learner { get; set; } = null!;

    public decimal Score { get; set; }
    public string? Notes { get; set; }

    public Guid? RecordedByUserId { get; set; }
    public User? RecordedByUser { get; set; }
}