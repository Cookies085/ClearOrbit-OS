using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context) => _context = context;

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.TaskItems
            .Include(t => t.Project).ThenInclude(p => p.Division)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems
            .Include(t => t.Project).ThenInclude(p => p.Division)
            .Include(t => t.AssignedToUser)
            .Where(t => t.IsActive)
            .OrderBy(t => t.Status == Domain.Enums.WorkItemStatus.Done ? 1 : 0)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetByProjectAsync(Guid projectId)
    {
        return await _context.TaskItems
            .Include(t => t.Project).ThenInclude(p => p.Division)
            .Include(t => t.AssignedToUser)
            .Where(t => t.IsActive && t.ProjectId == projectId)
            .OrderBy(t => t.Status == Domain.Enums.WorkItemStatus.Done ? 1 : 0)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task AddAsync(TaskItem task) => await _context.TaskItems.AddAsync(task);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}