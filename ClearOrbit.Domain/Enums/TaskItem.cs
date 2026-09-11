using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public WorkItemStatus Status { get; set; } = WorkItemStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
}