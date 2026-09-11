using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Expense : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-EXP-00001
    public string Description { get; set; } = string.Empty;

    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;

    // Optional: tied to a project and/or division
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid? DivisionId { get; set; }
    public Division? Division { get; set; }

    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";

    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}