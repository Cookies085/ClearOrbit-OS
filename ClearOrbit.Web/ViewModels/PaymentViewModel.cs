using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class PaymentViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid InvoiceId { get; set; }

    [Required(ErrorMessage = "Payment date is required.")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow.Date;

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public int Method { get; set; } = 1;

    public string? Reference { get; set; }
    public string? Notes { get; set; }

    // Display info
    public string? InvoiceCode { get; set; }
    public string? ClientName { get; set; }
    public decimal? BalanceDue { get; set; }
    public string Currency { get; set; } = "ZAR";
}

public class PaymentListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public int Method { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}