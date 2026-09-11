using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) => _context = context;

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.Invoice).ThenInclude(i => i.Client)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Invoice).ThenInclude(i => i.Client)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetByInvoiceAsync(Guid invoiceId)
    {
        return await _context.Payments
            .Include(p => p.Invoice).ThenInclude(i => i.Client)
            .Where(p => p.IsActive && p.InvoiceId == invoiceId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Payments.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Payment payment) => await _context.Payments.AddAsync(payment);

    public void Remove(Payment payment) => _context.Payments.Remove(payment);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}