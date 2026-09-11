using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id)
    {
        return await _context.Clients
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await _context.Clients
            .Include(c => c.Contacts)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var trimmed = name.Trim().ToLower();
        return await _context.Clients
            .AnyAsync(c => c.Name.ToLower() == trimmed);
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}