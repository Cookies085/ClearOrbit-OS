namespace ClearOrbit.Application.DTOs.Payments;

public class PaymentResponseDto
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