using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly AppDbContext _context;
    public AssessmentRepository(AppDbContext context) => _context = context;

    public async Task<Assessment?> GetByIdAsync(Guid id)
    {
        return await _context.Assessments
            .Include(a => a.Class)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Assessment>> GetAllAsync()
    {
        return await _context.Assessments
            .Include(a => a.Class)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<Assessment>> GetByClassAsync(Guid classId)
    {
        return await _context.Assessments
            .Include(a => a.Class)
            .Where(a => a.ClassId == classId && a.IsActive)
            .OrderByDescending(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Assessments.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Assessment assessment)
        => await _context.Assessments.AddAsync(assessment);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}