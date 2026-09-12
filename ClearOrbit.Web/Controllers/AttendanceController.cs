using System.Security.Claims;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly IApiClient _api;
    public AttendanceController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Attendance";
        ViewData["PageSubtitle"] = "Attendance overview across all Academy classes.";

        var classesResp = await _api.GetAsync<ApiResult<List<ClassListItem>>>("/api/Classes", Token);
        var classes = (classesResp?.Success == true ? classesResp.Data : null) ?? new List<ClassListItem>();

        var hub = new List<AttendanceHubRow>();
        foreach (var c in classes)
        {
            var sessionsResp = await _api.GetAsync<ApiResult<List<SessionSummaryItem>>>(
                $"/api/Attendance/class/{c.Id}/sessions", Token);
            var sessions = sessionsResp?.Data ?? new List<SessionSummaryItem>();

            var totalRecords = sessions.Sum(s => s.TotalRecords);
            var totalAttended = sessions.Sum(s => s.TotalPresent + s.TotalLate);

            hub.Add(new AttendanceHubRow
            {
                ClassId = c.Id,
                ClassCode = c.Code,
                ClassName = c.Name,
                Subject = c.Subject,
                GradeLevelName = c.GradeLevelName,
                TutorName = c.TutorName,
                DayOfWeekName = c.DayOfWeekName,
                StartTime = c.StartTime,
                EnrolledCount = c.EnrolledCount,
                SessionCount = sessions.Count,
                AttendanceRate = totalRecords == 0
                    ? 0
                    : Math.Round((decimal)totalAttended / totalRecords * 100, 1),
                LastSessionDate = sessions.OrderByDescending(s => s.SessionDate).FirstOrDefault()?.SessionDate
            });
        }

        return View(hub);
    }

    [HttpGet]
    public async Task<IActionResult> Take(Guid? classId = null, DateTime? date = null)
    {
        // No class selected — send the user back to the hub
        if (!classId.HasValue || classId.Value == Guid.Empty)
            return RedirectToAction(nameof(Index));

        var sessionDate = (date ?? DateTime.UtcNow.Date).Date;

        var classResp = await _api.GetAsync<ApiResult<ClassListItem>>($"/api/Classes/{classId.Value}", Token);
        if (classResp is null || !classResp.Success || classResp.Data is null) return NotFound();

        var enrollResp = await _api.GetAsync<ApiResult<List<EnrollmentListItem>>>($"/api/Classes/{classId.Value}/enrollments", Token);
        var enrollments = (enrollResp?.Data ?? new List<EnrollmentListItem>())
            .Where(e => e.StatusName == "Active")
            .ToList();

        var existingResp = await _api.GetAsync<ApiResult<List<AttendanceListItem>>>(
            $"/api/Attendance/class/{classId.Value}/date/{sessionDate:yyyy-MM-dd}", Token);
        var existing = existingResp?.Data ?? new List<AttendanceListItem>();

        var rows = enrollments.Select(e =>
        {
            var found = existing.FirstOrDefault(a => a.LearnerId == e.LearnerId);
            return new TakeAttendanceRow
            {
                LearnerId = e.LearnerId,
                LearnerCode = e.LearnerCode,
                LearnerName = e.LearnerName,
                Status = found?.Status ?? 1,
                Notes = found?.Notes
            };
        }).ToList();

        var vm = new TakeAttendanceViewModel
        {
            ClassId = classId.Value,
            ClassCode = classResp.Data.Code,
            ClassName = classResp.Data.Name,
            SessionDate = sessionDate,
            Rows = rows
        };

        ViewData["PageTitle"] = "Take Attendance";
        ViewData["PageSubtitle"] = $"{vm.ClassCode} — {vm.ClassName}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Take(TakeAttendanceViewModel model)
    {
        var payload = new
        {
            ClassId = model.ClassId,
            SessionDate = model.SessionDate,
            Records = model.Rows.Select(r => new
            {
                LearnerId = r.LearnerId,
                Status = r.Status,
                Notes = r.Notes
            }).ToList()
        };

        var response = await _api.PostAsync<ApiResult<bool>>("/api/Attendance/save", payload, Token);
        if (response is null || !response.Success)
        {
            TempData["Error"] = response?.Message ?? "Failed to save attendance.";
            return RedirectToAction(nameof(Take), new { classId = model.ClassId, date = model.SessionDate.ToString("yyyy-MM-dd") });
        }

        TempData["Success"] = response.Message ?? "Attendance saved.";
        return RedirectToAction("Details", "Classes", new { id = model.ClassId });
    }
}