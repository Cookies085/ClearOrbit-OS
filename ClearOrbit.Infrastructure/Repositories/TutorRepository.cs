using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly AppDbContext _context;

    public TutorRepository(AppDbContext context) => _context = context;

    public async Task<Tutor?> GetByIdAsync(Guid id)
    {
        return await _context.Tutors.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tutor>> GetAllAsync()
    {
        return await _context.Tutors
            .Where(t => t.IsActive)
            .OrderBy(t => t.LastName).ThenBy(t => t.FirstName)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Tutors.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Tutor tutor) => await _context.Tutors.AddAsync(tutor);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}