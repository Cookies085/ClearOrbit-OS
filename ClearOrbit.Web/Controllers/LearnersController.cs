using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class LearnersController : Controller
{
    private readonly IApiClient _api;
    public LearnersController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Learners";
        ViewData["PageSubtitle"] = "Everyone learning with ClearOrbit Academy.";
        var response = await _api.GetAsync<ApiResult<List<LearnerListItem>>>("/api/Learners", Token);
        return View(response?.Data ?? new List<LearnerListItem>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "Enroll Learner";
        ViewData["PageSubtitle"] = "Add a new learner to the Academy.";
        return View(new LearnerViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LearnerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Enroll Learner";
            return View(model);
        }

        var payload = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            GradeLevel = model.GradeLevel,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            GuardianName = model.GuardianName,
            GuardianPhone = model.GuardianPhone,
            GuardianEmail = model.GuardianEmail,
            Notes = model.Notes
        };

        var response = await _api.PostAsync<ApiResult<LearnerListItem>>("/api/Learners", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to enroll learner.");
            ViewData["PageTitle"] = "Enroll Learner";
            return View(model);
        }

        TempData["Success"] = $"{response.Data?.FullName} enrolled.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<LearnerListItem>>($"/api/Learners/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new LearnerViewModel
        {
            Id = response.Data.Id,
            FirstName = response.Data.FirstName,
            LastName = response.Data.LastName,
            DateOfBirth = response.Data.DateOfBirth,
            GradeLevel = response.Data.GradeLevel,
            Status = response.Data.Status,
            Email = response.Data.Email,
            Phone = response.Data.Phone,
            GuardianName = response.Data.GuardianName,
            GuardianPhone = response.Data.GuardianPhone
        };
        ViewData["PageTitle"] = "Edit Learner";
        ViewData["PageSubtitle"] = response.Data.FullName;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LearnerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Learner";
            return View(model);
        }

        var payload = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            GradeLevel = model.GradeLevel,
            Status = model.Status,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            GuardianName = model.GuardianName,
            GuardianPhone = model.GuardianPhone,
            GuardianEmail = model.GuardianEmail,
            Notes = model.Notes
        };

        var response = await _api.PutAsync<ApiResult<LearnerListItem>>($"/api/Learners/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update learner.");
            ViewData["PageTitle"] = "Edit Learner";
            return View(model);
        }

        TempData["Success"] = "Learner updated.";
        return RedirectToAction(nameof(Index));
    }
}