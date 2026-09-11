using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<List<Payment>> GetAllAsync();
    Task<List<Payment>> GetByInvoiceAsync(Guid invoiceId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Payment payment);
    void Remove(Payment payment);
    Task SaveChangesAsync();
}