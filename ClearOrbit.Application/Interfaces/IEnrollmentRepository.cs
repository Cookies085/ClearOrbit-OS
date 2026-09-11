using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<Enrollment?> GetByClassAndLearnerAsync(Guid classId, Guid learnerId);
    Task<List<Enrollment>> GetByClassAsync(Guid classId);
    Task<List<Enrollment>> GetByLearnerAsync(Guid learnerId);
    Task<int> CountInClassAsync(Guid classId);
    Task AddAsync(Enrollment enrollment);
    Task SaveChangesAsync();
}