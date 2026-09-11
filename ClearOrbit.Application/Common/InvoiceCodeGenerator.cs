namespace ClearOrbit.Application.Common;

public static class InvoiceCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-INV-{nextSequence:D5}";
    }
}