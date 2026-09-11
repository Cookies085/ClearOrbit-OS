using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ClassesController : Controller
{
    private readonly IApiClient _api;
    public ClassesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Classes";
        ViewData["PageSubtitle"] = "Every Academy class offering.";
        var response = await _api.GetAsync<ApiResult<List<ClassListItem>>>("/api/Classes", Token);
        return View(response?.Data ?? new List<ClassListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var classResp = await _api.GetAsync<ApiResult<ClassListItem>>($"/api/Classes/{id}", Token);
        if (classResp is null || !classResp.Success || classResp.Data is null) return NotFound();

        var enrollResp = await _api.GetAsync<ApiResult<List<EnrollmentListItem>>>($"/api/Classes/{id}/enrollments", Token);
        var learnersResp = await _api.GetAsync<ApiResult<List<LearnerListItem>>>("/api/Learners", Token);

        ViewData["PageTitle"] = classResp.Data.Code;
        ViewData["PageSubtitle"] = classResp.Data.Name;
        ViewData["Enrollments"] = enrollResp?.Data ?? new List<EnrollmentListItem>();
        ViewData["AllLearners"] = learnersResp?.Data ?? new List<LearnerListItem>();
        return View(classResp.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "New Class";
        var vm = new ClassViewModel();
        await LoadTutors(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Class";
            await LoadTutors(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            Subject = model.Subject,
            GradeLevel = model.GradeLevel,
            TutorId = model.TutorId,
            DayOfWeek = model.DayOfWeek,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Location = model.Location,
            MaxCapacity = model.MaxCapacity,
            FeePerLearner = model.FeePerLearner,
            Currency = model.Currency
        };

        var response = await _api.PostAsync<ApiResult<ClassListItem>>("/api/Classes", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create class.");
            ViewData["PageTitle"] = "New Class";
            await LoadTutors(model);
            return View(model);
        }

        TempData["Success"] = $"Class {response.Data?.Code} created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<ClassListItem>>($"/api/Classes/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new ClassViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Description = response.Data.Description,
            Subject = response.Data.Subject,
            GradeLevel = response.Data.GradeLevel,
            TutorId = response.Data.TutorId,
            DayOfWeek = response.Data.DayOfWeek,
            StartTime = TimeOnly.Parse(response.Data.StartTime),
            EndTime = TimeOnly.Parse(response.Data.EndTime),
            StartDate = response.Data.StartDate,
            EndDate = response.Data.EndDate,
            Location = response.Data.Location,
            MaxCapacity = response.Data.MaxCapacity,
            FeePerLearner = response.Data.FeePerLearner,
            Currency = response.Data.Currency,
            Status = response.Data.Status
        };
        await LoadTutors(vm);
        ViewData["PageTitle"] = "Edit Class";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Name}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ClassViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Class";
            await LoadTutors(model);
            return View(model);
        }

        var payload = new
        {
            Name = model.Name,
            Description = model.Description,
            Subject = model.Subject,
            GradeLevel = model.GradeLevel,
            TutorId = model.TutorId,
            DayOfWeek = model.DayOfWeek,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Location = model.Location,
            MaxCapacity = model.MaxCapacity,
            FeePerLearner = model.FeePerLearner,
            Currency = model.Currency,
            Status = model.Status
        };

        var response = await _api.PutAsync<ApiResult<ClassListItem>>($"/api/Classes/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update class.");
            ViewData["PageTitle"] = "Edit Class";
            await LoadTutors(model);
            return View(model);
        }

        TempData["Success"] = "Class updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(Guid classId, Guid learnerId)
    {
        var payload = new { ClassId = classId, LearnerId = learnerId };
        var response = await _api.PostAsync<ApiResult<EnrollmentListItem>>("/api/Classes/enroll", payload, Token);
        if (response is null || !response.Success)
            TempData["Error"] = response?.Message ?? "Failed to enroll learner.";
        else
            TempData["Success"] = $"{response.Data?.LearnerName} enrolled.";

        return RedirectToAction(nameof(Details), new { id = classId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(Guid enrollmentId, Guid classId)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Classes/enrollments/{enrollmentId}", Token);
        TempData["Success"] = "Learner withdrawn.";
        return RedirectToAction(nameof(Details), new { id = classId });
    }

    private async Task LoadTutors(ClassViewModel vm)
    {
        var response = await _api.GetAsync<ApiResult<List<TutorListItem>>>("/api/Tutors", Token);
        vm.Tutors = (response?.Data ?? new List<TutorListItem>())
            .Select(t => new TutorOption
            {
                Id = t.Id,
                FullName = t.FullName,
                Specializations = t.Specializations
            }).ToList();
    }
}