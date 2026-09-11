using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Invoice : BaseEntity
{
    public string Code { get; set; } = string.Empty; // CO-INV-00001
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    // Who and from where
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    // Dates
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public DateTime? SentAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Money (stored, computed from items on save)
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }      // e.g., 15 for 15%
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "ZAR";

    public string? Notes { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    // Computed helper (not mapped)
    public decimal BalanceDue => Total - AmountPaid;
}

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }  // Quantity * UnitPrice
}