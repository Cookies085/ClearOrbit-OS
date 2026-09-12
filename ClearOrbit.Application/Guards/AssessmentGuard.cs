using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Guards;

public static class AssessmentGuard
{
    public static List<string> Validate(CreateAssessmentDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Assessment title is required.");
        else if (request.Title.Length > 200)
            errors.Add("Title cannot exceed 200 characters.");

        if (request.ClassId == Guid.Empty)
            errors.Add("A class is required.");

        if (request.MaxScore <= 0)
            errors.Add("Max score must be greater than zero.");

        if (request.Weight < 0 || request.Weight > 100)
            errors.Add("Weight must be between 0 and 100.");

        return errors;
    }

    public static List<string> Validate(UpdateAssessmentDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Assessment title is required.");

        if (request.MaxScore <= 0)
            errors.Add("Max score must be greater than zero.");

        if (request.Weight < 0 || request.Weight > 100)
            errors.Add("Weight must be between 0 and 100.");

        return errors;
    }

    public static List<string> ValidateScore(decimal score, decimal maxScore)
    {
        var errors = new List<string>();

        if (score < 0)
            errors.Add("Score cannot be negative.");

        if (score > maxScore)
            errors.Add($"Score cannot exceed the maximum ({maxScore}).");

        return errors;
    }
}