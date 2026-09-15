namespace ClearOrbit.Application.Common;

public static class BugCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-BUG-{nextSequence:D5}";
    }
}