using ClearOrbit.Application.DTOs.Payments;

namespace ClearOrbit.Application.Guards;

public static class PaymentGuard
{
    public static List<string> Validate(CreatePaymentDto request, decimal balanceDue)
    {
        var errors = new List<string>();

        if (request.InvoiceId == Guid.Empty)
            errors.Add("An invoice is required.");

        if (request.Amount <= 0)
            errors.Add("Payment amount must be greater than zero.");

        if (balanceDue <= 0)
            errors.Add("This invoice has no balance due.");
        else if (request.Amount > balanceDue)
            errors.Add($"Payment amount exceeds the outstanding balance of {balanceDue:N2}.");

        if (request.PaymentDate.Date > DateTime.UtcNow.Date.AddDays(1))
            errors.Add("Payment date cannot be in the future.");

        return errors;
    }
}