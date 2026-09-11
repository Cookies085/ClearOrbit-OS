using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Expenses;

namespace ClearOrbit.Application.Interfaces;

public interface IExpenseService
{
    Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto request);
    Task<Result<ExpenseResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ExpenseResponseDto>>> GetAllAsync();
    Task<Result<List<ExpenseResponseDto>>> GetByProjectAsync(Guid projectId);
    Task<Result<ExpenseResponseDto>> UpdateAsync(Guid id, UpdateExpenseDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}