using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(Guid id);
    Task<List<Assessment>> GetAllAsync();
    Task<List<Assessment>> GetByClassAsync(Guid classId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Assessment assessment);
    Task SaveChangesAsync();
}