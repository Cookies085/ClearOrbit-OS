using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class LearnerService : ILearnerService
{
    private readonly ILearnerRepository _learnerRepo;

    public LearnerService(ILearnerRepository learnerRepo) => _learnerRepo = learnerRepo;

    public async Task<Result<LearnerResponseDto>> CreateAsync(CreateLearnerDto request)
    {
        var errors = LearnerGuard.Validate(request);
        if (errors.Any()) return Result<LearnerResponseDto>.Fail(errors);

        var sequence = await _learnerRepo.GetNextSequenceAsync();
        var code = LearnerCodeGenerator.Generate(sequence);

        var learner = new Learner
        {
            Code = code,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            GradeLevel = request.GradeLevel,
            Status = LearnerStatus.Active,
            Email = request.Email?.Trim(),
            Phone = request.Phone?.Trim(),
            Address = request.Address?.Trim(),
            GuardianName = request.GuardianName?.Trim(),
            GuardianPhone = request.GuardianPhone?.Trim(),
            GuardianEmail = request.GuardianEmail?.Trim(),
            Notes = request.Notes,
            EnrolledOn = DateTime.UtcNow
        };

        await _learnerRepo.AddAsync(learner);
        await _learnerRepo.SaveChangesAsync();

        return Result<LearnerResponseDto>.Ok(MapToDto(learner), $"Learner {code} enrolled.");
    }

    public async Task<Result<LearnerResponseDto>> GetByIdAsync(Guid id)
    {
        var learner = await _learnerRepo.GetByIdAsync(id);
        if (learner is null) return Result<LearnerResponseDto>.Fail("Learner not found.");
        return Result<LearnerResponseDto>.Ok(MapToDto(learner));
    }

    public async Task<Result<List<LearnerResponseDto>>> GetAllAsync()
    {
        var learners = await _learnerRepo.GetAllAsync();
        return Result<List<LearnerResponseDto>>.Ok(learners.Select(MapToDto).ToList());
    }

    public async Task<Result<LearnerResponseDto>> UpdateAsync(Guid id, UpdateLearnerDto request)
    {
        var errors = LearnerGuard.Validate(request);
        if (errors.Any()) return Result<LearnerResponseDto>.Fail(errors);

        var learner = await _learnerRepo.GetByIdAsync(id);
        if (learner is null) return Result<LearnerResponseDto>.Fail("Learner not found.");

        learner.FirstName = request.FirstName.Trim();
        learner.LastName = request.LastName.Trim();
        learner.DateOfBirth = request.DateOfBirth;
        learner.GradeLevel = request.GradeLevel;
        learner.Status = request.Status;
        learner.Email = request.Email?.Trim();
        learner.Phone = request.Phone?.Trim();
        learner.Address = request.Address?.Trim();
        learner.GuardianName = request.GuardianName?.Trim();
        learner.GuardianPhone = request.GuardianPhone?.Trim();
        learner.GuardianEmail = request.GuardianEmail?.Trim();
        learner.Notes = request.Notes;

        await _learnerRepo.SaveChangesAsync();
        return Result<LearnerResponseDto>.Ok(MapToDto(learner), "Learner updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var learner = await _learnerRepo.GetByIdAsync(id);
        if (learner is null) return Result<bool>.Fail("Learner not found.");

        learner.IsActive = false;
        await _learnerRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Learner deactivated.");
    }

    private static LearnerResponseDto MapToDto(Learner l) => new()
    {
        Id = l.Id,
        Code = l.Code,
        FirstName = l.FirstName,
        LastName = l.LastName,
        DateOfBirth = l.DateOfBirth,
        Age = l.DateOfBirth.HasValue
            ? (int)((DateTime.UtcNow - l.DateOfBirth.Value).TotalDays / 365.25)
            : 0,
        GradeLevel = (int)l.GradeLevel,
        GradeLevelName = FormatGrade(l.GradeLevel),
        Status = (int)l.Status,
        StatusName = l.Status.ToString(),
        Email = l.Email,
        Phone = l.Phone,
        Address = l.Address,
        GuardianName = l.GuardianName,
        GuardianPhone = l.GuardianPhone,
        GuardianEmail = l.GuardianEmail,
        Notes = l.Notes,
        EnrolledOn = l.EnrolledOn,
        CreatedAt = l.CreatedAt
    };

    private static string FormatGrade(GradeLevel level) => level switch
    {
        GradeLevel.NotApplicable => "—",
        GradeLevel.GradeR => "Grade R",
        GradeLevel.AdultLearner => "Adult Learner",
        _ => $"Grade {(int)level - 1}"
    };
}