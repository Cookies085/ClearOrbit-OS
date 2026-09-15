using ClearOrbit.Application.DTOs.Contacts;
using ClearOrbit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly IContactService _service;
    public ContactsController(IContactService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactDto request)
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

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
        => Ok(await _service.GetByClientAsync(clientId));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactDto request)
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