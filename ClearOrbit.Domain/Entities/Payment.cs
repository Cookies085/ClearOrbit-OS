using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Payment : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-PAY-00001

    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";

    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string? Reference { get; set; }             // bank ref, receipt no
    public string? Notes { get; set; }
}