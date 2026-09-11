using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetAllAsync();
    Task<List<TaskItem>> GetByProjectAsync(Guid projectId);
    Task AddAsync(TaskItem task);
    Task SaveChangesAsync();
}