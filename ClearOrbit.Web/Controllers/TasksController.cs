using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly IApiClient _api;
    public TasksController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Tasks";
        ViewData["PageSubtitle"] = "Every work item across the group.";
        var response = await _api.GetAsync<ApiResult<List<TaskListItem>>>("/api/Tasks", Token);
        return View(response?.Data ?? new List<TaskListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null)
    {
        ViewData["PageTitle"] = "New Task";
        var vm = new TaskViewModel();
        if (projectId.HasValue) vm.ProjectId = projectId.Value;
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Task";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            ProjectId = model.ProjectId,
            AssignedToUserId = model.AssignedToUserId,
            DueDate = model.DueDate
        };

        var response = await _api.PostAsync<ApiResult<TaskListItem>>("/api/Tasks", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create task.");
            ViewData["PageTitle"] = "New Task";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Task created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<TaskListItem>>($"/api/Tasks/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new TaskViewModel
        {
            Id = response.Data.Id,
            Title = response.Data.Title,
            Description = response.Data.Description,
            ProjectId = response.Data.ProjectId,
            Status = response.Data.Status,
            Priority = response.Data.Priority,
            AssignedToUserId = response.Data.AssignedToUserId,
            DueDate = response.Data.DueDate,
            ProjectCode = response.Data.ProjectCode,
            ProjectName = response.Data.ProjectName
        };
        await LoadDropdowns(vm);
        ViewData["PageTitle"] = "Edit Task";
        ViewData["PageSubtitle"] = vm.Title;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Task";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            Status = model.Status,
            Priority = model.Priority,
            AssignedToUserId = model.AssignedToUserId,
            DueDate = model.DueDate
        };

        var response = await _api.PutAsync<ApiResult<TaskListItem>>($"/api/Tasks/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update task.");
            ViewData["PageTitle"] = "Edit Task";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = "Task updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Tasks/{id}", Token);
        TempData["Success"] = "Task deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(TaskViewModel vm)
    {
        var projectsTask = _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        // We'll use the auth API to list users — add endpoint shortly, or skip if not available
        await Task.WhenAll(projectsTask);

        vm.Projects = (projectsTask.Result?.Data ?? new List<ProjectListItem>())
            .Select(p => new ProjectOption
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.DivisionName
            }).ToList();

        // Placeholder for users — filled when we add /api/Users (see note)
        vm.Users = new List<UserOption>();
    }
}