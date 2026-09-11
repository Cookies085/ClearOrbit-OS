using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., Finance Manager
        public string Description { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}