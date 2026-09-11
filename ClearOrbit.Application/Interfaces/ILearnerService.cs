using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Interfaces;

public interface ILearnerService
{
    Task<Result<LearnerResponseDto>> CreateAsync(CreateLearnerDto request);
    Task<Result<LearnerResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<LearnerResponseDto>>> GetAllAsync();
    Task<Result<LearnerResponseDto>> UpdateAsync(Guid id, UpdateLearnerDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}