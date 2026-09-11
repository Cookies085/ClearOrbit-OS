using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context) => _context = context;

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        return await _context.Services
            .Include(s => s.Division)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Service>> GetAllAsync()
    {
        return await _context.Services
            .Include(s => s.Division)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Division.Name).ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Service>> GetByDivisionAsync(Guid divisionId)
    {
        return await _context.Services
            .Include(s => s.Division)
            .Where(s => s.DivisionId == divisionId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameInDivisionAsync(string name, Guid divisionId)
    {
        var normalized = name.Trim().ToLower();
        return await _context.Services
            .AnyAsync(s => s.DivisionId == divisionId && s.Name.ToLower() == normalized);
    }

    public async Task AddAsync(Service service) => await _context.Services.AddAsync(service);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}