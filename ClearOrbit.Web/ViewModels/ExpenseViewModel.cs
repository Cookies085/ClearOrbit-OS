using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class ExpenseViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int Category { get; set; } = 99;

    public Guid? ProjectId { get; set; }
    public Guid? DivisionId { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow.Date;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "ZAR";

    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    // Dropdowns
    public List<ProjectOption> Projects { get; set; } = new();
    public List<DivisionOption> Divisions { get; set; } = new();
}

public class ExpenseListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public Guid? DivisionId { get; set; }
    public string? DivisionName { get; set; }
    public string? DivisionAccent { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}