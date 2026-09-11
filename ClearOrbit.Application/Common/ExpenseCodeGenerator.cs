namespace ClearOrbit.Application.Common;

public static class ExpenseCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-EXP-{nextSequence:D5}";
    }
}