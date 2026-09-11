using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IApiClient _api;
    public ExpensesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Expenses";
        ViewData["PageSubtitle"] = "Every cost ClearOrbit has incurred.";
        var response = await _api.GetAsync<ApiResult<List<ExpenseListItem>>>("/api/Expenses", Token);
        return View(response?.Data ?? new List<ExpenseListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null)
    {
        ViewData["PageTitle"] = "New Expense";
        var vm = new ExpenseViewModel();
        if (projectId.HasValue) vm.ProjectId = projectId.Value;
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Expense";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Description = model.Description,
            Category = model.Category,
            ProjectId = model.ProjectId,
            DivisionId = model.DivisionId,
            ExpenseDate = model.ExpenseDate,
            Amount = model.Amount,
            Currency = model.Currency,
            Vendor = model.Vendor,
            Reference = model.Reference,
            Notes = model.Notes
        };

        var response = await _api.PostAsync<ApiResult<ExpenseListItem>>("/api/Expenses", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to record expense.");
            ViewData["PageTitle"] = "New Expense";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Expense {response.Data?.Code} recorded.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ExpenseListItem>>($"/api/Expenses/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ExpenseViewModel
        {
            Id = response.Data.Id,
            Description = response.Data.Description,
            Category = response.Data.Category,
            ProjectId = response.Data.ProjectId,
            DivisionId = response.Data.DivisionId,
            ExpenseDate = response.Data.ExpenseDate,
            Amount = response.Data.Amount,
            Currency = response.Data.Currency,
            Vendor = response.Data.Vendor,
            Reference = response.Data.Reference,
            Notes = response.Data.Notes
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Expense";
        ViewData["PageSubtitle"] = response.Data.Code;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ExpenseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Expense";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Description = model.Description,
            Category = model.Category,
            ProjectId = model.ProjectId,
            DivisionId = model.DivisionId,
            ExpenseDate = model.ExpenseDate,
            Amount = model.Amount,
            Currency = model.Currency,
            Vendor = model.Vendor,
            Reference = model.Reference,
            Notes = model.Notes
        };

        var response = await _api.PutAsync<ApiResult<ExpenseListItem>>($"/api/Expenses/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update expense.");
            ViewData["PageTitle"] = "Edit Expense";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Expense updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Expenses/{id}", Token);
        TempData["Success"] = "Expense deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(ExpenseViewModel vm)
    {
        var projectsTask = _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        var divisionsTask = _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);
        await Task.WhenAll(projectsTask, divisionsTask);

        vm.Projects = (projectsTask.Result?.Data ?? new List<ProjectListItem>())
            .Select(p => new ProjectOption
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.DivisionName
            }).ToList();

        vm.Divisions = divisionsTask.Result?.Data ?? new List<DivisionOption>();
    }
}