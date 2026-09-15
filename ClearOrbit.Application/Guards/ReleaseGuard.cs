using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Guards;

public static class ReleaseGuard
{
    public static List<string> Validate(CreateReleaseDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Version))
            errors.Add("Version is required (e.g., v1.0.0).");
        else if (request.Version.Length > 50)
            errors.Add("Version cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Release name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Release name cannot exceed 200 characters.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        return errors;
    }

    public static List<string> Validate(UpdateReleaseDto request)
    {
        return Validate(new CreateReleaseDto
        {
            Version = request.Version,
            Name = request.Name,
            DivisionId = request.DivisionId
        });
    }
}