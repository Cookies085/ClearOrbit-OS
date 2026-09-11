using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<List<Class>> GetAllAsync();
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Class cls);
    Task SaveChangesAsync();
}