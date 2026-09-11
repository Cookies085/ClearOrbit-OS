using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Guards;

public static class LearnerGuard
{
    public static List<string> Validate(CreateLearnerDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");

        if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains("@"))
            errors.Add("Learner email must be a valid email address.");

        if (!string.IsNullOrWhiteSpace(request.GuardianEmail) && !request.GuardianEmail.Contains("@"))
            errors.Add("Guardian email must be a valid email address.");

        if (request.DateOfBirth.HasValue && request.DateOfBirth.Value.Date > DateTime.UtcNow.Date)
            errors.Add("Date of birth cannot be in the future.");

        return errors;
    }

    public static List<string> Validate(UpdateLearnerDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");

        return errors;
    }
}