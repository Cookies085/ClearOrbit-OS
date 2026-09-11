using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Interfaces;

public interface ITutorService
{
    Task<Result<TutorResponseDto>> CreateAsync(CreateTutorDto request);
    Task<Result<TutorResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<TutorResponseDto>>> GetAllAsync();
    Task<Result<TutorResponseDto>> UpdateAsync(Guid id, UpdateTutorDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}