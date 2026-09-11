using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context) => _context = context;

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Division)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Division)
            .Where(i => i.IsActive)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<List<Invoice>> GetByClientAsync(Guid clientId)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Division)
            .Where(i => i.IsActive && i.ClientId == clientId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<List<Invoice>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Client)
            .Include(i => i.Division)
            .Where(i => i.IsActive && i.ProjectId == projectId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Invoices.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Invoice invoice) => await _context.Invoices.AddAsync(invoice);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}