namespace ClearOrbit.Application.DTOs.Services;

public class ServiceResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public int Unit { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public string DivisionAccent { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}