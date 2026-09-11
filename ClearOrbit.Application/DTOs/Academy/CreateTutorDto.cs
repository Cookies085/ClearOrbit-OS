namespace ClearOrbit.Application.DTOs.Academy;

public class CreateTutorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Specializations { get; set; } = string.Empty;
    public string? Bio { get; set; }
}