using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Guards;

public static class BugGuard
{
    public static List<string> Validate(CreateBugDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Bug title is required.");
        else if (request.Title.Length > 300)
            errors.Add("Title cannot exceed 300 characters.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        return errors;
    }

    public static List<string> Validate(UpdateBugDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Bug title is required.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        return errors;
    }
}