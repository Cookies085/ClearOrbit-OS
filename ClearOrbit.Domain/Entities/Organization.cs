using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = "ClearOrbit Group";
    public string Tagline { get; set; } = "Where Clarity Meets Innovation";

    // Navigation Property
    public ICollection<Division> Divisions { get; set; } = new List<Division>();
}