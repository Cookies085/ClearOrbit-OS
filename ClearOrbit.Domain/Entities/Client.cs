using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }

    // Every client belongs to one organization (the Group)
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    // Navigation
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}