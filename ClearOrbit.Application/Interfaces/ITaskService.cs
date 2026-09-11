using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Tasks;

namespace ClearOrbit.Application.Interfaces;

public interface ITaskService
{
    Task<Result<TaskResponseDto>> CreateAsync(CreateTaskDto request);
    Task<Result<TaskResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<TaskResponseDto>>> GetAllAsync();
    Task<Result<List<TaskResponseDto>>> GetByProjectAsync(Guid projectId);
    Task<Result<TaskResponseDto>> UpdateAsync(Guid id, UpdateTaskDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}