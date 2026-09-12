using System.Security.Claims;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;
    public AttendanceController(IAttendanceService service) => _service = service;

    [HttpGet("class/{classId:guid}")]
    public async Task<IActionResult> GetByClass(Guid classId)
        => Ok(await _service.GetByClassAsync(classId));

    [HttpGet("class/{classId:guid}/sessions")]
    public async Task<IActionResult> GetSessions(Guid classId)
        => Ok(await _service.GetSessionSummaryAsync(classId));

    [HttpGet("class/{classId:guid}/date/{date}")]
    public async Task<IActionResult> GetByDate(Guid classId, DateTime date)
        => Ok(await _service.GetByClassAndDateAsync(classId, date));

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] SaveAttendanceDto request)
    {
        Guid? userId = null;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(sub, out var parsed)) userId = parsed;

        var result = await _service.SaveAsync(request, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}