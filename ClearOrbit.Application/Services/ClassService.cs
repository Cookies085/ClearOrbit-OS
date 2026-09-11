using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;
    private readonly ITutorRepository _tutorRepo;
    private readonly ILearnerRepository _learnerRepo;

    public ClassService(
        IClassRepository classRepo,
        IEnrollmentRepository enrollmentRepo,
        ITutorRepository tutorRepo,
        ILearnerRepository learnerRepo)
    {
        _classRepo = classRepo;
        _enrollmentRepo = enrollmentRepo;
        _tutorRepo = tutorRepo;
        _learnerRepo = learnerRepo;
    }

    public async Task<Result<ClassResponseDto>> CreateAsync(CreateClassDto request)
    {
        var errors = ClassGuard.Validate(request);
        if (errors.Any()) return Result<ClassResponseDto>.Fail(errors);

        var tutor = await _tutorRepo.GetByIdAsync(request.TutorId);
        if (tutor is null) return Result<ClassResponseDto>.Fail("Tutor not found.");

        var sequence = await _classRepo.GetNextSequenceAsync();
        var code = ClassCodeGenerator.Generate(sequence);

        var cls = new Class
        {
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            Subject = request.Subject.Trim(),
            GradeLevel = request.GradeLevel,
            TutorId = request.TutorId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location?.Trim(),
            MaxCapacity = request.MaxCapacity,
            FeePerLearner = request.FeePerLearner,
            Currency = request.Currency.ToUpperInvariant(),
            Status = ClassStatus.Draft
        };

        await _classRepo.AddAsync(cls);
        await _classRepo.SaveChangesAsync();

        return Result<ClassResponseDto>.Ok(MapToDto(cls, tutor, 0), $"Class {code} created.");
    }

    public async Task<Result<ClassResponseDto>> GetByIdAsync(Guid id)
    {
        var cls = await _classRepo.GetByIdAsync(id);
        if (cls is null) return Result<ClassResponseDto>.Fail("Class not found.");

        var count = await _enrollmentRepo.CountInClassAsync(id);
        return Result<ClassResponseDto>.Ok(MapToDto(cls, cls.Tutor, count));
    }

    public async Task<Result<List<ClassResponseDto>>> GetAllAsync()
    {
        var classes = await _classRepo.GetAllAsync();
        var dtos = new List<ClassResponseDto>();
        foreach (var c in classes)
        {
            var count = await _enrollmentRepo.CountInClassAsync(c.Id);
            dtos.Add(MapToDto(c, c.Tutor, count));
        }
        return Result<List<ClassResponseDto>>.Ok(dtos);
    }

    public async Task<Result<ClassResponseDto>> UpdateAsync(Guid id, UpdateClassDto request)
    {
        var errors = ClassGuard.Validate(request);
        if (errors.Any()) return Result<ClassResponseDto>.Fail(errors);

        var cls = await _classRepo.GetByIdAsync(id);
        if (cls is null) return Result<ClassResponseDto>.Fail("Class not found.");

        var tutor = await _tutorRepo.GetByIdAsync(request.TutorId);
        if (tutor is null) return Result<ClassResponseDto>.Fail("Tutor not found.");

        var enrolledCount = await _enrollmentRepo.CountInClassAsync(id);
        if (request.MaxCapacity < enrolledCount)
            return Result<ClassResponseDto>.Fail(
                $"Max capacity cannot be less than enrolled learners ({enrolledCount}).");

        cls.Name = request.Name.Trim();
        cls.Description = request.Description;
        cls.Subject = request.Subject.Trim();
        cls.GradeLevel = request.GradeLevel;
        cls.TutorId = request.TutorId;
        cls.DayOfWeek = request.DayOfWeek;
        cls.StartTime = request.StartTime;
        cls.EndTime = request.EndTime;
        cls.StartDate = request.StartDate;
        cls.EndDate = request.EndDate;
        cls.Location = request.Location?.Trim();
        cls.MaxCapacity = request.MaxCapacity;
        cls.FeePerLearner = request.FeePerLearner;
        cls.Currency = request.Currency.ToUpperInvariant();
        cls.Status = request.Status;

        await _classRepo.SaveChangesAsync();

        return Result<ClassResponseDto>.Ok(MapToDto(cls, tutor, enrolledCount), "Class updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var cls = await _classRepo.GetByIdAsync(id);
        if (cls is null) return Result<bool>.Fail("Class not found.");

        cls.IsActive = false;
        cls.Status = ClassStatus.Cancelled;
        await _classRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, "Class cancelled.");
    }

    public async Task<Result<List<EnrollmentResponseDto>>> GetEnrollmentsAsync(Guid classId)
    {
        var enrollments = await _enrollmentRepo.GetByClassAsync(classId);
        return Result<List<EnrollmentResponseDto>>.Ok(enrollments.Select(MapEnrollment).ToList());
    }

    public async Task<Result<EnrollmentResponseDto>> EnrollLearnerAsync(CreateEnrollmentDto request)
    {
        var cls = await _classRepo.GetByIdAsync(request.ClassId);
        if (cls is null) return Result<EnrollmentResponseDto>.Fail("Class not found.");

        if (cls.Status == ClassStatus.Cancelled || cls.Status == ClassStatus.Completed)
            return Result<EnrollmentResponseDto>.Fail("Cannot enroll in a cancelled or completed class.");

        var learner = await _learnerRepo.GetByIdAsync(request.LearnerId);
        if (learner is null) return Result<EnrollmentResponseDto>.Fail("Learner not found.");

        var existing = await _enrollmentRepo.GetByClassAndLearnerAsync(request.ClassId, request.LearnerId);
        if (existing is not null)
            return Result<EnrollmentResponseDto>.Fail("Learner is already enrolled in this class.");

        var currentCount = await _enrollmentRepo.CountInClassAsync(request.ClassId);
        var capacityErrors = EnrollmentGuard.ValidateCapacity(currentCount, cls.MaxCapacity);
        if (capacityErrors.Any()) return Result<EnrollmentResponseDto>.Fail(capacityErrors);

        var enrollment = new Enrollment
        {
            ClassId = request.ClassId,
            LearnerId = request.LearnerId,
            EnrolledOn = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            Notes = request.Notes
        };

        await _enrollmentRepo.AddAsync(enrollment);
        await _enrollmentRepo.SaveChangesAsync();

        // Reload to get learner navigation
        var reloaded = await _enrollmentRepo.GetByIdAsync(enrollment.Id);
        return Result<EnrollmentResponseDto>.Ok(MapEnrollment(reloaded!), "Learner enrolled.");
    }

    public async Task<Result<bool>> WithdrawLearnerAsync(Guid enrollmentId)
    {
        var enrollment = await _enrollmentRepo.GetByIdAsync(enrollmentId);
        if (enrollment is null) return Result<bool>.Fail("Enrollment not found.");

        enrollment.Status = EnrollmentStatus.Withdrawn;
        await _enrollmentRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Learner withdrawn.");
    }

    private static ClassResponseDto MapToDto(Class c, Tutor? tutor, int enrolledCount) => new()
    {
        Id = c.Id,
        Code = c.Code,
        Name = c.Name,
        Description = c.Description,
        Subject = c.Subject,
        GradeLevel = (int)c.GradeLevel,
        GradeLevelName = FormatGrade(c.GradeLevel),
        TutorId = c.TutorId,
        TutorName = tutor is null ? "—" : $"{tutor.FirstName} {tutor.LastName}",
        DayOfWeek = (int)c.DayOfWeek,
        DayOfWeekName = c.DayOfWeek.ToString(),
        StartTime = c.StartTime.ToString("HH\\:mm"),
        EndTime = c.EndTime.ToString("HH\\:mm"),
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        Location = c.Location,
        MaxCapacity = c.MaxCapacity,
        EnrolledCount = enrolledCount,
        AvailableSeats = Math.Max(0, c.MaxCapacity - enrolledCount),
        FeePerLearner = c.FeePerLearner,
        Currency = c.Currency,
        Status = (int)c.Status,
        StatusName = c.Status.ToString(),
        CreatedAt = c.CreatedAt
    };

    private static EnrollmentResponseDto MapEnrollment(Enrollment e) => new()
    {
        Id = e.Id,
        ClassId = e.ClassId,
        LearnerId = e.LearnerId,
        LearnerCode = e.Learner?.Code ?? string.Empty,
        LearnerName = e.Learner is null ? "—" : $"{e.Learner.FirstName} {e.Learner.LastName}",
        LearnerEmail = e.Learner?.Email,
        EnrolledOn = e.EnrolledOn,
        Status = (int)e.Status,
        StatusName = e.Status.ToString(),
        Notes = e.Notes
    };

    private static string FormatGrade(GradeLevel level) => level switch
    {
        GradeLevel.NotApplicable => "—",
        GradeLevel.GradeR => "Grade R",
        GradeLevel.AdultLearner => "Adult Learner",
        _ => $"Grade {(int)level - 1}"
    };
}