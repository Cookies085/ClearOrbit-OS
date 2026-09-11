using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Guards;

public static class ClassGuard
{
    public static List<string> Validate(CreateClassDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Class name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Class name cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(request.Subject))
            errors.Add("Subject is required.");

        if (request.TutorId == Guid.Empty)
            errors.Add("A tutor must be assigned.");

        if (request.EndTime <= request.StartTime)
            errors.Add("End time must be after start time.");

        if (request.MaxCapacity <= 0)
            errors.Add("Maximum capacity must be greater than zero.");

        if (request.FeePerLearner < 0)
            errors.Add("Fee cannot be negative.");

        return errors;
    }

    public static List<string> Validate(UpdateClassDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Class name is required.");

        if (string.IsNullOrWhiteSpace(request.Subject))
            errors.Add("Subject is required.");

        if (request.TutorId == Guid.Empty)
            errors.Add("A tutor must be assigned.");

        if (request.EndTime <= request.StartTime)
            errors.Add("End time must be after start time.");

        if (request.MaxCapacity <= 0)
            errors.Add("Maximum capacity must be greater than zero.");

        return errors;
    }
}