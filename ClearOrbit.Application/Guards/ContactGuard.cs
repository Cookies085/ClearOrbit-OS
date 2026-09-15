using ClearOrbit.Application.DTOs.Contacts;

namespace ClearOrbit.Application.Guards;

public static class ContactGuard
{
    public static List<string> Validate(CreateContactDto request)
    {
        var errors = new List<string>();

        if (request.ClientId == Guid.Empty)
            errors.Add("A client is required.");

        ValidateNames(request.FirstName, request.LastName, request.Email, errors);

        return errors;
    }

    public static List<string> Validate(UpdateContactDto request)
    {
        var errors = new List<string>();
        ValidateNames(request.FirstName, request.LastName, request.Email, errors);
        return errors;
    }

    private static void ValidateNames(string firstName, string lastName, string email, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add("First name is required.");
        else if (firstName.Length > 100)
            errors.Add("First name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add("Last name is required.");
        else if (lastName.Length > 100)
            errors.Add("Last name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required.");
        else if (!email.Contains("@") || !email.Contains("."))
            errors.Add("Enter a valid email address.");
    }
}