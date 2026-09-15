using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class BugsController : Controller
{
    private readonly IApiClient _api;
    public BugsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Bugs";
        ViewData["PageSubtitle"] = "Every issue we've tracked across all divisions.";
        var response = await _api.GetAsync<ApiResult<List<BugListItem>>>("/api/Bugs", Token);
        return View(response?.Data ?? new List<BugListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<BugListItem>>($"/api/Bugs/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        ViewData["PageTitle"] = $"{response.Data.Code} — {response.Data.Title}";
        ViewData["PageSubtitle"] = response.Data.DivisionName;
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null, Guid? featureId = null)
    {
        ViewData["PageTitle"] = "New Bug";
        var vm = new BugViewModel();
        if (projectId.HasValue) vm.ProjectId = projectId.Value;
        if (featureId.HasValue) vm.FeatureId = featureId.Value;
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BugViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Bug";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            StepsToReproduce = model.StepsToReproduce,
            ExpectedBehavior = model.ExpectedBehavior,
            ActualBehavior = model.ActualBehavior,
            Severity = model.Severity,
            Priority = model.Priority,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            FeatureId = model.FeatureId
        };

        var response = await _api.PostAsync<ApiResult<BugListItem>>("/api/Bugs", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to report bug.");
            ViewData["PageTitle"] = "New Bug";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Bug {response.Data?.Code} reported.";
        return RedirectToAction(nameof(Details), new { id = response.Data!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<BugListItem>>($"/api/Bugs/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new BugViewModel
        {
            Id = response.Data.Id,
            Code = response.Data.Code,
            Title = response.Data.Title,
            Description = response.Data.Description,
            StepsToReproduce = response.Data.StepsToReproduce,
            ExpectedBehavior = response.Data.ExpectedBehavior,
            ActualBehavior = response.Data.ActualBehavior,
            DivisionId = response.Data.DivisionId,
            ProjectId = response.Data.ProjectId,
            FeatureId = response.Data.FeatureId,
            Status = response.Data.Status,
            Severity = response.Data.Severity,
            Priority = response.Data.Priority
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Bug";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Title}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, BugViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Bug";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            StepsToReproduce = model.StepsToReproduce,
            ExpectedBehavior = model.ExpectedBehavior,
            ActualBehavior = model.ActualBehavior,
            Status = model.Status,
            Severity = model.Severity,
            Priority = model.Priority,
            DivisionId = model.DivisionId,
            ProjectId = model.ProjectId,
            FeatureId = model.FeatureId
        };

        var response = await _api.PutAsync<ApiResult<BugListItem>>($"/api/Bugs/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update bug.");
            ViewData["PageTitle"] = "Edit Bug";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Bug updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdowns(BugViewModel vm)
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
        vm.Features = (featTask.Result?.Data ?? new List<FeatureListItem>())
            .Select(f => new FeatureOption
            {
                Id = f.Id,
                Code = f.Code,
                Title = f.Title
            }).ToList();
    }
}