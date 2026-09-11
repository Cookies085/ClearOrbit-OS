using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Payments;

namespace ClearOrbit.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentResponseDto>> CreateAsync(CreatePaymentDto request);
    Task<Result<PaymentResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<PaymentResponseDto>>> GetAllAsync();
    Task<Result<List<PaymentResponseDto>>> GetByInvoiceAsync(Guid invoiceId);
    Task<Result<bool>> DeleteAsync(Guid id);
}