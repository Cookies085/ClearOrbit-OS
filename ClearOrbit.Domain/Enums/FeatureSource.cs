namespace ClearOrbit.Domain.Enums;

public enum FeatureSource
{
    Client = 1,      // Requested by a client
    Internal = 2,    // Internal need (e.g., ClearOrbit OS itself)
    Market = 3,      // Market-driven / strategic
    Regulatory = 4   // Compliance-driven
}