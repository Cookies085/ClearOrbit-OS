using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Tasks;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepo;
    private readonly IProjectRepository _projectRepo;

    public TaskService(ITaskRepository taskRepo, IProjectRepository projectRepo)
    {
        _taskRepo = taskRepo;
        _projectRepo = projectRepo;
    }

    public async Task<Result<TaskResponseDto>> CreateAsync(CreateTaskDto request)
    {
        var errors = TaskGuard.Validate(request);
        if (errors.Any()) return Result<TaskResponseDto>.Fail(errors);

        var project = await _projectRepo.GetByIdAsync(request.ProjectId);
        if (project is null) return Result<TaskResponseDto>.Fail("Project not found.");

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description,
            Priority = request.Priority,
            Status = WorkItemStatus.ToDo,
            ProjectId = request.ProjectId,
            AssignedToUserId = request.AssignedToUserId,
            DueDate = request.DueDate
        };

        await _taskRepo.AddAsync(task);
        await _taskRepo.SaveChangesAsync();

        return Result<TaskResponseDto>.Ok(MapToDto(task, project), "Task created.");
    }

    public async Task<Result<TaskResponseDto>> GetByIdAsync(Guid id)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task is null) return Result<TaskResponseDto>.Fail("Task not found.");
        return Result<TaskResponseDto>.Ok(MapToDto(task, task.Project));
    }

    public async Task<Result<List<TaskResponseDto>>> GetAllAsync()
    {
        var tasks = await _taskRepo.GetAllAsync();
        return Result<List<TaskResponseDto>>.Ok(tasks.Select(t => MapToDto(t, t.Project)).ToList());
    }

    public async Task<Result<List<TaskResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var tasks = await _taskRepo.GetByProjectAsync(projectId);
        return Result<List<TaskResponseDto>>.Ok(tasks.Select(t => MapToDto(t, t.Project)).ToList());
    }

    public async Task<Result<TaskResponseDto>> UpdateAsync(Guid id, UpdateTaskDto request)
    {
        var errors = TaskGuard.Validate(request);
        if (errors.Any()) return Result<TaskResponseDto>.Fail(errors);

        var task = await _taskRepo.GetByIdAsync(id);
        if (task is null) return Result<TaskResponseDto>.Fail("Task not found.");

        task.Title = request.Title.Trim();
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.AssignedToUserId = request.AssignedToUserId;
        task.DueDate = request.DueDate;

        // Handle status transition
        if (request.Status == WorkItemStatus.Done && task.Status != WorkItemStatus.Done)
            task.CompletedAt = DateTime.UtcNow;
        else if (request.Status != WorkItemStatus.Done && task.Status == WorkItemStatus.Done)
            task.CompletedAt = null;

        task.Status = request.Status;

        await _taskRepo.SaveChangesAsync();
        return Result<TaskResponseDto>.Ok(MapToDto(task, task.Project), "Task updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task is null) return Result<bool>.Fail("Task not found.");

        task.IsActive = false;
        await _taskRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Task deleted.");
    }

    private static TaskResponseDto MapToDto(TaskItem t, Project? project) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = (int)t.Status,
        StatusName = t.Status.ToString(),
        Priority = (int)t.Priority,
        PriorityName = t.Priority.ToString(),
        ProjectId = t.ProjectId,
        ProjectCode = project?.Code ?? string.Empty,
        ProjectName = project?.Name ?? string.Empty,
        ProjectDivisionAccent = project?.Division?.AccentColor ?? "#1E90FF",
        AssignedToUserId = t.AssignedToUserId,
        AssignedToName = t.AssignedToUser is null ? null : $"{t.AssignedToUser.FirstName} {t.AssignedToUser.LastName}",
        DueDate = t.DueDate,
        CompletedAt = t.CompletedAt,
        CreatedAt = t.CreatedAt
    };
}