using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; } // Nullable in case system generates it
    public string Action { get; set; } = string.Empty; // e.g., "Created Invoice"
    public string EntityName { get; set; } = string.Empty; // e.g., "Invoice"
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; } // JSON stored as string
    public string? NewValues { get; set; } // JSON stored as string
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}