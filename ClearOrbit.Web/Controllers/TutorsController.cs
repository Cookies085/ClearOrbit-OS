using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class TutorsController : Controller
{
    private readonly IApiClient _api;
    public TutorsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Tutors";
        ViewData["PageSubtitle"] = "Academy instructors across ClearOrbit.";
        var response = await _api.GetAsync<ApiResult<List<TutorListItem>>>("/api/Tutors", Token);
        return View(response?.Data ?? new List<TutorListItem>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "New Tutor";
        ViewData["PageSubtitle"] = "Add an Academy instructor.";
        return View(new TutorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TutorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Tutor";
            return View(model);
        }

        var payload = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Specializations = model.Specializations,
            Bio = model.Bio
        };

        var response = await _api.PostAsync<ApiResult<TutorListItem>>("/api/Tutors", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to add tutor.");
            ViewData["PageTitle"] = "New Tutor";
            return View(model);
        }

        TempData["Success"] = $"{response.Data?.FullName} added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<TutorListItem>>($"/api/Tutors/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new TutorViewModel
        {
            Id = response.Data.Id,
            FirstName = response.Data.FirstName,
            LastName = response.Data.LastName,
            Email = response.Data.Email,
            Phone = response.Data.Phone,
            Specializations = response.Data.Specializations,
            IsActive = response.Data.IsActive
        };
        ViewData["PageTitle"] = "Edit Tutor";
        ViewData["PageSubtitle"] = response.Data.FullName;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TutorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Tutor";
            return View(model);
        }

        var payload = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Specializations = model.Specializations,
            Bio = model.Bio,
            IsActive = model.IsActive
        };

        var response = await _api.PutAsync<ApiResult<TutorListItem>>($"/api/Tutors/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update tutor.");
            ViewData["PageTitle"] = "Edit Tutor";
            return View(model);
        }

        TempData["Success"] = "Tutor updated.";
        return RedirectToAction(nameof(Index));
    }
}