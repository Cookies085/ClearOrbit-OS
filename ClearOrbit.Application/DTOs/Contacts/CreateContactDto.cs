using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Contacts;

public class CreateContactDto
{
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public ContactRole Role { get; set; } = ContactRole.Other;
}