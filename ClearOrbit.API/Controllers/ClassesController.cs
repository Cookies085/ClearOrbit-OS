using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _service;
    public ClassesController(IClassService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassDto request)
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassDto request)
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

    [HttpGet("{id:guid}/enrollments")]
    public async Task<IActionResult> GetEnrollments(Guid id)
        => Ok(await _service.GetEnrollmentsAsync(id));

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto request)
    {
        var result = await _service.EnrollLearnerAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("enrollments/{enrollmentId:guid}")]
    public async Task<IActionResult> Withdraw(Guid enrollmentId)
    {
        var result = await _service.WithdrawLearnerAsync(enrollmentId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}