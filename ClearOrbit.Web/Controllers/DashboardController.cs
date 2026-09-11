using System.Security.Claims;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IApiClient _api;
    public DashboardController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Dashboard";
        ViewData["PageSubtitle"] = $"Welcome back, {User.FindFirst(ClaimTypes.Name)?.Value}";

        var response = await _api.GetAsync<ApiResult<DashboardSummary>>("/api/Dashboard/summary", Token);
        var summary = response?.Data ?? new DashboardSummary();
        return View(summary);
    }
}