namespace ClearOrbit.Application.Guards;

public static class EnrollmentGuard
{
    public static List<string> ValidateCapacity(int currentCount, int maxCapacity)
    {
        var errors = new List<string>();

        if (currentCount >= maxCapacity)
            errors.Add($"Class is full ({currentCount}/{maxCapacity}).");

        return errors;
    }
}