namespace ClearOrbit.Application.Common;

public static class ReleaseCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-REL-{nextSequence:D5}";
    }
}