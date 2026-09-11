using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Division : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., Academy, Software
    public string AccentColor { get; set; } = string.Empty; // e.g., Blue, Cyan

    // Foreign Key
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}