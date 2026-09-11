using ClearOrbit.Application.DTOs.Clients;

namespace ClearOrbit.Application.Guards;

public static class ClientGuard
{
    public static List<string> Validate(CreateClientDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Client name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Client name cannot exceed 200 characters.");

        if (!string.IsNullOrWhiteSpace(request.Website) &&
            !request.Website.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            errors.Add("Website must start with http:// or https://");

        return errors;
    }

    public static List<string> Validate(UpdateClientDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Client name is required.");
        else if (request.Name.Length > 200)
            errors.Add("Client name cannot exceed 200 characters.");

        if (!string.IsNullOrWhiteSpace(request.Website) &&
            !request.Website.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            errors.Add("Website must start with http:// or https://");

        return errors;
    }
}