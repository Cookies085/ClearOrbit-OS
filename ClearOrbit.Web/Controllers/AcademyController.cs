using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class AcademyController : Controller
{
    private readonly IApiClient _api;
    public AcademyController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Academy";
        ViewData["PageSubtitle"] = "Explore Knowledge. Build the Future.";

        var learnersTask = _api.GetAsync<ApiResult<List<LearnerListItem>>>("/api/Learners", Token);
        var tutorsTask = _api.GetAsync<ApiResult<List<TutorListItem>>>("/api/Tutors", Token);
        var classesTask = _api.GetAsync<ApiResult<List<ClassListItem>>>("/api/Classes", Token);

        await Task.WhenAll(learnersTask, tutorsTask, classesTask);

        var learners = learnersTask.Result?.Data ?? new List<LearnerListItem>();
        var tutors = tutorsTask.Result?.Data ?? new List<TutorListItem>();
        var classes = classesTask.Result?.Data ?? new List<ClassListItem>();

        var vm = new AcademyWorkspaceViewModel
        {
            LearnerCount = learners.Count,
            TutorCount = tutors.Count,
            ClassCount = classes.Count,
            TotalEnrollments = classes.Sum(c => c.EnrolledCount),
            RecentLearners = learners.Take(5).ToList(),
            RecentTutors = tutors.Take(5).ToList(),
            RecentClasses = classes.Take(5).ToList()
        };

        var totalSessions = 0;
        foreach (var c in classes)
        {
            var s = await _api.GetAsync<ApiResult<List<SessionSummaryItem>>>($"/api/Attendance/class/{c.Id}/sessions", Token);
            totalSessions += (s?.Data ?? new List<SessionSummaryItem>()).Count;
        }
        vm.SessionCount = totalSessions;

        return View(vm);
    }
}