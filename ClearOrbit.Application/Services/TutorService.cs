using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Services;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _tutorRepo;

    public TutorService(ITutorRepository tutorRepo) => _tutorRepo = tutorRepo;

    public async Task<Result<TutorResponseDto>> CreateAsync(CreateTutorDto request)
    {
        var errors = TutorGuard.Validate(request);
        if (errors.Any()) return Result<TutorResponseDto>.Fail(errors);

        var sequence = await _tutorRepo.GetNextSequenceAsync();
        var code = TutorCodeGenerator.Generate(sequence);

        var tutor = new Tutor
        {
            Code = code,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email?.Trim(),
            Phone = request.Phone?.Trim(),
            Specializations = request.Specializations.Trim(),
            Bio = request.Bio,
            JoinedOn = DateTime.UtcNow
        };

        await _tutorRepo.AddAsync(tutor);
        await _tutorRepo.SaveChangesAsync();

        return Result<TutorResponseDto>.Ok(MapToDto(tutor), $"Tutor {code} added.");
    }

    public async Task<Result<TutorResponseDto>> GetByIdAsync(Guid id)
    {
        var tutor = await _tutorRepo.GetByIdAsync(id);
        if (tutor is null) return Result<TutorResponseDto>.Fail("Tutor not found.");
        return Result<TutorResponseDto>.Ok(MapToDto(tutor));
    }

    public async Task<Result<List<TutorResponseDto>>> GetAllAsync()
    {
        var tutors = await _tutorRepo.GetAllAsync();
        return Result<List<TutorResponseDto>>.Ok(tutors.Select(MapToDto).ToList());
    }

    public async Task<Result<TutorResponseDto>> UpdateAsync(Guid id, UpdateTutorDto request)
    {
        var errors = TutorGuard.Validate(request);
        if (errors.Any()) return Result<TutorResponseDto>.Fail(errors);

        var tutor = await _tutorRepo.GetByIdAsync(id);
        if (tutor is null) return Result<TutorResponseDto>.Fail("Tutor not found.");

        tutor.FirstName = request.FirstName.Trim();
        tutor.LastName = request.LastName.Trim();
        tutor.Email = request.Email?.Trim();
        tutor.Phone = request.Phone?.Trim();
        tutor.Specializations = request.Specializations.Trim();
        tutor.Bio = request.Bio;
        tutor.IsActive = request.IsActive;

        await _tutorRepo.SaveChangesAsync();
        return Result<TutorResponseDto>.Ok(MapToDto(tutor), "Tutor updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var tutor = await _tutorRepo.GetByIdAsync(id);
        if (tutor is null) return Result<bool>.Fail("Tutor not found.");

        tutor.IsActive = false;
        await _tutorRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Tutor deactivated.");
    }

    private static TutorResponseDto MapToDto(Tutor t) => new()
    {
        Id = t.Id,
        Code = t.Code,
        FirstName = t.FirstName,
        LastName = t.LastName,
        Email = t.Email,
        Phone = t.Phone,
        Specializations = t.Specializations,
        Bio = t.Bio,
        JoinedOn = t.JoinedOn,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };
}