using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Reports;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class ReportsService : IReportsService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IDivisionRepository _divisionRepo;

    public ReportsService(
        IInvoiceRepository invoiceRepo,
        IExpenseRepository expenseRepo,
        IProjectRepository projectRepo,
        IClientRepository clientRepo,
        IDivisionRepository divisionRepo)
    {
        _invoiceRepo = invoiceRepo;
        _expenseRepo = expenseRepo;
        _projectRepo = projectRepo;
        _clientRepo = clientRepo;
        _divisionRepo = divisionRepo;
    }

    public async Task<Result<ReportsSummaryDto>> GetSummaryAsync()
    {
        var invoices = (await _invoiceRepo.GetAllAsync())
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .ToList();
        var expenses = await _expenseRepo.GetAllAsync();
        var projects = await _projectRepo.GetAllAsync();
        var clients = await _clientRepo.GetAllAsync();

        var today = DateTime.UtcNow.Date;
        var currency = invoices.FirstOrDefault()?.Currency
            ?? expenses.FirstOrDefault()?.Currency
            ?? "ZAR";

        var totalInvoiced = invoices.Sum(i => i.Total);
        var totalRevenue = invoices.Sum(i => i.AmountPaid);
        var outstanding = invoices.Sum(i => i.Total - i.AmountPaid);
        var overdue = invoices
            .Where(i => i.Status != InvoiceStatus.Paid && i.DueDate.Date < today)
            .Sum(i => i.Total - i.AmountPaid);
        var totalExpenses = expenses.Sum(e => e.Amount);
        var netProfit = totalRevenue - totalExpenses;
        var margin = totalRevenue > 0
            ? Math.Round(netProfit / totalRevenue * 100, 1)
            : 0;

        return Result<ReportsSummaryDto>.Ok(new ReportsSummaryDto
        {
            Currency = currency,
            TotalRevenue = totalRevenue,
            TotalInvoiced = totalInvoiced,
            OutstandingReceivables = outstanding,
            OverdueReceivables = overdue,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            ProfitMargin = margin,
            PaidInvoiceCount = invoices.Count(i => i.Status == InvoiceStatus.Paid),
            UnpaidInvoiceCount = invoices.Count(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Draft),
            OverdueInvoiceCount = invoices.Count(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled && i.DueDate.Date < today),
            TotalClients = clients.Count,
            TotalProjects = projects.Count
        });
    }

    public async Task<Result<List<DivisionProfitabilityDto>>> GetDivisionProfitabilityAsync()
    {
        var divisions = await _divisionRepo.GetAllAsync();
        var invoices = (await _invoiceRepo.GetAllAsync())
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .ToList();
        var expenses = await _expenseRepo.GetAllAsync();
        var projects = await _projectRepo.GetAllAsync();

        var result = divisions.Select(d =>
        {
            var divInvoices = invoices.Where(i => i.DivisionId == d.Id).ToList();
            var divExpenses = expenses.Where(e => e.DivisionId == d.Id).Sum(e => e.Amount);
            var divProjects = projects.Where(p => p.DivisionId == d.Id).ToList();

            var invoiced = divInvoices.Sum(i => i.Total);
            var paid = divInvoices.Sum(i => i.AmountPaid);
            var netProfit = paid - divExpenses;
            var margin = paid > 0 ? Math.Round(netProfit / paid * 100, 1) : 0;

            return new DivisionProfitabilityDto
            {
                DivisionId = d.Id,
                DivisionName = d.Name,
                AccentColor = d.AccentColor,
                ProjectCount = divProjects.Count,
                Invoiced = invoiced,
                Revenue = paid,
                Expenses = divExpenses,
                NetProfit = netProfit,
                ProfitMargin = margin
            };
        })
        .OrderByDescending(d => d.Revenue)
        .ToList();

        return Result<List<DivisionProfitabilityDto>>.Ok(result);
    }

    public async Task<Result<List<ClientProfitabilityDto>>> GetClientProfitabilityAsync()
    {
        var clients = await _clientRepo.GetAllAsync();
        var invoices = (await _invoiceRepo.GetAllAsync())
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .ToList();
        var projects = await _projectRepo.GetAllAsync();

        var today = DateTime.UtcNow.Date;

        var result = clients.Select(c =>
        {
            var clientInvoices = invoices.Where(i => i.ClientId == c.Id).ToList();
            var unpaid = clientInvoices
                .Where(i => i.Status != InvoiceStatus.Paid)
                .ToList();

            var oldestUnpaid = unpaid
                .OrderBy(i => i.IssueDate)
                .FirstOrDefault()?.IssueDate;

            var overdue = unpaid
                .Where(i => i.DueDate.Date < today)
                .Sum(i => i.Total - i.AmountPaid);

            return new ClientProfitabilityDto
            {
                ClientId = c.Id,
                ClientName = c.Name,
                ProjectCount = projects.Count(p => p.ClientId == c.Id),
                InvoiceCount = clientInvoices.Count,
                Invoiced = clientInvoices.Sum(i => i.Total),
                Paid = clientInvoices.Sum(i => i.AmountPaid),
                Outstanding = clientInvoices.Sum(i => i.Total - i.AmountPaid),
                Overdue = overdue,
                OldestUnpaidDate = oldestUnpaid,
                DaysOutstanding = oldestUnpaid.HasValue
                    ? (int)(today - oldestUnpaid.Value.Date).TotalDays
                    : 0
            };
        })
        .OrderByDescending(c => c.Invoiced)
        .ToList();

        return Result<List<ClientProfitabilityDto>>.Ok(result);
    }

    public async Task<Result<List<ProjectProfitabilityDto>>> GetProjectProfitabilityAsync()
    {
        var projects = await _projectRepo.GetAllAsync();
        var invoices = (await _invoiceRepo.GetAllAsync())
            .Where(i => i.Status != InvoiceStatus.Cancelled)
            .ToList();
        var expenses = await _expenseRepo.GetAllAsync();

        var result = projects.Select(p =>
        {
            var projInvoices = invoices.Where(i => i.ProjectId == p.Id).ToList();
            var projExpenses = expenses.Where(e => e.ProjectId == p.Id).Sum(e => e.Amount);

            var invoiced = projInvoices.Sum(i => i.Total);
            var paid = projInvoices.Sum(i => i.AmountPaid);
            var netProfit = paid - projExpenses;
            var margin = paid > 0 ? Math.Round(netProfit / paid * 100, 1) : 0;

            return new ProjectProfitabilityDto
            {
                ProjectId = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.Division?.Name ?? "—",
                DivisionAccent = p.Division?.AccentColor ?? "#1E90FF",
                ClientName = p.Client?.Name,
                StatusName = p.Status.ToString(),
                Invoiced = invoiced,
                Paid = paid,
                Expenses = projExpenses,
                NetProfit = netProfit,
                ProfitMargin = margin,
                Currency = projInvoices.FirstOrDefault()?.Currency ?? "ZAR"
            };
        })
        .OrderByDescending(p => p.NetProfit)
        .ToList();

        return Result<List<ProjectProfitabilityDto>>.Ok(result);
    }
}