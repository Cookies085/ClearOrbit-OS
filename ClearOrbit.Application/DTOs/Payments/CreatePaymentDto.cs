using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Payments;

public class CreatePaymentDto
{
    public Guid InvoiceId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}