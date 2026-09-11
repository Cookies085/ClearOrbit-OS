using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface ILearnerRepository
{
    Task<Learner?> GetByIdAsync(Guid id);
    Task<List<Learner>> GetAllAsync();
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Learner learner);
    Task SaveChangesAsync();
}