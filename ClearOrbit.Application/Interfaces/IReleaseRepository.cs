using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IReleaseRepository
{
    Task<Release?> GetByIdAsync(Guid id);
    Task<List<Release>> GetAllAsync();
    Task<List<Release>> GetByProjectAsync(Guid projectId);
    Task<List<Release>> GetByDivisionAsync(Guid divisionId);
    Task<int> GetNextSequenceAsync();
    Task<bool> VersionExistsAsync(string version, Guid? projectId, Guid? excludeId = null);
    Task AddAsync(Release release);
    Task SaveChangesAsync();
}