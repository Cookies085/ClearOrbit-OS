using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Expenses;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IDivisionRepository _divisionRepo;

    public ExpenseService(
        IExpenseRepository expenseRepo,
        IProjectRepository projectRepo,
        IDivisionRepository divisionRepo)
    {
        _expenseRepo = expenseRepo;
        _projectRepo = projectRepo;
        _divisionRepo = divisionRepo;
    }

    public async Task<Result<ExpenseResponseDto>> CreateAsync(CreateExpenseDto request)
    {
        var errors = ExpenseGuard.Validate(request);
        if (errors.Any()) return Result<ExpenseResponseDto>.Fail(errors);

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<ExpenseResponseDto>.Fail("Project not found.");
        }

        Division? division = null;
        if (request.DivisionId.HasValue)
        {
            division = await _divisionRepo.GetByIdAsync(request.DivisionId.Value);
            if (division is null) return Result<ExpenseResponseDto>.Fail("Division not found.");
        }

        // If division wasn't specified but project has one, inherit it
        if (division is null && project is not null)
        {
            division = await _divisionRepo.GetByIdAsync(project.DivisionId);
        }

        var sequence = await _expenseRepo.GetNextSequenceAsync();
        var code = ExpenseCodeGenerator.Generate(sequence);

        var expense = new Expense
        {
            Code = code,
            Description = request.Description.Trim(),
            Category = request.Category,
            ProjectId = request.ProjectId,
            DivisionId = division?.Id,
            ExpenseDate = request.ExpenseDate,
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            Vendor = request.Vendor?.Trim(),
            Reference = request.Reference?.Trim(),
            Notes = request.Notes
        };

        await _expenseRepo.AddAsync(expense);
        await _expenseRepo.SaveChangesAsync();

        return Result<ExpenseResponseDto>.Ok(MapToDto(expense, project, division), $"Expense {code} recorded.");
    }

    public async Task<Result<ExpenseResponseDto>> GetByIdAsync(Guid id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense is null) return Result<ExpenseResponseDto>.Fail("Expense not found.");
        return Result<ExpenseResponseDto>.Ok(MapToDto(expense, expense.Project, expense.Division));
    }

    public async Task<Result<List<ExpenseResponseDto>>> GetAllAsync()
    {
        var expenses = await _expenseRepo.GetAllAsync();
        return Result<List<ExpenseResponseDto>>.Ok(expenses
            .Select(e => MapToDto(e, e.Project, e.Division))
            .ToList());
    }

    public async Task<Result<List<ExpenseResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var expenses = await _expenseRepo.GetByProjectAsync(projectId);
        return Result<List<ExpenseResponseDto>>.Ok(expenses
            .Select(e => MapToDto(e, e.Project, e.Division))
            .ToList());
    }

    public async Task<Result<ExpenseResponseDto>> UpdateAsync(Guid id, UpdateExpenseDto request)
    {
        var errors = ExpenseGuard.Validate(request);
        if (errors.Any()) return Result<ExpenseResponseDto>.Fail(errors);

        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense is null) return Result<ExpenseResponseDto>.Fail("Expense not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<ExpenseResponseDto>.Fail("Project not found.");
        }

        Division? division = null;
        if (request.DivisionId.HasValue)
        {
            division = await _divisionRepo.GetByIdAsync(request.DivisionId.Value);
            if (division is null) return Result<ExpenseResponseDto>.Fail("Division not found.");
        }
        else if (project is not null)
        {
            division = await _divisionRepo.GetByIdAsync(project.DivisionId);
        }

        expense.Description = request.Description.Trim();
        expense.Category = request.Category;
        expense.ProjectId = request.ProjectId;
        expense.DivisionId = division?.Id;
        expense.ExpenseDate = request.ExpenseDate;
        expense.Amount = request.Amount;
        expense.Currency = request.Currency.ToUpperInvariant();
        expense.Vendor = request.Vendor?.Trim();
        expense.Reference = request.Reference?.Trim();
        expense.Notes = request.Notes;

        await _expenseRepo.SaveChangesAsync();
        return Result<ExpenseResponseDto>.Ok(MapToDto(expense, project, division), "Expense updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense is null) return Result<bool>.Fail("Expense not found.");

        expense.IsActive = false;
        await _expenseRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Expense deleted.");
    }

    private static ExpenseResponseDto MapToDto(Expense e, Project? project, Division? division) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Description = e.Description,
        Category = (int)e.Category,
        CategoryName = e.Category.ToString(),
        ProjectId = e.ProjectId,
        ProjectCode = project?.Code,
        ProjectName = project?.Name,
        DivisionId = e.DivisionId,
        DivisionName = division?.Name,
        DivisionAccent = division?.AccentColor,
        ExpenseDate = e.ExpenseDate,
        Amount = e.Amount,
        Currency = e.Currency,
        Vendor = e.Vendor,
        Reference = e.Reference,
        Notes = e.Notes,
        CreatedAt = e.CreatedAt
    };
}