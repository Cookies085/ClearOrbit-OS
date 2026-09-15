using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IApiClient _api;
    public ContactsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Contacts";
        ViewData["PageSubtitle"] = "Every person we work with across all clients.";
        var response = await _api.GetAsync<ApiResult<List<ContactListItem>>>("/api/Contacts", Token);
        return View(response?.Data ?? new List<ContactListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? clientId = null)
    {
        var vm = new ContactViewModel();

        if (clientId.HasValue && clientId.Value != Guid.Empty)
        {
            var clientResp = await _api.GetAsync<ApiResult<ClientListItem>>($"/api/Clients/{clientId.Value}", Token);
            if (clientResp is null || !clientResp.Success || clientResp.Data is null) return NotFound();

            vm.ClientId = clientId.Value;
            vm.ClientName = clientResp.Data.Name;
            ViewData["PageSubtitle"] = $"For {clientResp.Data.Name}";
        }
        else
        {
            // No client selected — load a picker list
            var clientsResp = await _api.GetAsync<ApiResult<List<ClientListItem>>>("/api/Clients", Token);
            vm.ClientOptions = (clientsResp?.Data ?? new List<ClientListItem>())
                .Select(c => new ClientOption { Id = c.Id, Name = c.Name })
                .ToList();
            ViewData["PageSubtitle"] = "Pick a client, then add the contact.";
        }

        ViewData["PageTitle"] = "New Contact";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Contact";
            return View(model);
        }

        var payload = new
        {
            ClientId = model.ClientId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            JobTitle = model.JobTitle,
            Role = model.Role
        };

        var response = await _api.PostAsync<ApiResult<ContactListItem>>("/api/Contacts", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to add contact.");
            ViewData["PageTitle"] = "New Contact";
            return View(model);
        }

        TempData["Success"] = $"{response.Data?.FullName} added.";
        return RedirectToAction("Details", "Clients", new { id = model.ClientId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ContactListItem>>($"/api/Contacts/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ContactViewModel
        {
            Id = response.Data.Id,
            ClientId = response.Data.ClientId,
            ClientName = response.Data.ClientName,
            FirstName = response.Data.FirstName,
            LastName = response.Data.LastName,
            Email = response.Data.Email,
            Phone = response.Data.Phone,
            JobTitle = response.Data.JobTitle,
            Role = response.Data.Role
        };
        ViewData["PageTitle"] = "Edit Contact";
        ViewData["PageSubtitle"] = response.Data.FullName;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Contact";
            return View(model);
        }

        var payload = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            JobTitle = model.JobTitle,
            Role = model.Role
        };

        var response = await _api.PutAsync<ApiResult<ContactListItem>>($"/api/Contacts/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update contact.");
            ViewData["PageTitle"] = "Edit Contact";
            return View(model);
        }

        TempData["Success"] = "Contact updated.";
        return RedirectToAction("Details", "Clients", new { id = model.ClientId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid clientId)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Contacts/{id}", Token);
        TempData["Success"] = "Contact removed.";
        return RedirectToAction("Details", "Clients", new { id = clientId });
    }
}