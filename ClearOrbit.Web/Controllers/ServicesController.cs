using System.Security.Claims;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ServicesController : Controller
{
    private readonly IApiClient _api;
    public ServicesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Services";
        ViewData["PageSubtitle"] = "What ClearOrbit Group provides across all divisions.";

        var response = await _api.GetAsync<ApiResult<List<ServiceListItem>>>("/api/Services", Token);
        return View(response?.Data ?? new List<ServiceListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "New Service";
        ViewData["PageSubtitle"] = "Add a service to a division.";
        var vm = new ServiceViewModel();
        await LoadDivisions(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDivisions(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            BasePrice = model.BasePrice,
            Currency = model.Currency,
            Unit = model.Unit,
            DivisionId = model.DivisionId
        };

        var response = await _api.PostAsync<ApiResult<ServiceListItem>>("/api/Services", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create service.");
            await LoadDivisions(model);
            return View(model);
        }

        TempData["Success"] = "Service created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ServiceListItem>>($"/api/Services/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ServiceViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Description = response.Data.Description,
            BasePrice = response.Data.BasePrice,
            Currency = response.Data.Currency,
            Unit = response.Data.Unit,
            DivisionId = response.Data.DivisionId,
            IsActive = response.Data.IsActive
        };
        await LoadDivisions(vm);
        ViewData["PageTitle"] = "Edit Service";
        ViewData["PageSubtitle"] = vm.Name;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ServiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Service";
            ViewData["PageSubtitle"] = model.Name;
            await LoadDivisions(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            BasePrice = model.BasePrice,
            Currency = model.Currency,
            Unit = model.Unit,
            DivisionId = model.DivisionId,
            IsActive = model.IsActive
        };

        var response = await _api.PutAsync<ApiResult<ServiceListItem>>($"/api/Services/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update service.");
            await LoadDivisions(model);
            return View(model);
        }

        TempData["Success"] = "Service updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Services/{id}", Token);
        TempData["Success"] = "Service deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDivisions(ServiceViewModel vm)
    {
        var response = await _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);
        vm.Divisions = response?.Data ?? new List<DivisionOption>();
    }
}