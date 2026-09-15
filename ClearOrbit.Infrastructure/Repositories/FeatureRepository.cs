using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly AppDbContext _context;
    public FeatureRepository(AppDbContext context) => _context = context;

    public async Task<Feature?> GetByIdAsync(Guid id)
    {
        return await _context.Features
            .Include(f => f.Division)
            .Include(f => f.Project)
            .Include(f => f.Client)
            .Include(f => f.AssignedToUser)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<Feature>> GetAllAsync()
    {
        return await _context.Features
            .Include(f => f.Division)
            .Include(f => f.Project)
            .Include(f => f.Client)
            .Include(f => f.AssignedToUser)
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.Priority)
            .ThenByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Feature>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Features
            .Include(f => f.Division)
            .Include(f => f.Project)
            .Include(f => f.Client)
            .Where(f => f.ProjectId == projectId && f.IsActive)
            .OrderByDescending(f => f.Priority)
            .ToListAsync();
    }

    public async Task<List<Feature>> GetByDivisionAsync(Guid divisionId)
    {
        return await _context.Features
            .Include(f => f.Division)
            .Include(f => f.Project)
            .Include(f => f.Client)
            .Where(f => f.DivisionId == divisionId && f.IsActive)
            .OrderByDescending(f => f.Priority)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Features.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Feature feature) => await _context.Features.AddAsync(feature);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}