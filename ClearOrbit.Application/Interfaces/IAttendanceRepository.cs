using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IAttendanceRepository
{
    Task<List<AttendanceRecord>> GetByClassAndDateAsync(Guid classId, DateTime date);
    Task<List<AttendanceRecord>> GetByClassAsync(Guid classId);
    Task<List<AttendanceRecord>> GetByLearnerAsync(Guid learnerId);
    Task AddRangeAsync(IEnumerable<AttendanceRecord> records);
    Task SaveChangesAsync();
}