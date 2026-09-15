using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ReleasesController : Controller
{
    private readonly IApiClient _api;
    public ReleasesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Releases";
        ViewData["PageSubtitle"] = "Every versioned shipment across all projects.";
        var response = await _api.GetAsync<ApiResult<List<ReleaseListItem>>>("/api/Releases", Token);
        return View(response?.Data ?? new List<ReleaseListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null)
    {
        ViewData["PageTitle"] = "New Release";
        var vm = new ReleaseViewModel();
        if (projectId.HasValue) vm.ProjectId = projectId.Value;
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReleaseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Release";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Version = model.Version,
            Name = model.Name,
            Description = model.Description,
            ReleaseNotes = model.ReleaseNotes,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            PlannedDate = model.PlannedDate,
            FeatureIds = model.FeatureIds
        };

        var response = await _api.PostAsync<ApiResult<ReleaseListItem>>("/api/Releases", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create release.");
            ViewData["PageTitle"] = "New Release";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Release {response.Data?.Code} created.";
        return RedirectToAction(nameof(Details), new { id = response.Data!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ReleaseListItem>>($"/api/Releases/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        ViewData["PageTitle"] = $"{response.Data.Version} — {response.Data.Name}";
        ViewData["PageSubtitle"] = response.Data.Code;
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ReleaseListItem>>($"/api/Releases/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ReleaseViewModel
        {
            Id = response.Data.Id,
            Code = response.Data.Code,
            Version = response.Data.Version,
            Name = response.Data.Name,
            Description = response.Data.Description,
            ReleaseNotes = response.Data.ReleaseNotes,
            Status = response.Data.Status,
            DivisionId = response.Data.DivisionId,
            ProjectId = response.Data.ProjectId,
            PlannedDate = response.Data.PlannedDate,
            FeatureIds = response.Data.Features.Select(f => f.Id).ToList()
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Release";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Version} {response.Data.Name}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ReleaseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Release";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Version = model.Version,
            Name = model.Name,
            Description = model.Description,
            ReleaseNotes = model.ReleaseNotes,
            Status = model.Status,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            PlannedDate = model.PlannedDate,
            FeatureIds = model.FeatureIds
        };

        var response = await _api.PutAsync<ApiResult<ReleaseListItem>>($"/api/Releases/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update release.");
            ViewData["PageTitle"] = "Edit Release";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Release updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdowns(ReleaseViewModel vm)
    {
        var divTask = _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);
        var projTask = _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        var featTask = _api.GetAsync<ApiResult<List<FeatureListItem>>>("/api/Features", Token);

        await Task.WhenAll(divTask, projTask, featTask);

        vm.Divisions = divTask.Result?.Data ?? new List<DivisionOption>();
        vm.Projects = (projTask.Result?.Data ?? new List<ProjectListItem>())
            .Select(p => new ProjectOption
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.DivisionName
            }).ToList();
        vm.AvailableFeatures = (featTask.Result?.Data ?? new List<FeatureListItem>())
            .Select(f => new FeatureOption
            {
                Id = f.Id,
                Code = f.Code,
                Title = f.Title
            }).ToList();
    }
}