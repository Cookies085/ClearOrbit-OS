using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Invoices;

namespace ClearOrbit.Application.Interfaces;

public interface IInvoiceService
{
    Task<Result<InvoiceResponseDto>> CreateAsync(CreateInvoiceDto request);
    Task<Result<InvoiceResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<InvoiceResponseDto>>> GetAllAsync();
    Task<Result<InvoiceResponseDto>> UpdateAsync(Guid id, UpdateInvoiceDto request);
    Task<Result<InvoiceResponseDto>> MarkAsSentAsync(Guid id);
    Task<Result<InvoiceResponseDto>> CancelAsync(Guid id);
    Task<Result<bool>> DeleteAsync(Guid id);
}