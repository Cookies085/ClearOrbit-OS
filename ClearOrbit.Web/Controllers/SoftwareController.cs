using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class SoftwareController : Controller
{
    private readonly IApiClient _api;
    public SoftwareController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Software";
        ViewData["PageSubtitle"] = "Ideas. Code. Solutions.";

        var featTask = _api.GetAsync<ApiResult<List<FeatureListItem>>>("/api/Features", Token);
        var bugTask = _api.GetAsync<ApiResult<List<BugListItem>>>("/api/Bugs", Token);
        var relTask = _api.GetAsync<ApiResult<List<ReleaseListItem>>>("/api/Releases", Token);

        await Task.WhenAll(featTask, bugTask, relTask);

        var features = featTask.Result?.Data ?? new List<FeatureListItem>();
        var bugs = bugTask.Result?.Data ?? new List<BugListItem>();
        var releases = relTask.Result?.Data ?? new List<ReleaseListItem>();

        var vm = new SoftwareWorkspaceViewModel
        {
            TotalFeatures = features.Count,
            ShippedFeatures = features.Count(f => f.StatusName == "Shipped"),
            InProgressFeatures = features.Count(f => f.StatusName == "InProgress" || f.StatusName == "Testing"),
            CriticalFeatures = features.Count(f => f.PriorityName == "Critical" && f.StatusName != "Shipped" && f.StatusName != "Rejected"),
            RecentFeatures = features.Take(5).ToList(),
            TotalBugs = bugs.Count,
            OpenBugs = bugs.Count(b => b.StatusName == "Open" || b.StatusName == "InProgress"),
            CriticalBugs = bugs.Count(b => b.SeverityName == "Critical" || b.SeverityName == "Blocker"),
            RecentBugs = bugs.Take(5).ToList(),
            TotalReleases = releases.Count,
            ReleasedCount = releases.Count(r => r.StatusName == "Released"),
            RecentReleases = releases.Take(5).ToList()
        };
        return View(vm);
    }
}