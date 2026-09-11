namespace ClearOrbit.Application.DTOs.Clients;

public class ClientResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public int ContactCount { get; set; }
    public DateTime CreatedAt { get; set; }
}