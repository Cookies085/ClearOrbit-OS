using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly AppDbContext _context;
    public ClassRepository(AppDbContext context) => _context = context;

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _context.Classes
            .Include(c => c.Tutor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Tutor)
            .Where(c => c.IsActive)
            .OrderBy(c => c.DayOfWeek).ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Classes.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Class cls) => await _context.Classes.AddAsync(cls);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}