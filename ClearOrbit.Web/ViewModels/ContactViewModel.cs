using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class ContactViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Please select a client.")]
    public Guid ClientId { get; set; }

    public string? ClientName { get; set; }

    // Only populated when clientId is not preset
    public List<ClientOption> ClientOptions { get; set; } = new();

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? JobTitle { get; set; }

    public int Role { get; set; } = 99;
}

public class ContactListItem
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