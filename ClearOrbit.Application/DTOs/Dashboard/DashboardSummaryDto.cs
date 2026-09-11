namespace ClearOrbit.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    // Money
    public decimal TotalRevenue { get; set; }
    public decimal OutstandingReceivables { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Profit { get; set; }
    public string Currency { get; set; } = "ZAR";

    // Counts
    public int ClientCount { get; set; }
    public int TotalProjectCount { get; set; }
    public int ActiveProjectCount { get; set; }
    public int OpenTaskCount { get; set; }
    public int OverdueInvoiceCount { get; set; }

    // Recent activity
    public List<RecentProjectDto> RecentProjects { get; set; } = new();
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    public List<DivisionBreakdownDto> DivisionBreakdown { get; set; } = new();
}

public class RecentProjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;
    public string? ClientName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentInvoiceDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal BalanceDue { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string StatusName { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public DateTime IssueDate { get; set; }
}

public class DivisionBreakdownDto
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public int ProjectCount { get; set; }
}