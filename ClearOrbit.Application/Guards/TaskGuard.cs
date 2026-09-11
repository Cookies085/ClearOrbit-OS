using ClearOrbit.Application.DTOs.Tasks;

namespace ClearOrbit.Application.Guards;

public static class TaskGuard
{
    public static List<string> Validate(CreateTaskDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Task title is required.");
        else if (request.Title.Length > 300)
            errors.Add("Task title cannot exceed 300 characters.");

        if (request.ProjectId == Guid.Empty)
            errors.Add("A project is required.");

        return errors;
    }

    public static List<string> Validate(UpdateTaskDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Task title is required.");

        return errors;
    }
}