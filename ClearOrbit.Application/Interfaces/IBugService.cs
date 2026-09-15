using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Interfaces;

public interface IBugService
{
    Task<Result<BugResponseDto>> CreateAsync(CreateBugDto request, Guid? reportedByUserId);
    Task<Result<BugResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<BugResponseDto>>> GetAllAsync();
    Task<Result<List<BugResponseDto>>> GetByProjectAsync(Guid projectId);
    Task<Result<List<BugResponseDto>>> GetByFeatureAsync(Guid featureId);
    Task<Result<List<BugResponseDto>>> GetByDivisionAsync(Guid divisionId);
    Task<Result<BugResponseDto>> UpdateAsync(Guid id, UpdateBugDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}