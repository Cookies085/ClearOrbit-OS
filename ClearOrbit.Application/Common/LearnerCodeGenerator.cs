namespace ClearOrbit.Application.Common;

public static class LearnerCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-LRN-{nextSequence:D5}";
    }
}