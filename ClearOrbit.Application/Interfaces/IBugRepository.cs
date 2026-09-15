using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IBugRepository
{
    Task<Bug?> GetByIdAsync(Guid id);
    Task<List<Bug>> GetAllAsync();
    Task<List<Bug>> GetByProjectAsync(Guid projectId);
    Task<List<Bug>> GetByFeatureAsync(Guid featureId);
    Task<List<Bug>> GetByDivisionAsync(Guid divisionId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Bug bug);
    Task SaveChangesAsync();
}