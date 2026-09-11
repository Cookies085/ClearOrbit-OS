using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<List<Project>> GetAllAsync();
    Task<int> GetNextSequenceForDivisionAsync(Guid divisionId);
    Task AddAsync(Project project);
    Task SaveChangesAsync();
}