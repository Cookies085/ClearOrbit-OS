using ClearOrbit.Domain.Common;

namespace ClearOrbit.Domain.Entities;

public class Session : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty; // e.g., "iPhone 14", "Web Chrome"
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}