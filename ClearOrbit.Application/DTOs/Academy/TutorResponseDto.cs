namespace ClearOrbit.Application.DTOs.Academy;

public class TutorResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Specializations { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime JoinedOn { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}