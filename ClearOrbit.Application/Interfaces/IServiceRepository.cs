using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id);
    Task<List<Service>> GetAllAsync();
    Task<List<Service>> GetByDivisionAsync(Guid divisionId);
    Task<bool> ExistsByNameInDivisionAsync(string name, Guid divisionId);
    Task AddAsync(Service service);
    Task SaveChangesAsync();
}