namespace ClearOrbit.Application.DTOs.Contacts;

public class ContactResponseDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public int Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}