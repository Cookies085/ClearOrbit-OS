using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Interfaces;

public interface IReleaseService
{
    Task<Result<ReleaseResponseDto>> CreateAsync(CreateReleaseDto request);
    Task<Result<ReleaseResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ReleaseResponseDto>>> GetAllAsync();
    Task<Result<List<ReleaseResponseDto>>> GetByProjectAsync(Guid projectId);
    Task<Result<ReleaseResponseDto>> UpdateAsync(Guid id, UpdateReleaseDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}