using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context) => _context = context;

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Division)
            .Include(p => p.Service)
            .Include(p => p.RequestingDivision)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Division)
            .Include(p => p.Service)
            .Include(p => p.RequestingDivision)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceForDivisionAsync(Guid divisionId)
    {
        var count = await _context.Projects
            .Where(p => p.DivisionId == divisionId)
            .CountAsync();

        return count + 1;
    }

    public async Task AddAsync(Project project) => await _context.Projects.AddAsync(project);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}