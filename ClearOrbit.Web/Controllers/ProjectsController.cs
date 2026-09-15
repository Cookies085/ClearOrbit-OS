using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IApiClient _api;
    public ProjectsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Projects";
        ViewData["PageSubtitle"] = "Every unit of work across ClearOrbit Group.";

        var response = await _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        return View(response?.Data ?? new List<ProjectListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var projectResp = await _api.GetAsync<ApiResult<ProjectListItem>>($"/api/Projects/{id}", Token);
        if (projectResp is null || !projectResp.Success || projectResp.Data is null) return NotFound();

        var featuresTask = _api.GetAsync<ApiResult<List<FeatureListItem>>>($"/api/Features/project/{id}", Token);
        var bugsTask = _api.GetAsync<ApiResult<List<BugListItem>>>($"/api/Bugs/project/{id}", Token);
        var releasesTask = _api.GetAsync<ApiResult<List<ReleaseListItem>>>($"/api/Releases/project/{id}", Token);
        var invoicesTask = _api.GetAsync<ApiResult<List<InvoiceListItem>>>("/api/Invoices", Token);
        var expensesTask = _api.GetAsync<ApiResult<List<ExpenseListItem>>>($"/api/Expenses/project/{id}", Token);

        await Task.WhenAll(featuresTask, bugsTask, releasesTask, invoicesTask, expensesTask);

        ViewData["Features"] = featuresTask.Result?.Data ?? new List<FeatureListItem>();
        ViewData["Bugs"] = bugsTask.Result?.Data ?? new List<BugListItem>();
        ViewData["Releases"] = releasesTask.Result?.Data ?? new List<ReleaseListItem>();
        ViewData["Invoices"] = (invoicesTask.Result?.Data ?? new List<InvoiceListItem>())
            .Where(i => i.ProjectId == id).ToList();
        ViewData["Expenses"] = expensesTask.Result?.Data ?? new List<ExpenseListItem>();

        ViewData["PageTitle"] = $"{projectResp.Data.Code} — {projectResp.Data.Name}";
        ViewData["PageSubtitle"] = projectResp.Data.DivisionName;
        return View(projectResp.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "New Project";
        ViewData["PageSubtitle"] = "Start a new unit of work.";
        var vm = new ProjectViewModel();
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Project";
            ViewData["PageSubtitle"] = "Start a new unit of work.";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            Type = model.Type,
            ClientId = model.ClientId,
            DivisionId = model.DivisionId,
            ServiceId = model.ServiceId,
            RequestingDivisionId = model.RequestingDivisionId,
            Budget = model.Budget,
            Currency = model.Currency,
            StartDate = model.StartDate,
            DueDate = model.DueDate
        };

        var response = await _api.PostAsync<ApiResult<ProjectListItem>>("/api/Projects", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create project.");
            ViewData["PageTitle"] = "New Project";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Project {response.Data?.Code} created.";
        return RedirectToAction(nameof(Details), new { id = response.Data!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ProjectListItem>>($"/api/Projects/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ProjectViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Description = response.Data.Description,
            Type = response.Data.Type,
            Status = response.Data.Status,
            ClientId = response.Data.ClientId,
            DivisionId = response.Data.DivisionId,
            ServiceId = response.Data.ServiceId,
            RequestingDivisionId = response.Data.RequestingDivisionId,
            Budget = response.Data.Budget,
            Currency = response.Data.Currency,
            StartDate = response.Data.StartDate,
            DueDate = response.Data.DueDate
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Project";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Name}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Project";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            Status = model.Status,
            ClientId = model.ClientId,
            DivisionId = model.DivisionId,
            ServiceId = model.ServiceId,
            RequestingDivisionId = model.RequestingDivisionId,
            Budget = model.Budget,
            Currency = model.Currency,
            StartDate = model.StartDate,
            DueDate = model.DueDate
        };

        var response = await _api.PutAsync<ApiResult<ProjectListItem>>($"/api/Projects/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update project.");
            ViewData["PageTitle"] = "Edit Project";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Project updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdowns(ProjectViewModel vm)
    {
        var clientsTask = _api.GetAsync<ApiResult<List<ClientListItem>>>("/api/Clients", Token);
        var divisionsTask = _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);
        var servicesTask = _api.GetAsync<ApiResult<List<ServiceListItem>>>("/api/Services", Token);

        await Task.WhenAll(clientsTask, divisionsTask, servicesTask);

        vm.Clients = (clientsTask.Result?.Data ?? new List<ClientListItem>())
            .Select(c => new ClientOption { Id = c.Id, Name = c.Name }).ToList();

        vm.Divisions = divisionsTask.Result?.Data ?? new List<DivisionOption>();

        vm.Services = (servicesTask.Result?.Data ?? new List<ServiceListItem>())
            .Select(s => new ServiceOption { Id = s.Id, Name = s.Name, DivisionId = s.DivisionId }).ToList();
    }
}