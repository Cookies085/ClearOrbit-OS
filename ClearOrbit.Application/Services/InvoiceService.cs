using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Invoices;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IDivisionRepository _divisionRepo;

    public InvoiceService(
        IInvoiceRepository invoiceRepo,
        IClientRepository clientRepo,
        IProjectRepository projectRepo,
        IDivisionRepository divisionRepo)
    {
        _invoiceRepo = invoiceRepo;
        _clientRepo = clientRepo;
        _projectRepo = projectRepo;
        _divisionRepo = divisionRepo;
    }

    public async Task<Result<InvoiceResponseDto>> CreateAsync(CreateInvoiceDto request)
    {
        var errors = InvoiceGuard.Validate(request);
        if (errors.Any()) return Result<InvoiceResponseDto>.Fail(errors);

        var client = await _clientRepo.GetByIdAsync(request.ClientId);
        if (client is null) return Result<InvoiceResponseDto>.Fail("Client not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<InvoiceResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<InvoiceResponseDto>.Fail("Project not found.");
        }

        var sequence = await _invoiceRepo.GetNextSequenceAsync();
        var code = InvoiceCodeGenerator.Generate(sequence);

        var invoice = new Invoice
        {
            Code = code,
            Status = InvoiceStatus.Draft,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            DivisionId = request.DivisionId,
            IssueDate = request.IssueDate,
            DueDate = request.DueDate,
            Currency = request.Currency.ToUpperInvariant(),
            TaxRate = request.TaxRate,
            Notes = request.Notes
        };

        foreach (var item in request.Items)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            });
        }

        RecalculateTotals(invoice);

        await _invoiceRepo.AddAsync(invoice);
        await _invoiceRepo.SaveChangesAsync();

        return Result<InvoiceResponseDto>.Ok(
            MapToDto(invoice, client, project, division),
            $"Invoice {code} created.");
    }

    public async Task<Result<InvoiceResponseDto>> GetByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice is null) return Result<InvoiceResponseDto>.Fail("Invoice not found.");
        return Result<InvoiceResponseDto>.Ok(MapToDto(invoice, invoice.Client, invoice.Project, invoice.Division));
    }

    public async Task<Result<List<InvoiceResponseDto>>> GetAllAsync()
    {
        var invoices = await _invoiceRepo.GetAllAsync();
        return Result<List<InvoiceResponseDto>>.Ok(invoices
            .Select(i => MapToDto(i, i.Client, i.Project, i.Division))
            .ToList());
    }

    public async Task<Result<InvoiceResponseDto>> UpdateAsync(Guid id, UpdateInvoiceDto request)
    {
        var errors = InvoiceGuard.Validate(request);
        if (errors.Any()) return Result<InvoiceResponseDto>.Fail(errors);

        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice is null) return Result<InvoiceResponseDto>.Fail("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<InvoiceResponseDto>.Fail("Only draft invoices can be edited.");

        invoice.IssueDate = request.IssueDate;
        invoice.DueDate = request.DueDate;
        invoice.TaxRate = request.TaxRate;
        invoice.Notes = request.Notes;

        // Replace all items
        invoice.Items.Clear();
        foreach (var item in request.Items)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            });
        }

        RecalculateTotals(invoice);

        await _invoiceRepo.SaveChangesAsync();
        return Result<InvoiceResponseDto>.Ok(
            MapToDto(invoice, invoice.Client, invoice.Project, invoice.Division),
            "Invoice updated.");
    }

    public async Task<Result<InvoiceResponseDto>> MarkAsSentAsync(Guid id)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice is null) return Result<InvoiceResponseDto>.Fail("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<InvoiceResponseDto>.Fail("Only draft invoices can be sent.");

        invoice.Status = InvoiceStatus.Sent;
        invoice.SentAt = DateTime.UtcNow;
        await _invoiceRepo.SaveChangesAsync();

        return Result<InvoiceResponseDto>.Ok(
            MapToDto(invoice, invoice.Client, invoice.Project, invoice.Division),
            "Invoice marked as sent.");
    }

    public async Task<Result<InvoiceResponseDto>> CancelAsync(Guid id)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice is null) return Result<InvoiceResponseDto>.Fail("Invoice not found.");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<InvoiceResponseDto>.Fail("Paid invoices cannot be cancelled.");

        invoice.Status = InvoiceStatus.Cancelled;
        await _invoiceRepo.SaveChangesAsync();

        return Result<InvoiceResponseDto>.Ok(
            MapToDto(invoice, invoice.Client, invoice.Project, invoice.Division),
            "Invoice cancelled.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(id);
        if (invoice is null) return Result<bool>.Fail("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Draft)
            return Result<bool>.Fail("Only draft invoices can be deleted.");

        invoice.IsActive = false;
        await _invoiceRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Invoice deleted.");
    }

    private static void RecalculateTotals(Invoice invoice)
    {
        invoice.SubTotal = invoice.Items.Sum(i => i.LineTotal);
        invoice.TaxAmount = Math.Round(invoice.SubTotal * invoice.TaxRate / 100m, 2);
        invoice.Total = invoice.SubTotal + invoice.TaxAmount;
    }

    private static InvoiceResponseDto MapToDto(Invoice i, Client? client, Project? project, Division? division)
    {
        var today = DateTime.UtcNow.Date;
        var isOverdue = i.Status != InvoiceStatus.Paid
            && i.Status != InvoiceStatus.Cancelled
            && i.DueDate.Date < today;

        return new InvoiceResponseDto
        {
            Id = i.Id,
            Code = i.Code,
            Status = (int)i.Status,
            StatusName = i.Status.ToString(),
            ClientId = i.ClientId,
            ClientName = client?.Name ?? string.Empty,
            ProjectId = i.ProjectId,
            ProjectCode = project?.Code,
            ProjectName = project?.Name,
            DivisionId = i.DivisionId,
            DivisionName = division?.Name ?? string.Empty,
            DivisionAccent = division?.AccentColor ?? "#1E90FF",
            IssueDate = i.IssueDate,
            DueDate = i.DueDate,
            SentAt = i.SentAt,
            PaidAt = i.PaidAt,
            SubTotal = i.SubTotal,
            TaxRate = i.TaxRate,
            TaxAmount = i.TaxAmount,
            Total = i.Total,
            AmountPaid = i.AmountPaid,
            BalanceDue = i.Total - i.AmountPaid,
            Currency = i.Currency,
            Notes = i.Notes,
            IsOverdue = isOverdue,
            Items = i.Items.Select(it => new InvoiceItemDto
            {
                Id = it.Id,
                Description = it.Description,
                Quantity = it.Quantity,
                UnitPrice = it.UnitPrice,
                LineTotal = it.LineTotal
            }).ToList(),
            CreatedAt = i.CreatedAt
        };
    }
}