using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class TaskViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Please select a project.")]
    public Guid ProjectId { get; set; }

    public int Status { get; set; } = 1;
    public int Priority { get; set; } = 2;

    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }

    // Dropdowns
    public List<ProjectOption> Projects { get; set; } = new();
    public List<UserOption> Users { get; set; } = new();

    // Display
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }
}

public class ProjectOption
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
}

public class UserOption
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class TaskListItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDivisionAccent { get; set; } = string.Empty;
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}