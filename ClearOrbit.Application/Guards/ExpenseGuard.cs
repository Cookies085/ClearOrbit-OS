using ClearOrbit.Application.DTOs.Expenses;

namespace ClearOrbit.Application.Guards;

public static class ExpenseGuard
{
    public static List<string> Validate(CreateExpenseDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add("Description is required.");
        else if (request.Description.Length > 500)
            errors.Add("Description cannot exceed 500 characters.");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero.");

        if (request.ExpenseDate.Date > DateTime.UtcNow.Date.AddDays(1))
            errors.Add("Expense date cannot be in the future.");

        if (request.ProjectId is null && request.DivisionId is null)
            errors.Add("An expense must be linked to a project or a division.");

        return errors;
    }

    public static List<string> Validate(UpdateExpenseDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add("Description is required.");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero.");

        if (request.ExpenseDate.Date > DateTime.UtcNow.Date.AddDays(1))
            errors.Add("Expense date cannot be in the future.");

        if (request.ProjectId is null && request.DivisionId is null)
            errors.Add("An expense must be linked to a project or a division.");

        return errors;
    }
}