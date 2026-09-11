namespace ClearOrbit.Domain.Enums;

public enum ServiceUnit
{
    Fixed = 1,        // One-time deliverable
    Hourly = 2,       // Billed per hour
    Monthly = 3,      // Retainer / subscription
    PerSession = 4    // Per class / session (Academy)
}