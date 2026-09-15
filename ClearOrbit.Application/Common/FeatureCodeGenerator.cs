namespace ClearOrbit.Application.Common;

public static class FeatureCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-FTR-{nextSequence:D5}";
    }
}