using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class AssessmentsController : Controller
{
    private readonly IApiClient _api;
    public AssessmentsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Assessments";
        ViewData["PageSubtitle"] = "Tests, quizzes, exams across all classes.";
        var response = await _api.GetAsync<ApiResult<List<AssessmentListItem>>>("/api/Assessments", Token);
        return View(response?.Data ?? new List<AssessmentListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? classId = null)
    {
        ViewData["PageTitle"] = "New Assessment";
        var vm = new AssessmentViewModel();
        if (classId.HasValue) vm.ClassId = classId.Value;
        await LoadClasses(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssessmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "New Assessment";
            await LoadClasses(model);
            return View(model);
        }

        var payload = new
        {
            ClassId = model.ClassId,
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            ScheduledDate = model.ScheduledDate,
            MaxScore = model.MaxScore,
            Weight = model.Weight
        };

        var response = await _api.PostAsync<ApiResult<AssessmentListItem>>("/api/Assessments", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create assessment.");
            ViewData["PageTitle"] = "New Assessment";
            await LoadClasses(model);
            return View(model);
        }

        TempData["Success"] = $"Assessment {response.Data?.Code} created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<AssessmentListItem>>($"/api/Assessments/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var vm = new AssessmentViewModel
        {
            Id = response.Data.Id,
            Title = response.Data.Title,
            Description = response.Data.Description,
            ClassId = response.Data.ClassId,
            ClassCode = response.Data.ClassCode,
            ClassName = response.Data.ClassName,
            Type = response.Data.Type,
            Status = response.Data.Status,
            ScheduledDate = response.Data.ScheduledDate,
            MaxScore = response.Data.MaxScore,
            Weight = response.Data.Weight
        };
        await LoadClasses(vm);
        ViewData["PageTitle"] = "Edit Assessment";
        ViewData["PageSubtitle"] = $"{response.Data.Code} — {response.Data.Title}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AssessmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Edit Assessment";
            await LoadClasses(model);
            return View(model);
        }

        var payload = new
        {
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            Status = model.Status,
            ScheduledDate = model.ScheduledDate,
            MaxScore = model.MaxScore,
            Weight = model.Weight
        };

        var response = await _api.PutAsync<ApiResult<AssessmentListItem>>($"/api/Assessments/{id}", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to update assessment.");
            ViewData["PageTitle"] = "Edit Assessment";
            await LoadClasses(model);
            return View(model);
        }

        TempData["Success"] = "Assessment updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Record(Guid id)
    {
        // Load assessment
        var aResp = await _api.GetAsync<ApiResult<AssessmentListItem>>($"/api/Assessments/{id}", Token);
        if (aResp is null || !aResp.Success || aResp.Data is null) return NotFound();

        // Load active enrollments for the class
        var enrollResp = await _api.GetAsync<ApiResult<List<EnrollmentListItem>>>(
            $"/api/Classes/{aResp.Data.ClassId}/enrollments", Token);
        var enrollments = (enrollResp?.Data ?? new List<EnrollmentListItem>())
            .Where(e => e.StatusName == "Active")
            .ToList();

        // Load existing results
        var existingResp = await _api.GetAsync<ApiResult<List<ResultListItem>>>(
            $"/api/Assessments/{id}/results", Token);
        var existing = existingResp?.Data ?? new List<ResultListItem>();

        var rows = enrollments.Select(e =>
        {
            var r = existing.FirstOrDefault(x => x.LearnerId == e.LearnerId);
            return new RecordResultRow
            {
                LearnerId = e.LearnerId,
                LearnerCode = e.LearnerCode,
                LearnerName = e.LearnerName,
                Score = r?.Score,
                Notes = r?.Notes
            };
        }).ToList();

        var vm = new RecordResultsViewModel
        {
            AssessmentId = aResp.Data.Id,
            AssessmentCode = aResp.Data.Code,
            AssessmentTitle = aResp.Data.Title,
            ClassCode = aResp.Data.ClassCode,
            ClassName = aResp.Data.ClassName,
            MaxScore = aResp.Data.MaxScore,
            Rows = rows
        };

        ViewData["PageTitle"] = "Record Results";
        ViewData["PageSubtitle"] = $"{vm.AssessmentCode} — {vm.AssessmentTitle}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Record(RecordResultsViewModel model)
    {
        var payload = new
        {
            AssessmentId = model.AssessmentId,
            Results = model.Rows.Select(r => new
            {
                LearnerId = r.LearnerId,
                Score = r.Score,
                Notes = r.Notes
            }).ToList()
        };

        var response = await _api.PostAsync<ApiResult<bool>>("/api/Assessments/results", payload, Token);
        if (response is null || !response.Success)
        {
            TempData["Error"] = response?.Message ?? "Failed to save results.";
            return RedirectToAction(nameof(Record), new { id = model.AssessmentId });
        }

        TempData["Success"] = response.Message ?? "Results saved.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadClasses(AssessmentViewModel vm)
    {
        var response = await _api.GetAsync<ApiResult<List<ClassListItem>>>("/api/Classes", Token);
        vm.Classes = (response?.Data ?? new List<ClassListItem>())
            .Select(c => new ClassOption
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            }).ToList();
    }
}