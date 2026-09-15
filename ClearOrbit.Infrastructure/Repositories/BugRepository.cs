using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class BugRepository : IBugRepository
{
    private readonly AppDbContext _context;
    public BugRepository(AppDbContext context) => _context = context;

    public async Task<Bug?> GetByIdAsync(Guid id)
    {
        return await _context.Bugs
            .Include(b => b.Division)
            .Include(b => b.Project)
            .Include(b => b.Feature)
            .Include(b => b.AssignedToUser)
            .Include(b => b.ReportedByUser)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Bug>> GetAllAsync()
    {
        return await _context.Bugs
            .Include(b => b.Division)
            .Include(b => b.Project)
            .Include(b => b.Feature)
            .Include(b => b.AssignedToUser)
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.Severity)
            .ThenByDescending(b => b.ReportedAt)
            .ToListAsync();
    }

    public async Task<List<Bug>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Bugs
            .Include(b => b.Division)
            .Include(b => b.Feature)
            .Where(b => b.ProjectId == projectId && b.IsActive)
            .OrderByDescending(b => b.Severity)
            .ToListAsync();
    }

    public async Task<List<Bug>> GetByFeatureAsync(Guid featureId)
    {
        return await _context.Bugs
            .Include(b => b.Division)
            .Where(b => b.FeatureId == featureId && b.IsActive)
            .OrderByDescending(b => b.Severity)
            .ToListAsync();
    }

    public async Task<List<Bug>> GetByDivisionAsync(Guid divisionId)
    {
        return await _context.Bugs
            .Include(b => b.Division)
            .Include(b => b.Project)
            .Where(b => b.DivisionId == divisionId && b.IsActive)
            .OrderByDescending(b => b.Severity)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Bugs.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Bug bug) => await _context.Bugs.AddAsync(bug);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}