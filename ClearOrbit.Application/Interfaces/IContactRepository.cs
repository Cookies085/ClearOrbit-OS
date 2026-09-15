using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id);
    Task<List<Contact>> GetByClientAsync(Guid clientId);
    Task<List<Contact>> GetAllAsync();
    Task<bool> ExistsWithEmailInClientAsync(string email, Guid clientId, Guid? excludeId = null);
    Task AddAsync(Contact contact);
    Task SaveChangesAsync();
}