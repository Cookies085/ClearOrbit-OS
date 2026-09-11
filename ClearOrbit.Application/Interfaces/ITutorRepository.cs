using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface ITutorRepository
{
    Task<Tutor?> GetByIdAsync(Guid id);
    Task<List<Tutor>> GetAllAsync();
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Tutor tutor);
    Task SaveChangesAsync();
}