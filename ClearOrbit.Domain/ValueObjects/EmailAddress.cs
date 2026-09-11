using System.Text.RegularExpressions;

namespace ClearOrbit.Domain.ValueObjects;

public record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        if (!IsValid(value))
            throw new ArgumentException("Invalid email format.");

        Value = value.ToLowerInvariant();
    }

    private static bool IsValid(string email)
    {
        // Simple regex for email validation
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public override string ToString() => Value;
}