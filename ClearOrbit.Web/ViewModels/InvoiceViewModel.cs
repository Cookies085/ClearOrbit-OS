using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class InvoiceViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Please select a client.")]
    public Guid ClientId { get; set; }

    public Guid? ProjectId { get; set; }

    [Required(ErrorMessage = "Please select a division.")]
    public Guid DivisionId { get; set; }

    [DataType(DataType.Date)]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow.Date;

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date.AddDays(30);

    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "ZAR";

    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 0;

    public string? Notes { get; set; }

    public List<InvoiceItemViewModel> Items { get; set; } = new();

    // Dropdowns
    public List<ClientOption> Clients { get; set; } = new();
    public List<ProjectOption> Projects { get; set; } = new();
    public List<DivisionOption> Divisions { get; set; } = new();
}

public class InvoiceItemViewModel
{
    public Guid Id { get; set; }           
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }   
}

public class InvoiceListItem
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
    public List<InvoiceItemViewModel> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}