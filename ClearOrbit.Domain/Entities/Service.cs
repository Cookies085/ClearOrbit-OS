using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public ServiceUnit Unit { get; set; } = ServiceUnit.Fixed;
    public bool IsActive { get; set; } = true;

    public Guid DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}