using Microsoft.AspNetCore.Mvc;
using backend.Application.DTOs;
using backend.Application.Interfaces;
using backend.Domain.Enums;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly IMaintenanceRequestService _service;

    public MaintenanceRequestsController(IMaintenanceRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetAllMaintenanceRequests(
        [FromQuery] string? searchTerm = null,
        [FromQuery] MaintenanceStatus? status = null)
    {
        try
        {
            IEnumerable<MaintenanceRequestDto> requests;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                requests = await _service.SearchMaintenanceRequestsAsync(searchTerm);
            }
            else if (status.HasValue)
            {
                requests = await _service.GetMaintenanceRequestsByStatusAsync(status.Value);
            }
            else
            {
                requests = await _service.GetAllMaintenanceRequestsAsync();
            }

            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceRequestDto>> GetMaintenanceRequest(int id)
    {
        try
        {
            var request = await _service.GetMaintenanceRequestByIdAsync(id);
            if (request == null)
                return NotFound($"Maintenance request with ID {id} not found.");

            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<MaintenanceRequestDto>> CreateMaintenanceRequest(
        [FromBody] CreateMaintenanceRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = await _service.CreateMaintenanceRequestAsync(dto);
            return CreatedAtAction(nameof(GetMaintenanceRequest), new { id = request.Id }, request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MaintenanceRequestDto>> UpdateMaintenanceRequest(
        int id,
        [FromBody] UpdateMaintenanceRequestDto dto,
        [FromQuery] UserRole userRole = UserRole.PropertyManager)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = await _service.UpdateMaintenanceRequestAsync(id, dto, userRole);
            return Ok(request);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMaintenanceRequest(int id)
    {
        try
        {
            var result = await _service.DeleteMaintenanceRequestAsync(id);
            if (!result)
                return NotFound($"Maintenance request with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
