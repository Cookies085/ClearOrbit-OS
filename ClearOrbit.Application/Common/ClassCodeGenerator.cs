namespace ClearOrbit.Application.Common;

public static class ClassCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-CLS-{nextSequence:D5}";
    }
}