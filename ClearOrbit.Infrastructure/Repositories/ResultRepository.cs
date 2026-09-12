using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly AppDbContext _context;
    public ResultRepository(AppDbContext context) => _context = context;

    public async Task<List<Result>> GetByAssessmentAsync(Guid assessmentId)
    {
        return await _context.Results
            .Include(r => r.Learner)
            .Where(r => r.AssessmentId == assessmentId && r.IsActive)
            .OrderBy(r => r.Learner!.LastName).ThenBy(r => r.Learner!.FirstName)
            .ToListAsync();
    }

    public async Task<List<Result>> GetByLearnerAsync(Guid learnerId)
    {
        return await _context.Results
            .Include(r => r.Assessment).ThenInclude(a => a.Class)
            .Where(r => r.LearnerId == learnerId && r.IsActive)
            .OrderByDescending(r => r.Assessment!.ScheduledDate)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Result> results)
        => await _context.Results.AddRangeAsync(results);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}