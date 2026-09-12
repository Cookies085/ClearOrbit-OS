namespace ClearOrbit.Application.DTOs.Reports;

public class ReportsSummaryDto
{
    public string Currency { get; set; } = "ZAR";
    public decimal TotalRevenue { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal OutstandingReceivables { get; set; }
    public decimal OverdueReceivables { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int PaidInvoiceCount { get; set; }
    public int UnpaidInvoiceCount { get; set; }
    public int OverdueInvoiceCount { get; set; }
    public int TotalClients { get; set; }
    public int TotalProjects { get; set; }
}

public class DivisionProfitabilityDto
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string AccentColor { get; set; } = "#1E90FF";
    public int ProjectCount { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class ClientProfitabilityDto
{
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
    public int InvoiceCount { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Outstanding { get; set; }
    public decimal Overdue { get; set; }
    public DateTime? OldestUnpaidDate { get; set; }
    public int DaysOutstanding { get; set; }
}

public class ProjectProfitabilityDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = "#1E90FF";
    public string? ClientName { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public string Currency { get; set; } = "ZAR";
}