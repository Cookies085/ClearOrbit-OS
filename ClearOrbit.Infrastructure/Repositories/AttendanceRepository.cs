using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;
    public AttendanceRepository(AppDbContext context) => _context = context;

    public async Task<List<AttendanceRecord>> GetByClassAndDateAsync(Guid classId, DateTime date)
    {
        return await _context.AttendanceRecords
            .Include(a => a.Learner)
            .Where(a => a.ClassId == classId && a.SessionDate.Date == date.Date && a.IsActive)
            .ToListAsync();
    }

    public async Task<List<AttendanceRecord>> GetByClassAsync(Guid classId)
    {
        return await _context.AttendanceRecords
            .Include(a => a.Learner)
            .Where(a => a.ClassId == classId && a.IsActive)
            .OrderByDescending(a => a.SessionDate)
            .ToListAsync();
    }

    public async Task<List<AttendanceRecord>> GetByLearnerAsync(Guid learnerId)
    {
        return await _context.AttendanceRecords
            .Include(a => a.Class)
            .Where(a => a.LearnerId == learnerId && a.IsActive)
            .OrderByDescending(a => a.SessionDate)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<AttendanceRecord> records)
        => await _context.AttendanceRecords.AddRangeAsync(records);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}