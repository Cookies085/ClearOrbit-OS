using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Project : BaseEntity
{
    // Identity
    public string Code { get; set; } = string.Empty;   // e.g., CO-ACD-00001
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Classification
    public ProjectType Type { get; set; } = ProjectType.External;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    // Client (nullable — internal projects have none)
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }

    // Division delivering the work
    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    // Service being provided (nullable)
    public Guid? ServiceId { get; set; }
    public Service? Service { get; set; }

    // For internal projects only: who requested the work?
    public Guid? RequestingDivisionId { get; set; }
    public Division? RequestingDivision { get; set; }

    // Commercial & Schedule
    public decimal? Budget { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}