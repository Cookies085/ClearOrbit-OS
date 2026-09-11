using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearOrbit.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Token).IsRequired();
        builder.Property(s => s.DeviceInfo).HasMaxLength(300);

        // Index for fast lookups by token (used heavily by JWT middleware)
        builder.HasIndex(s => s.Token).IsUnique();
    }
}