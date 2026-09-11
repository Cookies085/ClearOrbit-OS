using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class ProjectViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int Type { get; set; } = 1;    // 1 = External, 2 = Internal

    public int Status { get; set; } = 1;  // 1 = Draft

    public Guid? ClientId { get; set; }

    [Required(ErrorMessage = "Please select a delivering division.")]
    public Guid DivisionId { get; set; }

    public Guid? ServiceId { get; set; }
    public Guid? RequestingDivisionId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Budget { get; set; }

    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "ZAR";

    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    // Dropdowns
    public List<ClientOption> Clients { get; set; } = new();
    public List<DivisionOption> Divisions { get; set; } = new();
    public List<ServiceOption> Services { get; set; } = new();
}

public class ClientOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ServiceOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DivisionId { get; set; }
}

public class ProjectListItem
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