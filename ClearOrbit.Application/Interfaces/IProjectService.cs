using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Projects;

namespace ClearOrbit.Application.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectResponseDto>> CreateAsync(CreateProjectDto request);
    Task<Result<ProjectResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ProjectResponseDto>>> GetAllAsync();
    Task<Result<ProjectResponseDto>> UpdateAsync(Guid id, UpdateProjectDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}