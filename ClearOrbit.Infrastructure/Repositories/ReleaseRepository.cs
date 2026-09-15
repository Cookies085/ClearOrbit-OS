using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ReleaseRepository : IReleaseRepository
{
    private readonly AppDbContext _context;
    public ReleaseRepository(AppDbContext context) => _context = context;

    public async Task<Release?> GetByIdAsync(Guid id)
    {
        return await _context.Releases
            .Include(r => r.Division)
            .Include(r => r.Project)
            .Include(r => r.ReleaseManagerUser)
            .Include(r => r.Features)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Release>> GetAllAsync()
    {
        return await _context.Releases
            .Include(r => r.Division)
            .Include(r => r.Project)
            .Include(r => r.Features)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.PlannedDate)
            .ToListAsync();
    }

    public async Task<List<Release>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Releases
            .Include(r => r.Division)
            .Include(r => r.Features)
            .Where(r => r.ProjectId == projectId && r.IsActive)
            .OrderByDescending(r => r.PlannedDate)
            .ToListAsync();
    }

    public async Task<List<Release>> GetByDivisionAsync(Guid divisionId)
    {
        return await _context.Releases
            .Include(r => r.Project)
            .Include(r => r.Features)
            .Where(r => r.DivisionId == divisionId && r.IsActive)
            .OrderByDescending(r => r.PlannedDate)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Releases.CountAsync();
        return count + 1;
    }

    public async Task<bool> VersionExistsAsync(string version, Guid? projectId, Guid? excludeId = null)
    {
        var normalized = version.Trim().ToLowerInvariant();
        return await _context.Releases.AnyAsync(r =>
            r.Version.ToLower() == normalized &&
            r.ProjectId == projectId &&
            (excludeId == null || r.Id != excludeId.Value));
    }

    public async Task AddAsync(Release release) => await _context.Releases.AddAsync(release);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}