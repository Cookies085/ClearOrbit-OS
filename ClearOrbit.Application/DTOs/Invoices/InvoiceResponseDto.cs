namespace ClearOrbit.Application.DTOs.Invoices;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;

    public Guid? ProjectId { get; set; }
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }

    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string Currency { get; set; } = "ZAR";

    public string? Notes { get; set; }
    public bool IsOverdue { get; set; }

    public List<InvoiceItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}