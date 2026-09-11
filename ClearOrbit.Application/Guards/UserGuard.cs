using ClearOrbit.Application.DTOs.Auth;

namespace ClearOrbit.Application.Guards;

public static class UserGuard
{
    public static List<string> Validate(RegisterRequestDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            errors.Add("A valid email address is required.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            errors.Add("Password must be at least 8 characters.");

        if (!request.Password.Any(char.IsDigit))
            errors.Add("Password must contain at least one number.");

        if (!request.Password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");

        return errors;
    }
}