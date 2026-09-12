using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IResultRepository
{
    Task<List<Result>> GetByAssessmentAsync(Guid assessmentId);
    Task<List<Result>> GetByLearnerAsync(Guid learnerId);
    Task AddRangeAsync(IEnumerable<Result> results);
    Task SaveChangesAsync();
}