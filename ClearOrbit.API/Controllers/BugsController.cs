using System.Security.Claims;
using ClearOrbit.Application.DTOs.Software;
using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BugsController : ControllerBase
{
    private readonly IBugService _service;
    public BugsController(IBugService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBugDto request)
    {
        Guid? userId = null;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(sub, out var parsed)) userId = parsed;

        var result = await _service.CreateAsync(request, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
        => Ok(await _service.GetByProjectAsync(projectId));

    [HttpGet("feature/{featureId:guid}")]
    public async Task<IActionResult> GetByFeature(Guid featureId)
        => Ok(await _service.GetByFeatureAsync(featureId));

    [HttpGet("division/{divisionId:guid}")]
    public async Task<IActionResult> GetByDivision(Guid divisionId)
        => Ok(await _service.GetByDivisionAsync(divisionId));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBugDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}