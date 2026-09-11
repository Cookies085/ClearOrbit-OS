using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Dashboard;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IClientRepository _clientRepo;
    private readonly ITaskRepository _taskRepo;
    private readonly IDivisionRepository _divisionRepo;

    public DashboardService(
        IInvoiceRepository invoiceRepo,
        IExpenseRepository expenseRepo,
        IProjectRepository projectRepo,
        IClientRepository clientRepo,
        ITaskRepository taskRepo,
        IDivisionRepository divisionRepo)
    {
        _invoiceRepo = invoiceRepo;
        _expenseRepo = expenseRepo;
        _projectRepo = projectRepo;
        _clientRepo = clientRepo;
        _taskRepo = taskRepo;
        _divisionRepo = divisionRepo;
    }

    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync()
    {
        var invoices = await _invoiceRepo.GetAllAsync();
        var expenses = await _expenseRepo.GetAllAsync();
        var projects = await _projectRepo.GetAllAsync();
        var clients = await _clientRepo.GetAllAsync();
        var tasks = await _taskRepo.GetAllAsync();
        var divisions = await _divisionRepo.GetAllAsync();

        var currency = invoices.FirstOrDefault()?.Currency
            ?? expenses.FirstOrDefault()?.Currency
            ?? "ZAR";

        var today = DateTime.UtcNow.Date;

        // Money — exclude cancelled invoices
        var validInvoices = invoices.Where(i => i.Status != InvoiceStatus.Cancelled).ToList();
        var totalRevenue = validInvoices.Sum(i => i.AmountPaid);
        var outstanding = validInvoices.Sum(i => i.Total - i.AmountPaid);
        var totalExpenses = expenses.Sum(e => e.Amount);
        var profit = totalRevenue - totalExpenses;

        // Counts
        var overdueCount = validInvoices.Count(i =>
            i.Status != InvoiceStatus.Paid && i.DueDate.Date < today);

        var openTasks = tasks.Count(t => t.Status != WorkItemStatus.Done
            && t.Status != WorkItemStatus.Cancelled);

        var activeProjects = projects.Count(p => p.Status == ProjectStatus.Active);

        // Recent projects (last 5)
        var recentProjects = projects
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new RecentProjectDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                StatusName = p.Status.ToString(),
                DivisionName = p.Division?.Name ?? string.Empty,
                DivisionAccent = p.Division?.AccentColor ?? "#1E90FF",
                ClientName = p.Client?.Name,
                CreatedAt = p.CreatedAt
            }).ToList();

        // Recent invoices (last 5)
        var recentInvoices = validInvoices
            .OrderByDescending(i => i.IssueDate)
            .Take(5)
            .Select(i => new RecentInvoiceDto
            {
                Id = i.Id,
                Code = i.Code,
                ClientName = i.Client?.Name ?? string.Empty,
                Total = i.Total,
                BalanceDue = i.Total - i.AmountPaid,
                Currency = i.Currency,
                StatusName = i.Status.ToString(),
                IsOverdue = i.Status != InvoiceStatus.Paid
                    && i.Status != InvoiceStatus.Cancelled
                    && i.DueDate.Date < today,
                IssueDate = i.IssueDate
            }).ToList();

        // Division breakdown
        var divisionBreakdown = divisions.Select(d =>
        {
            var divProjects = projects.Where(p => p.DivisionId == d.Id).ToList();
            var divRevenue = validInvoices
                .Where(i => i.DivisionId == d.Id)
                .Sum(i => i.AmountPaid);
            var divExpenses = expenses
                .Where(e => e.DivisionId == d.Id)
                .Sum(e => e.Amount);

            return new DivisionBreakdownDto
            {
                DivisionId = d.Id,
                DivisionName = d.Name,
                AccentColor = d.AccentColor,
                Revenue = divRevenue,
                Expenses = divExpenses,
                Profit = divRevenue - divExpenses,
                ProjectCount = divProjects.Count
            };
        })
        .OrderByDescending(d => d.Revenue)
        .ToList();

        return Result<DashboardSummaryDto>.Ok(new DashboardSummaryDto
        {
            TotalRevenue = totalRevenue,
            OutstandingReceivables = outstanding,
            TotalExpenses = totalExpenses,
            Profit = profit,
            Currency = currency,
            ClientCount = clients.Count,
            TotalProjectCount = projects.Count,
            ActiveProjectCount = activeProjects,
            OpenTaskCount = openTasks,
            OverdueInvoiceCount = overdueCount,
            RecentProjects = recentProjects,
            RecentInvoices = recentInvoices,
            DivisionBreakdown = divisionBreakdown
        });
    }
}