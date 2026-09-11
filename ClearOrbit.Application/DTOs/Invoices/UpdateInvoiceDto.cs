namespace ClearOrbit.Application.DTOs.Invoices;

public class UpdateInvoiceDto
{
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}