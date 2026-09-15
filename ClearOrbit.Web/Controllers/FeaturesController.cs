using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class FeaturesController : Controller
{
    private readonly IApiClient _api;
    public FeaturesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Features";
        ViewData["PageSubtitle"] = "Everything we're building across all divisions.";
        var response = await _api.GetAsync<ApiResult<List<FeatureListItem>>>("/api/Features", Token);
        return View(response?.Data ?? new List<FeatureListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null)
    {
        ViewData["PageTitle"] = "New Feature";
        var vm = new FeatureViewModel();
        if (projectId.HasValue) vm.ProjectId = projectId.Value;
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeatureViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Feature";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            Source = model.Source,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            ClientId = model.ClientId,
            EstimatedEffort = model.EstimatedEffort,
            TargetDate = model.TargetDate
        };

        var response = await _api.PostAsync<ApiResult<FeatureListItem>>("/api/Features", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create feature.");
            ViewData["PageTitle"] = "New Feature";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Feature {response.Data?.Code} created.";
        return RedirectToAction(nameof(Details), new { id = response.Data!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<FeatureListItem>>($"/api/Features/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        // Load bugs linked to this feature
        var bugsResp = await _api.GetAsync<ApiResult<List<BugListItem>>>($"/api/Bugs/feature/{id}", Token);
        ViewData["Bugs"] = bugsResp?.Data ?? new List<BugListItem>();

        // Load releases this feature is part of (via the Releases endpoint)
        var releasesResp = await _api.GetAsync<ApiResult<List<ReleaseListItem>>>("/api/Releases", Token);
        var allReleases = releasesResp?.Data ?? new List<ReleaseListItem>();
        ViewData["Releases"] = allReleases.Where(r => r.Features.Any(f => f.Id == id)).ToList();

        ViewData["PageTitle"] = $"{response.Data.Code} — {response.Data.Title}";
        ViewData["PageSubtitle"] = response.Data.DivisionName;
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<FeatureListItem>>($"/api/Features/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new FeatureViewModel
        {
            Id = response.Data.Id,
            Code = response.Data.Code,
            Title = response.Data.Title,
            Description = response.Data.Description,
            DivisionId = response.Data.DivisionId,
            ProjectId = response.Data.ProjectId,
            ClientId = response.Data.ClientId,
            Source = response.Data.Source,
            Priority = response.Data.Priority,
            Status = response.Data.Status,
            EstimatedEffort = response.Data.EstimatedEffort,
            ActualEffort = response.Data.ActualEffort,
            TargetDate = response.Data.TargetDate
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Feature";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Title}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FeatureViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Feature";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            Status = model.Status,
            Priority = model.Priority,
            Source = model.Source,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            ClientId = model.ClientId,
            EstimatedEffort = model.EstimatedEffort,
            ActualEffort = model.ActualEffort,
            TargetDate = model.TargetDate
        };

        var response = await _api.PutAsync<ApiResult<FeatureListItem>>($"/api/Features/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update feature.");
            ViewData["PageTitle"] = "Edit Feature";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Feature updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdowns(FeatureViewModel vm)
    {
        var divTask = _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);
        var projTask = _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        var clientTask = _api.GetAsync<ApiResult<List<ClientListItem>>>("/api/Clients", Token);

        await Task.WhenAll(divTask, projTask, clientTask);

        vm.Divisions = divTask.Result?.Data ?? new List<DivisionOption>();
        vm.Projects = (projTask.Result?.Data ?? new List<ProjectListItem>())
            .Select(p => new ProjectOption
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.DivisionName
            }).ToList();
        vm.Clients = (clientTask.Result?.Data ?? new List<ClientListItem>())
            .Select(c => new ClientOption { Id = c.Id, Name = c.Name }).ToList();
    }
}