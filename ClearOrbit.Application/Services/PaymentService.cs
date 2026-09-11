using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Payments;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IInvoiceRepository _invoiceRepo;

    public PaymentService(IPaymentRepository paymentRepo, IInvoiceRepository invoiceRepo)
    {
        _paymentRepo = paymentRepo;
        _invoiceRepo = invoiceRepo;
    }

    public async Task<Result<PaymentResponseDto>> CreateAsync(CreatePaymentDto request)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(request.InvoiceId);
        if (invoice is null) return Result<PaymentResponseDto>.Fail("Invoice not found.");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result<PaymentResponseDto>.Fail("Cannot pay a cancelled invoice.");

        if (invoice.Status == InvoiceStatus.Draft)
            return Result<PaymentResponseDto>.Fail("Mark the invoice as Sent before recording payments.");

        var balanceDue = invoice.Total - invoice.AmountPaid;
        var errors = PaymentGuard.Validate(request, balanceDue);
        if (errors.Any()) return Result<PaymentResponseDto>.Fail(errors);

        var sequence = await _paymentRepo.GetNextSequenceAsync();
        var code = PaymentCodeGenerator.Generate(sequence);

        var payment = new Payment
        {
            Code = code,
            InvoiceId = request.InvoiceId,
            PaymentDate = request.PaymentDate,
            Amount = request.Amount,
            Currency = invoice.Currency,
            Method = request.Method,
            Reference = request.Reference?.Trim(),
            Notes = request.Notes
        };

        await _paymentRepo.AddAsync(payment);

        // Update invoice totals & status
        invoice.AmountPaid += request.Amount;
        UpdateInvoiceStatus(invoice);

        await _paymentRepo.SaveChangesAsync();

        return Result<PaymentResponseDto>.Ok(MapToDto(payment, invoice), $"Payment {code} recorded.");
    }

    public async Task<Result<PaymentResponseDto>> GetByIdAsync(Guid id)
    {
        var payment = await _paymentRepo.GetByIdAsync(id);
        if (payment is null) return Result<PaymentResponseDto>.Fail("Payment not found.");
        return Result<PaymentResponseDto>.Ok(MapToDto(payment, payment.Invoice));
    }

    public async Task<Result<List<PaymentResponseDto>>> GetAllAsync()
    {
        var payments = await _paymentRepo.GetAllAsync();
        return Result<List<PaymentResponseDto>>.Ok(payments
            .Select(p => MapToDto(p, p.Invoice))
            .ToList());
    }

    public async Task<Result<List<PaymentResponseDto>>> GetByInvoiceAsync(Guid invoiceId)
    {
        var payments = await _paymentRepo.GetByInvoiceAsync(invoiceId);
        return Result<List<PaymentResponseDto>>.Ok(payments
            .Select(p => MapToDto(p, p.Invoice))
            .ToList());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var payment = await _paymentRepo.GetByIdAsync(id);
        if (payment is null) return Result<bool>.Fail("Payment not found.");

        var invoice = await _invoiceRepo.GetByIdAsync(payment.InvoiceId);
        if (invoice is not null)
        {
            invoice.AmountPaid = Math.Max(0, invoice.AmountPaid - payment.Amount);
            UpdateInvoiceStatus(invoice);
        }

        payment.IsActive = false;
        await _paymentRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, "Payment reversed.");
    }

    private static void UpdateInvoiceStatus(Invoice invoice)
    {
        if (invoice.Status == InvoiceStatus.Cancelled) return;

        if (invoice.AmountPaid >= invoice.Total)
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt ??= DateTime.UtcNow;
        }
        else if (invoice.AmountPaid > 0)
        {
            invoice.Status = InvoiceStatus.PartiallyPaid;
            invoice.PaidAt = null;
        }
        else if (invoice.SentAt.HasValue)
        {
            invoice.Status = InvoiceStatus.Sent;
            invoice.PaidAt = null;
        }
        else
        {
            invoice.Status = InvoiceStatus.Draft;
            invoice.PaidAt = null;
        }
    }

    private static PaymentResponseDto MapToDto(Payment p, Invoice? invoice) => new()
    {
        Id = p.Id,
        Code = p.Code,
        InvoiceId = p.InvoiceId,
        InvoiceCode = invoice?.Code ?? string.Empty,
        ClientName = invoice?.Client?.Name ?? string.Empty,
        PaymentDate = p.PaymentDate,
        Amount = p.Amount,
        Currency = p.Currency,
        Method = (int)p.Method,
        MethodName = p.Method.ToString(),
        Reference = p.Reference,
        Notes = p.Notes,
        CreatedAt = p.CreatedAt
    };
}