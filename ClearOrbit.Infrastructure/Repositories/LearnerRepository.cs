using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class LearnerRepository : ILearnerRepository
{
    private readonly AppDbContext _context;

    public LearnerRepository(AppDbContext context) => _context = context;

    public async Task<Learner?> GetByIdAsync(Guid id)
    {
        return await _context.Learners.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Learner>> GetAllAsync()
    {
        return await _context.Learners
            .Where(l => l.IsActive)
            .OrderBy(l => l.LastName).ThenBy(l => l.FirstName)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Learners.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Learner learner) => await _context.Learners.AddAsync(learner);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}