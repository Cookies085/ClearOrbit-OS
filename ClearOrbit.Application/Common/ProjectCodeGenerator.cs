using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Common;

public static class ProjectCodeGenerator
{
    private static readonly Dictionary<string, string> DivisionCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Academy", "ACD" },
        { "Software", "SFW" },
        { "Games", "GAM" },
        { "Designs", "DSN" },
        { "Innovation Lab", "INN" },
        { "Digital Solutions", "DIG" },
        { "MediaWorks", "MDW" },
        { "Training & Certification", "TRN" }
    };

    public static string Generate(Division division, int nextSequence)
    {
        var divCode = DivisionCodes.TryGetValue(division.Name, out var code) ? code : "GEN";
        return $"CO-{divCode}-{nextSequence:D5}";
    }
}