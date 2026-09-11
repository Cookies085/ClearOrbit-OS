using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;
    public EnrollmentRepository(AppDbContext context) => _context = context;

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments
            .Include(e => e.Learner)
            .Include(e => e.Class)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment?> GetByClassAndLearnerAsync(Guid classId, Guid learnerId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.ClassId == classId && e.LearnerId == learnerId);
    }

    public async Task<List<Enrollment>> GetByClassAsync(Guid classId)
    {
        return await _context.Enrollments
            .Include(e => e.Learner)
            .Where(e => e.ClassId == classId && e.IsActive)
            .OrderBy(e => e.Learner!.LastName).ThenBy(e => e.Learner!.FirstName)
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetByLearnerAsync(Guid learnerId)
    {
        return await _context.Enrollments
            .Include(e => e.Class).ThenInclude(c => c.Tutor)
            .Where(e => e.LearnerId == learnerId && e.IsActive)
            .ToListAsync();
    }

    public async Task<int> CountInClassAsync(Guid classId)
    {
        return await _context.Enrollments
            .CountAsync(e => e.ClassId == classId
                && e.IsActive
                && e.Status == EnrollmentStatus.Active);
    }

    public async Task AddAsync(Enrollment enrollment) => await _context.Enrollments.AddAsync(enrollment);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}