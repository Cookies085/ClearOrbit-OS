using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Division : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Project> DeliveredProjects { get; set; } = new List<Project>();
    public ICollection<Project> RequestedProjects { get; set; } = new List<Project>();
    public ICollection<Invoice> IssuedInvoices { get; set; } = new List<Invoice>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Feature> Features { get; set; } = new List<Feature>();
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
    public ICollection<Release> Releases { get; set; } = new List<Release>();
}