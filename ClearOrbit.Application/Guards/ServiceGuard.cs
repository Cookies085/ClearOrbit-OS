using ClearOrbit.Application.DTOs.Services;

namespace ClearOrbit.Application.Guards;

public static class ServiceGuard
{
    public static List<string> Validate(CreateServiceDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Service name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Service name cannot exceed 200 characters.");

        if (request.BasePrice < 0)
            errors.Add("Base price cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
            errors.Add("Currency must be a 3-letter code (e.g., ZAR).");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division must be selected.");

        return errors;
    }

    public static List<string> Validate(UpdateServiceDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Service name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Service name cannot exceed 200 characters.");

        if (request.BasePrice < 0)
            errors.Add("Base price cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
            errors.Add("Currency must be a 3-letter code (e.g., ZAR).");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division must be selected.");

        return errors;
    }
}