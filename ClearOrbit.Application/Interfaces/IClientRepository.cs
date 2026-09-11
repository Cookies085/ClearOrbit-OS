using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<List<Client>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name);
    Task AddAsync(Client client);
    Task SaveChangesAsync();
}