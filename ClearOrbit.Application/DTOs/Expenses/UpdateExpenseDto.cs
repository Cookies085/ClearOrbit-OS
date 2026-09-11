using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Expenses;

public class UpdateExpenseDto
{
    public string Description { get; set; } = string.Empty;
    public ExpenseCategory Category { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? DivisionId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}