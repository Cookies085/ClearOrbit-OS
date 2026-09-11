using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class DivisionRepository : IDivisionRepository
{
    private readonly AppDbContext _context;

    public DivisionRepository(AppDbContext context) => _context = context;

    public async Task<List<Division>> GetAllAsync()
    {
        return await _context.Divisions
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Division?> GetByIdAsync(Guid id)
    {
        return await _context.Divisions.FirstOrDefaultAsync(d => d.Id == id);
    }
}