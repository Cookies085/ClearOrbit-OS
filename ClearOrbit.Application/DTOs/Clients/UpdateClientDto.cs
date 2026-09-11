namespace ClearOrbit.Application.DTOs.Clients;

public class UpdateClientDto
{
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
}