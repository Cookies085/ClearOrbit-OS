namespace ClearOrbit.Application.Common;

public static class AssessmentCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-ASM-{nextSequence:D5}";
    }
}