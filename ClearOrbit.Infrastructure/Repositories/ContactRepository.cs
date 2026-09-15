using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;
    public ContactRepository(AppDbContext context) => _context = context;

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Contact>> GetByClientAsync(Guid clientId)
    {
        return await _context.Contacts
            .Include(c => c.Client)
            .Where(c => c.ClientId == clientId && c.IsActive)
            .OrderBy(c => c.Role)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<List<Contact>> GetAllAsync()
    {
        return await _context.Contacts
            .Include(c => c.Client)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Client!.Name)
            .ThenBy(c => c.LastName)
            .ToListAsync();
    }

    public async Task<bool> ExistsWithEmailInClientAsync(string email, Guid clientId, Guid? excludeId = null)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Contacts.AnyAsync(c =>
            c.ClientId == clientId &&
            c.Email.Value == normalized &&
            (excludeId == null || c.Id != excludeId.Value));
    }

    public async Task AddAsync(Contact contact) => await _context.Contacts.AddAsync(contact);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}