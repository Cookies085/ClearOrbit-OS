using ClearOrbit.Application.DTOs.Projects;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Guards;

public static class ProjectGuard
{
    public static List<string> Validate(CreateProjectDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Project name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Project name cannot exceed 200 characters.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A delivering division is required.");

        if (request.Type == ProjectType.External && request.ClientId is null)
            errors.Add("External projects require a client.");

        if (request.Type == ProjectType.Internal && request.RequestingDivisionId is null)
            errors.Add("Internal projects require a requesting division.");

        if (request.Type == ProjectType.Internal && request.RequestingDivisionId == request.DivisionId)
            errors.Add("A division cannot request work from itself.");

        if (request.Budget.HasValue && request.Budget < 0)
            errors.Add("Budget cannot be negative.");

        if (request.StartDate.HasValue && request.DueDate.HasValue && request.DueDate < request.StartDate)
            errors.Add("Due date cannot be before start date.");

        return errors;
    }

    public static List<string> Validate(UpdateProjectDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Project name is required.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A delivering division is required.");

        if (request.Budget.HasValue && request.Budget < 0)
            errors.Add("Budget cannot be negative.");

        if (request.StartDate.HasValue && request.DueDate.HasValue && request.DueDate < request.StartDate)
            errors.Add("Due date cannot be before start date.");

        return errors;
    }
}