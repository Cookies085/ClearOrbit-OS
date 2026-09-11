using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Tutor : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // CO-TUT-00001
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string Specializations { get; set; } = string.Empty;  // comma-separated: "Maths, Physical Sciences"
    public string? Bio { get; set; }

    public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

    // Optional link to a system user account
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}