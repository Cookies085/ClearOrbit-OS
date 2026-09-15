using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IFeatureRepository
{
    Task<Feature?> GetByIdAsync(Guid id);
    Task<List<Feature>> GetAllAsync();
    Task<List<Feature>> GetByProjectAsync(Guid projectId);
    Task<List<Feature>> GetByDivisionAsync(Guid divisionId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Feature feature);
    Task SaveChangesAsync();
}