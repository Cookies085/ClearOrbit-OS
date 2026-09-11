using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClearOrbit.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context) => _context = context;

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.Division)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.Division)
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.Division)
            .Where(e => e.IsActive && e.ProjectId == projectId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetByDivisionAsync(Guid divisionId)
    {
        return await _context.Expenses
            .Include(e => e.Division)
            .Where(e => e.IsActive && e.DivisionId == divisionId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<int> GetNextSequenceAsync()
    {
        var count = await _context.Expenses.CountAsync();
        return count + 1;
    }

    public async Task AddAsync(Expense expense) => await _context.Expenses.AddAsync(expense);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}