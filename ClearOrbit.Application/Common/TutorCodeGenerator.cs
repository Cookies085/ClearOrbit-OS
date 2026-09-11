namespace ClearOrbit.Application.Common;

public static class TutorCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-TUT-{nextSequence:D5}";
    }
}