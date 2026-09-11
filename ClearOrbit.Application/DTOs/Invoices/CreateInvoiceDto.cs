namespace ClearOrbit.Application.DTOs.Invoices;

public class CreateInvoiceDto
{
    public Guid ClientId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid DivisionId { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public string Currency { get; set; } = "ZAR";
    public decimal TaxRate { get; set; } = 0;
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}