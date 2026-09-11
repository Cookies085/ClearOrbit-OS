using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.ToLowerInvariant();
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email.Value == normalized);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var normalized = email.ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Email.Value == normalized);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}