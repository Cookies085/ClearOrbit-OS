using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Guards;

public static class TutorGuard
{
    public static List<string> Validate(CreateTutorDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");

        if (string.IsNullOrWhiteSpace(request.Specializations))
            errors.Add("At least one specialization is required.");

        return errors;
    }

    public static List<string> Validate(UpdateTutorDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");

        if (string.IsNullOrWhiteSpace(request.Specializations))
            errors.Add("At least one specialization is required.");

        return errors;
    }
}