using System.Security.Claims;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ClientsController : Controller
{
    private readonly IApiClient _api;

    public ClientsController(IApiClient api)
    {
        _api = api;
    }

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Clients";
        ViewData["PageSubtitle"] = "Everyone ClearOrbit Group serves.";

        var response = await _api.GetAsync<ApiResult<List<ClientListItem>>>("/api/Clients", Token);
        var clients = response?.Data ?? new List<ClientListItem>();
        return View(clients);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "New Client";
        ViewData["PageSubtitle"] = "Add a new client to the group.";
        return View(new ClientViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Client";
            ViewData["PageSubtitle"] = "Add a new client to the group.";
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Industry = model.Industry,
            Website = model.Website,
            Notes = model.Notes
        };

        var response = await _api.PostAsync<ApiResult<ClientListItem>>("/api/Clients", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create client.");
            ViewData["PageTitle"] = "New Client";
            ViewData["PageSubtitle"] = "Add a new client to the group.";
            return View(model);
        }

        TempData["Success"] = "Client created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ClientListItem>>($"/api/Clients/{id}", Token);
        if (response is null || !response.Success || response.Data is null)
            return NotFound();

        var vm = new ClientViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Industry = response.Data.Industry,
            Website = response.Data.Website
        };

        ViewData["PageTitle"] = "Edit Client";
        ViewData["PageSubtitle"] = vm.Name;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ClientViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Client";
            ViewData["PageSubtitle"] = model.Name;
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Industry = model.Industry,
            Website = model.Website,
            Notes = model.Notes
        };

        var response = await _api.PutAsync<ApiResult<ClientListItem>>($"/api/Clients/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update client.");
            ViewData["PageTitle"] = "Edit Client";
            ViewData["PageSubtitle"] = model.Name;
            return View(model);
        }

        TempData["Success"] = "Client updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Clients/{id}", Token);
        TempData["Success"] = "Client deactivated.";
        return RedirectToAction(nameof(Index));
    }
}