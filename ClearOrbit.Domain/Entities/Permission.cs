using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Code { get; set; } = string.Empty; // e.g., Invoice:Create
        public string Description { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}