using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id);
    Task<List<Expense>> GetAllAsync();
    Task<List<Expense>> GetByProjectAsync(Guid projectId);
    Task<List<Expense>> GetByDivisionAsync(Guid divisionId);
    Task<int> GetNextSequenceAsync();
    Task AddAsync(Expense expense);
    Task SaveChangesAsync();
}