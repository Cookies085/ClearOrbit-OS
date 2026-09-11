using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Enums;
using ClearOrbit.Domain.ValueObjects;

namespace ClearOrbit.Domain.Entities;

public class Contact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public EmailAddress Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public ContactRole Role { get; set; } = ContactRole.Other;

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
}