using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Services;

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public ServiceUnit Unit { get; set; } = ServiceUnit.Fixed;
    public Guid DivisionId { get; set; }
}