using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<List<Invoice>> GetAllAsync();
    Task<List<Invoice>> GetByClientAsync(Guid clientId);
    Task<List<Invoice>> GetByProjectAsync(Guid projectId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Invoice invoice);
    Task SaveChangesAsync();
}