using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class ServiceViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Service name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal BasePrice { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "ZAR";

    public int Unit { get; set; } = 1;

    [Required(ErrorMessage = "Please select a division.")]
    public Guid DivisionId { get; set; }

    public bool IsActive { get; set; } = true;

    // For dropdown
    public List<DivisionOption> Divisions { get; set; } = new();
}

public class DivisionOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
}

public class ServiceListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public int Unit { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}