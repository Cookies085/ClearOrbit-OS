using System.Security.Claims;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IApiClient _api;
    public ReportsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Reports";
        ViewData["PageSubtitle"] = "Profitability and receivables across the group.";

        var summaryTask = _api.GetAsync<ApiResult<ReportsSummary>>("/api/Reports/summary", Token);
        var divisionsTask = _api.GetAsync<ApiResult<List<DivisionProfitabilityItem>>>("/api/Reports/divisions", Token);
        var clientsTask = _api.GetAsync<ApiResult<List<ClientProfitabilityItem>>>("/api/Reports/clients", Token);
        var projectsTask = _api.GetAsync<ApiResult<List<ProjectProfitabilityItem>>>("/api/Reports/projects", Token);

        await Task.WhenAll(summaryTask, divisionsTask, clientsTask, projectsTask);

        var vm = new ReportsIndexViewModel
        {
            Summary = summaryTask.Result?.Data ?? new ReportsSummary(),
            Divisions = divisionsTask.Result?.Data ?? new List<DivisionProfitabilityItem>(),
            Clients = clientsTask.Result?.Data ?? new List<ClientProfitabilityItem>(),
            Projects = projectsTask.Result?.Data ?? new List<ProjectProfitabilityItem>()
        };

        return View(vm);
    }
}