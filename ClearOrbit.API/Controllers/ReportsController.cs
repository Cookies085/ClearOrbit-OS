using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _service;
    public ReportsController(IReportsService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary() => Ok(await _service.GetSummaryAsync());

    [HttpGet("divisions")]
    public async Task<IActionResult> Divisions() => Ok(await _service.GetDivisionProfitabilityAsync());

    [HttpGet("clients")]
    public async Task<IActionResult> Clients() => Ok(await _service.GetClientProfitabilityAsync());

    [HttpGet("projects")]
    public async Task<IActionResult> Projects() => Ok(await _service.GetProjectProfitabilityAsync());
}