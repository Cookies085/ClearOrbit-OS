namespace ClearOrbit.Application.DTOs.Expenses;

public class ExpenseResponseDto
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