using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Projects;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectType Type { get; set; } = ProjectType.External;

    public Guid? ClientId { get; set; }
    public Guid DivisionId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? RequestingDivisionId { get; set; }

    public decimal? Budget { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
}