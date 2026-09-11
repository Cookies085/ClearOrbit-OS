using ClearOrbit.Application.DTOs.Invoices;

namespace ClearOrbit.Application.Guards;

public static class InvoiceGuard
{
    public static List<string> Validate(CreateInvoiceDto request)
    {
        var errors = new List<string>();

        if (request.ClientId == Guid.Empty)
            errors.Add("A client is required.");

        if (request.DivisionId == Guid.Empty)
            errors.Add("A division is required.");

        if (request.DueDate < request.IssueDate)
            errors.Add("Due date cannot be before issue date.");

        if (request.TaxRate < 0 || request.TaxRate > 100)
            errors.Add("Tax rate must be between 0 and 100.");

        if (request.Items == null || request.Items.Count == 0)
            errors.Add("At least one invoice item is required.");
        else
        {
            for (int i = 0; i < request.Items.Count; i++)
            {
                var item = request.Items[i];
                if (string.IsNullOrWhiteSpace(item.Description))
                    errors.Add($"Item {i + 1}: description is required.");
                if (item.Quantity <= 0)
                    errors.Add($"Item {i + 1}: quantity must be greater than zero.");
                if (item.UnitPrice < 0)
                    errors.Add($"Item {i + 1}: unit price cannot be negative.");
            }
        }

        return errors;
    }

    public static List<string> Validate(UpdateInvoiceDto request)
    {
        var errors = new List<string>();

        if (request.DueDate < request.IssueDate)
            errors.Add("Due date cannot be before issue date.");

        if (request.TaxRate < 0 || request.TaxRate > 100)
            errors.Add("Tax rate must be between 0 and 100.");

        if (request.Items == null || request.Items.Count == 0)
            errors.Add("At least one invoice item is required.");

        return errors;
    }
}