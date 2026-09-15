using ClearOrbit.Application.DTOs.Software;

namespace ClearOrbit.Application.Guards;

public static class FeatureGuard
{
    public static List<string> Validate(CreateFeatureDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Feature title is required.");
        else if (request.Title.Length > 300)
            errors.Add("Title cannot exceed 300 characters.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        if (request.Source == Domain.Enums.FeatureSource.Client && request.ClientId is null)
            errors.Add("Client-sourced features must specify a client.");

        if (request.EstimatedEffort.HasValue && request.EstimatedEffort < 0)
            errors.Add("Estimated effort cannot be negative.");

        if (request.TargetDate.HasValue && request.TargetDate.Value.Date < DateTime.UtcNow.Date.AddDays(-1))
            errors.Add("Target date cannot be in the past.");

        return errors;
    }

    public static List<string> Validate(UpdateFeatureDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Feature title is required.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        if (request.Source == Domain.Enums.FeatureSource.Client && request.ClientId is null)
            errors.Add("Client-sourced features must specify a client.");

        if (request.EstimatedEffort.HasValue && request.EstimatedEffort < 0)
            errors.Add("Estimated effort cannot be negative.");

        if (request.ActualEffort.HasValue && request.ActualEffort < 0)
            errors.Add("Actual effort cannot be negative.");

        return errors;
    }
}