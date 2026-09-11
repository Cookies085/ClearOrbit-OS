namespace ClearOrbit.Application.Common;

public static class PaymentCodeGenerator
{
    public static string Generate(int nextSequence)
    {
        return $"CO-PAY-{nextSequence:D5}";
    }
}