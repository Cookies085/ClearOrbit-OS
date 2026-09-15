using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Interfaces;

public interface IFeatureService
{
    Task<Result<FeatureResponseDto>> CreateAsync(CreateFeatureDto request);
    Task<Result<FeatureResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<FeatureResponseDto>>> GetAllAsync();
    Task<Result<List<FeatureResponseDto>>> GetByProjectAsync(Guid projectId);
    Task<Result<List<FeatureResponseDto>>> GetByDivisionAsync(Guid divisionId);
    Task<Result<FeatureResponseDto>> UpdateAsync(Guid id, UpdateFeatureDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}