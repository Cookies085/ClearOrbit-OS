using ClearOrbit.Domain.Common;
using ClearOrbit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClearOrbit.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Identity & Access
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<Tutor> Tutors => Set<Tutor>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Organization>().HasData(new Organization
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "ClearOrbit Group",
            Tagline = "Where Clarity Meets Innovation",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true
        });

        var orgId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Division>().HasData(
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Academy", AccentColor = "#1E90FF", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Software", AccentColor = "#06B6D4", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Games", AccentColor = "#8B5CF6", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Designs", AccentColor = "#F59E0B", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Innovation Lab", AccentColor = "#14B8A6", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Digital Solutions", AccentColor = "#3B82F6", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "MediaWorks", AccentColor = "#EC4899", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true },
            new Division { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "Training & Certification", AccentColor = "#EAB308", OrganizationId = orgId, CreatedAt = seedDate, IsActive = true }
        );
    }

    // Override SaveChanges to auto-populate audit fields and audit log
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}