namespace ClearOrbit.Application.DTOs.Projects;

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public Guid? ClientId { get; set; }
    public string? ClientName { get; set; }

    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;

    public Guid? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    public Guid? RequestingDivisionId { get; set; }
    public string? RequestingDivisionName { get; set; }

    public decimal? Budget { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}