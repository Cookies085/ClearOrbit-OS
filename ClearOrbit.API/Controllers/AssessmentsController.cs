using System.Security.Claims;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _service;
    public AssessmentsController(IAssessmentService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentDto request)
    {
        var result = await _service.CreateAsync(request);
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

    [HttpGet("class/{classId:guid}")]
    public async Task<IActionResult> GetByClass(Guid classId)
        => Ok(await _service.GetByClassAsync(classId));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssessmentDto request)
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

    [HttpGet("{id:guid}/results")]
    public async Task<IActionResult> GetResults(Guid id)
        => Ok(await _service.GetResultsAsync(id));

    [HttpPost("results")]
    public async Task<IActionResult> SaveResults([FromBody] SaveResultsDto request)
    {
        Guid? userId = null;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(sub, out var parsed)) userId = parsed;

        var result = await _service.SaveResultsAsync(request, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}